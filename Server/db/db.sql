/*
.mode columns
.headers on
.nullvalue NULL
PRAGMA FOREIGN_KEYS = ON;
*/

CREATE TABLE Users (
	id INTEGER PRIMARY KEY,
	name TEXT  NOT NULL,
	username TEXT  NOT NULL UNIQUE,
	password TEXT  NOT NULL
);

CREATE TABLE Diginotes (
	id INTEGER PRIMARY KEY,
	userId INTEGER NOT NULL,
	FOREIGN KEY (userId) REFERENCES Users(id)
);

CREATE Table Quote (
	id INTEGER PRIMARY KEY,
	timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
	value REAL NOT NULL
);

/* Test data */
INSERT INTO Quote ("value") VALUES (1.0);
INSERT INTO Users ("name", "username", "password") VALUES ("Teste", "user", "pass");
INSERT INTO Diginotes ("userId") VALUES (1);
INSERT INTO Diginotes ("userId") VALUES (1);
INSERT INTO Diginotes ("userId") VALUES (1);

/*
INSERT INTO Users ("name", "username", "password") VALUES ("um", "user1", "pass1");
INSERT INTO Users ("name", "username", "password") VALUES ("dois", "user2", "pass2");
INSERT INTO Users ("name", "username", "password") VALUES ("tres", "user3", "pass3");

INSERT INTO Diginotes ("userId") VALUES (2);
INSERT INTO Diginotes ("userId") VALUES (2);
INSERT INTO Diginotes ("userId") VALUES (3);

INSERT INTO Quote ("value") VALUES (0.87);
*/