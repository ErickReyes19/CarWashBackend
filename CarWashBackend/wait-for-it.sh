#!/usr/bin/env bash
host="$1"
shift
cmd="$@"

until nc -z "$host" 3306; do
  echo "Esperando a que MySQL esté disponible..."
  sleep 2
done

exec $cmd
