
DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190312081602_ColumnNamesClarification') THEN
    ALTER TABLE integration_event_logs RENAME COLUMN "Name" TO "EventName";
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190312081602_ColumnNamesClarification') THEN
    ALTER TABLE integration_event_logs RENAME COLUMN "Id" TO "EventId";
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190312081602_ColumnNamesClarification') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20190312081602_ColumnNamesClarification', '2.2.6-servicing-10079');
    END IF;
END $$;

