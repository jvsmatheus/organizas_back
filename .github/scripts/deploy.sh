#!/usr/bin/env bash
set -euo pipefail

package_uri="${1:?Informe a URI S3 do pacote}"
release_id="${2:?Informe o identificador da publicação}"

if [[ ! "$release_id" =~ ^[a-f0-9]{40}-[0-9]+-[0-9]+$ ]]; then
  echo "Identificador de publicação inválido."
  exit 1
fi

release_dir="/var/www/organizas-releases/$release_id"
previous_release=$(readlink -f /var/www/organizas)

if [[ ! -L /var/www/organizas || ! -f "$previous_release/Organizas.dll" ]]; then
  echo "A publicação atual não possui a estrutura esperada."
  exit 1
fi

mkdir "$release_dir"

aws s3 cp "$package_uri" "$release_dir/package.tar.gz" \
  --region us-east-1 --no-progress

tar --extract --gzip \
  --file "$release_dir/package.tar.gz" \
  --directory "$release_dir" \
  --no-same-owner

if [[ ! -f "$release_dir/Organizas.dll" ]]; then
  echo "O pacote não contém Organizas.dll."
  exit 1
fi

check_api() {
  for attempt in {1..15}; do
    if curl --fail --silent --show-error \
      --connect-timeout 2 \
      --max-time 5 \
      --output /dev/null \
      http://127.0.0.1:5000/ShoppingList; then
      return 0
    fi

    sleep 2
  done

  return 1
}

activate_release() {
  local target="$1"
  local temporary_link="/var/www/.organizas-$release_id"

  ln -sTf "$target" "$temporary_link" || return 1
  mv -Tf "$temporary_link" /var/www/organizas || return 1
}

cleanup_releases() {
  local releases_root="/var/www/organizas-releases"
  local active_release candidates timestamp name candidate resolved
  local kept_extra=0

  active_release=$(readlink -f /var/www/organizas) || return 1
  if [[ "$active_release" != "$release_dir" ]]; then
    echo "Limpeza cancelada: a versão ativa mudou."
    return 1
  fi

  # Ordena as pastas pela data de modificação, da mais recente à mais antiga.
  candidates=$(find "$releases_root" -mindepth 1 -maxdepth 1 -type d \
    -printf '%T@ %f\n' | sort -nr) || return 1

  while read -r timestamp name; do
    # Só considera os nomes de publicações reconhecidos por esta esteira.
    if [[ "$name" != "manual-inicial" &&
          ! "$name" =~ ^[a-f0-9]{40}-[0-9]+-[0-9]+$ ]]; then
      continue
    fi

    candidate="$releases_root/$name"
    resolved=$(readlink -f "$candidate") || return 1

    # Nunca segue links nem remove um caminho fora da pasta de versões.
    if [[ -L "$candidate" || "$resolved" != "$candidate" ]]; then
      echo "Limpeza ignorada para caminho inesperado: $candidate"
      continue
    fi

    if [[ "$candidate" == "$active_release" ||
          "$candidate" == "$previous_release" ]]; then
      continue
    fi

    if [[ "$kept_extra" -eq 0 && -f "$candidate/Organizas.dll" ]]; then
      kept_extra=1
      echo "Versão adicional preservada: $name"
      continue
    fi

    rm -r -- "$candidate" || return 1
    echo "Versão antiga removida: $name"
  done <<< "$candidates"

  return 0
}

systemctl stop organizas

if activate_release "$release_dir" &&
   systemctl start organizas &&
   check_api; then
  echo "Deploy concluído: $release_id"

  # Falha de limpeza não desfaz uma implantação saudável.
  if ! cleanup_releases; then
    echo "AVISO: deploy concluído, mas a limpeza de versões precisa ser verificada."
  fi
else
  echo "Falha no deploy. Restaurando a publicação anterior."

  systemctl stop organizas
  activate_release "$previous_release"
  systemctl start organizas

  if check_api; then
    echo "Publicação anterior restaurada."
  else
    echo "A publicação anterior também não respondeu. Verifique o serviço."
  fi

  exit 1
fi