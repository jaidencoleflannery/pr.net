#!/usr/bin/env bash

ENVIRONMENT="$1"

if [[ -z "$1" ]]; then
    read -r -p "* Environment: " ENVIRONMENT
fi

export ASPNETCORE_ENVIRONMENT="$ENVIRONMENT"
export DOTNET_ENVIRONMENT="$ENVIRONMENT"

dotnet build ./pr.net.sln
./src/pr.net/bin/Debug/net10.0/pr.net

