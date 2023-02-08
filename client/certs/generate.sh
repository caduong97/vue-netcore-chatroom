#!/bin/bash
openssl genrsa -out localhost.key 4096
openssl req -new -sha256 \
    -out localhost.csr \
    -key localhost.key \
    -config ssl.conf 
openssl x509 -req \
    -sha256 \
    -days 3650 \
    -in localhost.csr \
    -signkey localhost.key \
    -out localhost.crt \
    -extensions req_ext \
    -extfile ssl.conf
echo "Next, enter your sudo password:"
sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain localhost.crt
openssl x509 -in localhost.crt -out localhost.pem -outform PEM