#!/usr/bin/env bash
set -euo pipefail

package_uri="${1:?Informe a URI S3 do pacote}"

migration_dir=$(mktemp -d /tmp/organizas-migrations.XXXXXX)

aws s3 cp "$package_uri" "$migration_dir/package.tar.gz" \
  --region us-east-1

mkdir "$migration_dir/app"

tar --extract --gzip \
  --file "$migration_dir/package.tar.gz" \
  --directory "$migration_dir/app" \
  --no-same-owner

chown -R ubuntu:ubuntu "$migration_dir"
chmod u+x "$migration_dir/app/efbundle"

systemd-run \
  --wait \
  --pipe \
  --collect \
  --property=User=ubuntu \
  --property=EnvironmentFile=/etc/organizas/organizas.env \
  --working-directory="$migration_dir/app" \
  "$migration_dir/app/efbundle"