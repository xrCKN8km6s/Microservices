#!/usr/bin/env pwsh

Param (
    [Parameter(Position=0)][string[]] $cns = $(throw "-cn is required"),
    [Parameter(Position=1)][string] $passwd = $(throw "-passwd is required")
)

foreach ($cn in $cns) {

    $FULL_PATH_NO_EXT="certs/$cn"

    openssl req -x509 -sha256 -newkey rsa:2048 -days 1095 -subj "/CN=$cn" -passout "pass:$passwd" `
        -keyout "$FULL_PATH_NO_EXT.key" `
        -out "$FULL_PATH_NO_EXT.crt";

    openssl pkcs12 -export -passin "pass:$passwd" -passout "pass:$passwd" `
        -out "$FULL_PATH_NO_EXT.pfx" `
        -inkey "$FULL_PATH_NO_EXT.key" `
        -in "$FULL_PATH_NO_EXT.crt";
}
