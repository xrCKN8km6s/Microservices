Get-Content sql/Orders/*.sql,sql/IntegrationEventLog/*.sql | docker exec -i postgres psql -d Orders -U db_user

Get-Content sql/Users/*.sql | docker exec -i postgres psql -d Users -U db_user