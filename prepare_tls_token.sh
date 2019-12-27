#!/bin/bash

TARGET_DIR=$1
CERT_NAME=$2
COMMON_NAME=$3
KEY_PASSWORD=$4
PFX_PASSWORD=$5
CRT_COPY_TO=$6

FULL_PATH="$TARGET_DIR/$CERT_NAME"


openssl req -x509 -sha256 -newkey rsa:2048 -keyout $FULL_PATH.key -out $FULL_PATH.crt -days 1095 -subj '/CN='$COMMON_NAME'' -passout pass:$KEY_PASSWORD
openssl pkcs12 -export -out $FULL_PATH.pfx -inkey $FULL_PATH.key -in $FULL_PATH.crt -passin pass:$KEY_PASSWORD -passout pass:$PFX_PASSWORD

cp $FULL_PATH.crt $CRT_COPY_TO
