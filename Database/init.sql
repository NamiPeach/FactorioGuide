-- FactorioGuide: database schema and test data (SQLite)
PRAGMA foreign_keys = ON;

-- Users of the MAUI app (the single admin is defined in code)
CREATE TABLE Users (
    Id           INTEGER PRIMARY KEY AUTOINCREMENT,
    Name         TEXT NOT NULL,
    Email        TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    CreatedAt    TEXT NOT NULL DEFAULT (datetime('now'))
);

-- Factorio items and buildings. UserId = user whose description edit was approved last (NULL for imported items)
CREATE TABLE Items (
    Id            INTEGER PRIMARY KEY AUTOINCREMENT,
    InternalName  TEXT NOT NULL UNIQUE,
    Name          TEXT NOT NULL,
    Category      TEXT,
    Description   TEXT,
    StackSize     INTEGER,
    CraftTime     REAL,
    ProductAmount INTEGER,
    Stats         TEXT,
    UserId        INTEGER,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE SET NULL
);

-- Recipe of an item: ItemId is crafted from IngredientItemId x Amount
CREATE TABLE RecipeIngredients (
    Id               INTEGER PRIMARY KEY AUTOINCREMENT,
    ItemId           INTEGER NOT NULL,
    IngredientItemId INTEGER NOT NULL,
    Amount           INTEGER NOT NULL CHECK (Amount > 0),
    FOREIGN KEY (ItemId) REFERENCES Items(Id) ON DELETE CASCADE,
    FOREIGN KEY (IngredientItemId) REFERENCES Items(Id) ON DELETE CASCADE
);

-- User requests to add, edit or delete an item description (reviewed by the admin)
CREATE TABLE Actions (
    Id            INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId        INTEGER NOT NULL,
    ItemId        INTEGER NOT NULL,
    Type          TEXT NOT NULL CHECK (Type IN ('Add', 'Edit', 'Delete')),
    ActionDetails TEXT,
    Status        TEXT NOT NULL DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Approved', 'Rejected')),
    AdminComment  TEXT,
    CreatedAt     TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (ItemId) REFERENCES Items(Id) ON DELETE CASCADE
);

-- Blueprints shared by users (no moderation)
CREATE TABLE Blueprints (
    Id              INTEGER PRIMARY KEY AUTOINCREMENT,
    Title           TEXT NOT NULL,
    Description     TEXT,
    BlueprintString TEXT NOT NULL,
    ScreenshotPath  TEXT,
    AuthorId        INTEGER,
    CreatedAt       TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (AuthorId) REFERENCES Users(Id) ON DELETE SET NULL
);

-- Test data (password of both users: user123)
INSERT INTO Users (Id, Name, Email, PasswordHash) VALUES
    (1, 'Test User',  'user1@test.com', '$2b$11$GIeYTa5Uz6zPLI2AwbkJAOhB6eVEW0P6YEnEV6PgGtO5wTWbeffwW'),
    (2, 'Test User 2', 'user2@test.com', '$2b$11$IVm8leFGjvB9x4NoS29XhOU4oKZnD.mXxcMDOyptJncJlGlBpwPKm');

INSERT INTO Items (Id, InternalName, Name, Category, Description, StackSize, CraftTime, ProductAmount, UserId) VALUES
    (1, 'iron-plate',         'Iron plate',         'Intermediate products', 'Basic building material made by smelting iron ore.', 100, 3.2, 1, 2),
    (2, 'copper-plate',       'Copper plate',       'Intermediate products', NULL, 100, 3.2, 1, NULL),
    (3, 'copper-cable',       'Copper cable',       'Intermediate products', NULL, 200, 0.5, 2, NULL),
    (4, 'electronic-circuit', 'Electronic circuit', 'Intermediate products', NULL, 200, 0.5, 1, NULL);

INSERT INTO RecipeIngredients (ItemId, IngredientItemId, Amount) VALUES
    (3, 2, 1),
    (4, 1, 1),
    (4, 3, 3);

INSERT INTO Actions (UserId, ItemId, Type, ActionDetails, Status, AdminComment) VALUES
    (1, 3, 'Add',  'Thin wire used in electronic circuits and power poles.', 'Pending',  NULL),
    (2, 1, 'Edit', 'Basic building material made by smelting iron ore.',      'Approved', 'Looks good');

INSERT INTO Blueprints (Title, Description, BlueprintString, AuthorId) VALUES
    ('Sample blueprint', 'Placeholder entry for testing', 'SAMPLE-BLUEPRINT-STRING', 1);
