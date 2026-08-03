FROM mcr.microsoft.com/mssql/server:2022-latest

USER root

RUN apt-get update \
    && DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends ca-certificates curl \
    && curl -fsSL \
        https://packages.microsoft.com/config/ubuntu/22.04/mssql-server-2022.list \
        -o /etc/apt/sources.list.d/mssql-server-2022.list \
    && apt-get update \
    && DEBIAN_FRONTEND=noninteractive apt-get install -y --no-install-recommends mssql-server-fts \
    && rm -rf /var/lib/apt/lists/*

USER mssql
