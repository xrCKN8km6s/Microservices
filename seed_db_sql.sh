#!/bin/bash -x

cat sql/IntegrationEventLog/* | docker exec -i postgres psql -d IntegrationEventLog -U db_user

cat sql/Orders/* | docker exec -i postgres psql -d Orders -U db_user

cat sql/Users/* | docker exec -i postgres psql -d Users -U db_user