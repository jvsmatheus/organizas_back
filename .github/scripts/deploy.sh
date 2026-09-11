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

systemctl stop organizas

if activate_release "$release_dir" &&
   systemctl start organizas &&
   check_api; then
  echo "Deploy concluído: $release_id"
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