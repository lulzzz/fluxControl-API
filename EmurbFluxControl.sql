CREATE DATABASE FluxControl;
GO

USE FluxControl;
GO

CREATE TABLE Companies
(
	Id					INT				NOT NULL	IDENTITY	PRIMARY KEY,
	Name				VARCHAR(60)		NOT NULL,
	Thumbnail			VARCHAR(MAX)	NOT NULL,
	Invoice_Interval	SMALLINT		NOT NULL	DEFAULT 30,
	Inactive			BIT							DEFAULT 0	
);
GO

CREATE TABLE Buses
(
	Id				INT				NOT NULL	IDENTITY PRIMARY KEY,
	Number			INT				NOT NULL,
	LicensePlate	VARCHAR(10)		NOT NULL,
	Inactive		BIT							DEFAULT 0,

	Company_Id		INT				NOT NULL	REFERENCES Companies
);
GO

CREATE TABLE UserTypes
(
	Id		SMALLINT		NOT NULL	IDENTITY	PRIMARY KEY,
	Name	VARCHAR(30)		NOT NULL
);
GO

CREATE TABLE Users
(
	Id				INT				NOT NULL	IDENTITY	PRIMARY KEY,
	Name			VARCHAR(60)		NOT NULL,
	Registration	INT				NOT NULL	UNIQUE,
	Email			VARCHAR(60)		NOT NULL	UNIQUE,
	Password		VARCHAR(MAX),
	Type			SMALLINT		NOT NULL				REFERENCES UserTypes,
	CreationDate	DATE			NOT NULL,
	Inactive		BIT							DEFAULT 0
);
GO

CREATE TABLE ProviderRules
(
	Id					INT			NOT NULL	IDENTITY	PRIMARY KEY,
	Tax					MONEY		NOT NULL,
	IntervalMinutes		INT			NOT NULL
);
GO

CREATE TABLE Occurrences
(
	Id			INT			NOT NULL	IDENTITY	PRIMARY KEY,
	Type		SMALLINT	NOT NULL				REFERENCES UserTypes,
	Inactive	BIT						DEFAULT 0,

	"User_Id"	INT			NOT NULL				REFERENCES Users
);
GO

CREATE TABLE Bus_Occurrences
(
	Justification	VARCHAR(MAX)	NOT NULL,

	Occurrence_Id	INT				NOT NULL	REFERENCES Occurrences,
	Bus_Id			INT				NOT NULL	REFERENCES Buses,

	CONSTRAINT PK_Bus_Ocurrences	PRIMARY KEY(Occurrence_Id, Bus_Id)
);
GO

CREATE TABLE Invoices
(
	Id					INT			NOT NULL	IDENTITY	PRIMARY KEY,
	GenerationDate		DATETIME	NOT NULL,
	TaxConsidered		MONEY		NOT NULL,
	IntervalConsidered	DATETIME	NOT NULL,
	Total				MONEY		NOT NULL,
	Inactive			BIT						DEFAULT 0

	Company_Id			INT				NOT	NULL			REFERENCES Companies
);
GO

CREATE TABLE FlowRecords
(
	Id				INT			NOT NULL	IDENTITY	PRIMARY KEY,
	Arrival			DATETIME	NOT NULL,
	Departure		DATETIME,
	Inactive		BIT						DEFAULT 0,

	Bus_Id			INT			NOT NULL				REFERENCES Buses,
	"User_Id"		INT			NOT NULL				REFERENCES Users,
	Invoice_Id		INT									REFERENCES Invoices
);
GO

CREATE TABLE Tokens
(
	Code		INT				NOT NULL	IDENTITY	PRIMARY KEY,
	Hash		VARCHAR(MAX)	NOT NULL,
	Expires		DATETIME		NOT NULL,

	"User_Id"	INT				NOT NULL	UNIQUE		REFERENCES Users,
				
);
GO