#!/usr/bin/env pwsh

Get-Content sql/0_init_db.sql | docker-compose exec -T postgreSql psql -U db_user

Get-Content sql/Orders/*.sql,sql/IntegrationEventLog/*.sql | docker-compose exec -T postgreSql psql -d Orders -U db_user

Get-Content sql/Users/*.sql | docker-compose exec -T postgreSql psql -d Users -U db_user
