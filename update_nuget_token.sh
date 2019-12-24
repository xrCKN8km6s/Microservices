#!/bin/bash

CURRENT=$1
NEW=$2

sed -i 's/'$CURRENT'/'$NEW'/g' $(find . -maxdepth 3 -type f -name 'NuGet.Config')
