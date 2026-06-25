#!/usr/bin/env bash

ENVIRONMENT="$1"

if [[ -z "$1" ]]; then
    read -r -p "* Environment: " ENVIRONMENT
fi

echo "* Running environment: $ENVIRONMENT"

export ASPNETCORE_ENVIRONMENT="$ENVIRONMENT"
export DOTNET_ENVIRONMENT="$ENVIRONMENT"

echo "* Set environment variables: "
echo "* ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT"
echo "* DOTNET_ENVIRONMENT=$DOTNET_ENVIRONMENT"

dotnet build ./pr.net.sln
./src/pr.net/bin/Debug/net10.0/pr.net
