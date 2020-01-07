#!/usr/bin/env pwsh

Param (
    [Parameter(Position=0)][string[]] $cns = $(throw "-cns is required"),
    [Parameter(Position=1)][string] $passwd = $(throw "-passwd is required"),
    [Parameter(Position=2)][string] $dir = 'certs'
)

openssl req -x509 -sha256 -newkey rsa:2048 -days 1095 `
    -keyout "$dir/rootCA.key" -out "$dir/rootCA.crt" -passout "pass:$passwd" `
    -subj "/C=UA/O=MicroservicesDEV/CN=rootCA"

foreach ($cn in $cns) {

"[req]
prompt                      = no
default_bits                = 2048
distinguished_name          = req_distinguished_name
req_extensions              = req_ext
[req_distinguished_name]
C=UA
O=MicroservicesDEV
CN=$cn
[req_ext]
subjectAltName              = @alt_names
[alt_names]
DNS.1                       = $cn" | Out-File "$dir/$cn.conf"

    openssl genrsa -out "$dir/$cn.key" 2048

    openssl req -new -sha256 -key "$dir/$cn.key" -out "$dir/$cn.csr" -config "$dir/$cn.conf"

    openssl x509 -req -in "$dir/$cn.csr" -CA "$dir/rootCA.crt" -CAkey "$dir/rootCA.key" `
        -CAcreateserial -out "$dir/$cn.crt" -days 1095 -sha256 -passin "pass:$passwd" -extensions req_ext -extfile "$dir/$cn.conf"

    openssl pkcs12 -export -passin "pass:$passwd" -passout "pass:$passwd" `
        -out "$dir/$cn.pfx" `
        -inkey "$dir/$cn.key" `
        -in "$dir/$cn.crt";

    Remove-Item "$dir/$cn.csr", "$dir/$cn.conf"
}
