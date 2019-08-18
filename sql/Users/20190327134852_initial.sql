CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);


DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190327134852_initial') THEN
    CREATE TABLE roles (
        id bigserial NOT NULL,
        name text NOT NULL,
        is_global boolean NOT NULL,
        is_active boolean NOT NULL,
        CONSTRAINT "PK_roles" PRIMARY KEY (id)
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190327134852_initial') THEN
    CREATE TABLE users (
        id bigserial NOT NULL,
        sub text NOT NULL,
        is_active boolean NOT NULL,
        CONSTRAINT "PK_users" PRIMARY KEY (id)
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190327134852_initial') THEN
    CREATE TABLE permission_roles (
        permission bigint NOT NULL,
        role_id bigint NOT NULL,
        CONSTRAINT "PK_permission_roles" PRIMARY KEY (role_id, permission),
        CONSTRAINT "FK_permission_roles_roles_role_id" FOREIGN KEY (role_id) REFERENCES roles (id) ON DELETE CASCADE
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190327134852_initial') THEN
    CREATE TABLE user_roles (
        user_id bigint NOT NULL,
        role_id bigint NOT NULL,
        CONSTRAINT "PK_user_roles" PRIMARY KEY (role_id, user_id),
        CONSTRAINT "FK_user_roles_roles_role_id" FOREIGN KEY (role_id) REFERENCES roles (id) ON DELETE CASCADE,
        CONSTRAINT "FK_user_roles_users_user_id" FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE CASCADE
    );
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190327134852_initial') THEN
    CREATE UNIQUE INDEX "IX_permission_roles_role_id_permission" ON permission_roles (role_id, permission);
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190327134852_initial') THEN
    CREATE INDEX "IX_roles_id" ON roles (id);
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190327134852_initial') THEN
    CREATE INDEX "IX_user_roles_user_id" ON user_roles (user_id);
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190327134852_initial') THEN
    CREATE UNIQUE INDEX "IX_user_roles_role_id_user_id" ON user_roles (role_id, user_id);
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190327134852_initial') THEN
    CREATE INDEX "IX_users_id" ON users (id);
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190327134852_initial') THEN
    INSERT INTO users (sub, is_active)
    VALUES ('affc1ed6-e923-461f-8199-e95c07dc373b', TRUE);
    INSERT INTO users (sub, is_active)
    VALUES ('6ddbe58f-b173-47b5-bc65-848833a93ba2', TRUE);
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190327134852_initial') THEN
    INSERT INTO roles (name, is_global, is_active)
    VALUES ('Admin', TRUE, TRUE);
    INSERT INTO roles (name, is_global, is_active)
    VALUES ('OrdersManager', FALSE, TRUE);
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190327134852_initial') THEN
    INSERT INTO user_roles (role_id, user_id)
    VALUES (1, 1);
    INSERT INTO user_roles (role_id, user_id)
    VALUES (2, 2);
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190327134852_initial') THEN
    INSERT INTO permission_roles (permission, role_id)
    VALUES (1, 2);
    INSERT INTO permission_roles (permission, role_id)
    VALUES (2, 2);
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20190327134852_initial') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20190327134852_initial', '2.2.6-servicing-10079');
    END IF;
END $$;

