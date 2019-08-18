CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);


DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190402155226_Initial') THEN
    CREATE TABLE orders (
        "Id" bigserial NOT NULL,
        "CreationDateTime" timestamp without time zone NOT NULL,
        "Name" text NOT NULL,
        "OrderStatus" integer NOT NULL,
        CONSTRAINT "PK_orders" PRIMARY KEY ("Id")
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190402155226_Initial') THEN
    CREATE INDEX "IX_orders_Id" ON orders ("Id");
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190402155226_Initial') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20190402155226_Initial', '2.2.6-servicing-10079');
    END IF;
END $$;

