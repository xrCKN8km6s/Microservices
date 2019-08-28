Get-Content sql/IntegrationEventLog/*.sql | docker exec -i postgres psql -d IntegrationEventLog -U db_user

Get-Content sql/Orders/*.sql | docker exec -i postgres psql -d Orders -U db_user

Get-Content sql/Users/*.sql | docker exec -i postgres psql -d Users -U db_user