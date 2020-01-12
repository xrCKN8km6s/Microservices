#!/usr/bin/env pwsh

Param (
    [Parameter(Position=0)][string[]] $cns = "localhost",
    [Parameter(Position=1)][string] $passwd = "qwerty1234",
    [Parameter(Position=2)][string] $dir = 'certs',
    [Parameter(Position=3)][string] $days = 1095
)

$rootCAKey = "$dir/rootCA.key"
$rootCACert = "$dir/rootCA.crt"

if (-not(Test-Path $rootCAKey -PathType Leaf) -and
    -not(Test-Path $rootCACert -PathType Leaf)) {

    openssl req -x509 -sha256 -newkey rsa:2048 -days $days `
        -keyout $rootCAKey -out $rootCACert -passout "pass:$passwd" `
        -subj "/C=UA/O=MicroservicesDEV/CN=rootCA"

}

foreach ($cn in $cns) {

"[req]
prompt                      = no
default_bits                = 2048
distinguished_name          = req_distinguished_name
req_extensions              = req_ext
[req_distinguished_name]
countryName                 = UA
organizationName            = MicroservicesDEV
commonName                  = $cn
[req_ext]
subjectAltName              = @alt_names
[alt_names]
DNS.1                       = $cn" > "$dir/$cn.conf"

    openssl genrsa -out "$dir/$cn.key" 2048

    openssl req -new -sha256 -key "$dir/$cn.key" -out "$dir/$cn.csr" -config "$dir/$cn.conf"

    openssl x509 -req -in "$dir/$cn.csr" -CA $rootCACert -CAkey $rootCAKey `
        -CAcreateserial -out "$dir/$cn.crt" -days $days -sha256 -passin "pass:$passwd" `
        -extensions req_ext -extfile "$dir/$cn.conf"

    openssl pkcs12 -export -passin "pass:$passwd" -passout "pass:$passwd" `
        -out "$dir/$cn.pfx" `
        -inkey "$dir/$cn.key" `
        -in "$dir/$cn.crt";

    Remove-Item "$dir/$cn.csr", "$dir/$cn.conf"
}
