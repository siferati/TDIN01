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
	password TEXT  NOT NULL,
	money REAL NOT NULL DEFAULT 0.0
);

CREATE TABLE Diginotes (
	id INTEGER PRIMARY KEY,
	userId INTEGER NOT NULL REFERENCES Users(id)
);

CREATE Table Quote (
	id INTEGER PRIMARY KEY,
	timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
	value REAL NOT NULL
);

CREATE Table SellingOrders (
	id INTEGER PRIMARY KEY,
	timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
	userId INTEGER NOT NULL REFERENCES Users(id),
	amount INTEGER NOT NULL
);

CREATE Table PurchaseOrders (
	id INTEGER PRIMARY KEY,
	timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
	userId INTEGER NOT NULL REFERENCES Users(id),
	amount INTEGER NOT NULL
);

CREATE Table CompletedOrders (
	sellingOrderId INTEGER REFERENCES SellingOrders(id),
	purchaseOrderId INTEGER REFERENCES PurchaseOrders(id),
	timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
	amount INTEGER NOT NULL,
	PRIMARY KEY (sellingOrderId, purchaseOrderId)
);

/* Test data */
INSERT INTO Quote ("value") VALUES (1.0);
INSERT INTO Users ("money", "name", "username", "password") VALUES (100, "Teste", "user", "d74ff0ee8da3b9806b18c877dbf29bbde50b5bd8e4dad7a3a725000feb82e8f1");
INSERT INTO Diginotes ("userId") VALUES (1);
INSERT INTO Diginotes ("userId") VALUES (1);
INSERT INTO Diginotes ("userId") VALUES (1);
INSERT INTO Diginotes ("userId") VALUES (1);
INSERT INTO Diginotes ("userId") VALUES (1);
INSERT INTO Diginotes ("userId") VALUES (1);


INSERT INTO Users ("money", "name", "username", "password") VALUES (200, "Um", "user1", "e6c3da5b206634d7f3f3586d747ffdb36b5c675757b380c6a5fe5c570c714349");
INSERT INTO Diginotes ("userId") VALUES (2);
INSERT INTO Diginotes ("userId") VALUES (2);
INSERT INTO Diginotes ("userId") VALUES (2);

INSERT INTO PurchaseOrders ("userId", "amount") VALUES (2, 2);