#!/usr/bin/env pwsh

Param (
    [Parameter(Position=0)][string[]] $cns = $(throw "-cn is required"),
    [Parameter(Position=1)][string] $passwd = $(throw "-passwd is required"),
    [Parameter(Position=2)][string] $dir = 'certs'
)

openssl req -x509 -sha256 -newkey rsa:2048 -days 1095 `
    -keyout "$dir/rootCA.key" -out "$dir/rootCA.crt" -passout "pass:$passwd"

foreach ($cn in $cns) {

    openssl genrsa -out "$dir/$cn.key" 2048

    openssl req -new -sha256 -key "$dir/$cn.key" -subj "/CN=$cn" -out "$dir/$cn.csr"

    openssl x509 -req -in "$dir/$cn.csr" -CA "$dir/rootCA.crt" -CAkey "$dir/rootCA.key" `
        -CAcreateserial -out "$dir/$cn.crt" -days 1095 -sha256 -passin "pass:$passwd"

    openssl pkcs12 -export -passin "pass:$passwd" -passout "pass:$passwd" `
        -out "$dir/$cn.pfx" `
        -inkey "$dir/$cn.key" `
        -in "$dir/$cn.crt";

    Remove-Item "$dir/$cn.csr"
}
