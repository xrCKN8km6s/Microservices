#!/usr/bin/env pwsh

Param (
    [Parameter(Position=0)][string[]] $cns = "localhost",
    [Parameter(Position=1)][string] $passwd = "qwerty1234",
    [Parameter(Position=2)][string] $dir = 'certs',
    [Parameter(Position=3)][string] $curve = 'prime256v1',
    [Parameter(Position=4)][string] $days = 1095
)

$rootCAKey = "$dir/rootCA.pem"
$rootCACert = "$dir/rootCA.crt"

if (-not(Test-Path $rootCAKey -PathType Leaf) -and
    -not(Test-Path $rootCACert -PathType Leaf)) {

    openssl ecparam -name $curve -genkey -out $rootCAKey

    openssl req -x509 -new -key $rootCAKey -days $days `
        -subj "/C=UA/O=MicroservicesDEV/CN=rootCA" `
        -out $rootCACert -passout "pass:$passwd"
}

foreach ($cn in $cns) {

"[req]
prompt                      = no
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

    openssl ecparam -name $curve -genkey -out "$dir/$cn.pem"
        
    openssl req -new -sha256 -config "$dir/$cn.conf" -nodes -key "$dir/$cn.pem" -out "$dir/$cn.csr"
    
    openssl x509 -req -in "$dir/$cn.csr" -CA $rootCACert -CAkey $rootCAKey `
        -CAcreateserial -out "$dir/$cn.crt" -days $days -sha256 -passin "pass:$passwd" `
        -extensions req_ext -extfile "$dir/$cn.conf"
        
    openssl pkcs12 -export -passin "pass:$passwd" -passout "pass:$passwd" `
        -out "$dir/$cn.pfx" `
        -inkey "$dir/$cn.pem" `
        -in "$dir/$cn.crt";
        
    Remove-Item "$dir/$cn.csr", "$dir/$cn.conf"
}
