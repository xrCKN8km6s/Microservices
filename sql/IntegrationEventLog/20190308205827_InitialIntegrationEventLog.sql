CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);


DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190308205827_InitialIntegrationEventLog') THEN
    CREATE TABLE integration_event_logs (
        "Id" uuid NOT NULL,
        "Name" text NOT NULL,
        "CreatedDate" timestamp without time zone NOT NULL,
        "TimesSent" integer NOT NULL,
        "Content" json NOT NULL,
        "State" integer NOT NULL,
        CONSTRAINT "PK_integration_event_logs" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190308205827_InitialIntegrationEventLog') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20190308205827_InitialIntegrationEventLog', '2.2.6-servicing-10079');
    END IF;
END $$;

