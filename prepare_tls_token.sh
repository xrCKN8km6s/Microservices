#!/bin/bash

CN=$1
PASSWD=$2

FULL_PATH_NO_EXT="certs/$CN"

openssl req -x509 -sha256 -newkey rsa:2048 -keyout $FULL_PATH_NO_EXT.key -out $FULL_PATH_NO_EXT.crt -days 1095 -subj '/CN='$CN'' -passout pass:$PASSWD
openssl pkcs12 -export -out $FULL_PATH_NO_EXT.pfx -inkey $FULL_PATH_NO_EXT.key -in $FULL_PATH_NO_EXT.crt -passin pass:$PASSWD -passout pass:$PASSWD
