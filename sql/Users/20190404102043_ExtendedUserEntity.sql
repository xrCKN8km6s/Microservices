
DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190404102043_ExtendedUserEntity') THEN
    ALTER TABLE users ADD email text NOT NULL DEFAULT '';
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190404102043_ExtendedUserEntity') THEN
    ALTER TABLE users ADD name text NOT NULL DEFAULT '';
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190404102043_ExtendedUserEntity') THEN
    UPDATE users SET name = 'Alice Smith', email = 'AliceSmith@email.com'
    WHERE sub = 'affc1ed6-e923-461f-8199-e95c07dc373b';
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190404102043_ExtendedUserEntity') THEN
    UPDATE users SET name = 'Bob Smith', email = 'BobSmith@email.com'
    WHERE sub = '6ddbe58f-b173-47b5-bc65-848833a93ba2';
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190404102043_ExtendedUserEntity') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20190404102043_ExtendedUserEntity', '2.2.6-servicing-10079');
    END IF;
END $$;

