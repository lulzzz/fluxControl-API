CREATE DATABASE FluxControl;
GO

USE FluxControl;
GO

CREATE TABLE Companies
(
	Id					INT				NOT NULL	IDENTITY	PRIMARY KEY,
	Name				VARCHAR(60)		NOT NULL,
	Thumbnail			VARCHAR(MAX)	NOT NULL,
	Invoice_Interval	SMALLINT		NOT NULL	DEFAULT 30
);
GO

CREATE TABLE Buses
(
	Id				INT				NOT NULL	IDENTITY PRIMARY KEY,
	Number			INT				NOT NULL,
	LicensePlate	VARCHAR(10)		NOT NULL,

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
	Email			VARCHAR(60)		NOT NULL,
	Password		VARCHAR(MAX),
	Type			SMALLINT		NOT NULL				REFERENCES UserTypes
);
GO

CREATE TABLE Occurrences
(
	Id			INT			NOT NULL	IDENTITY	PRIMARY KEY,
	Type		SMALLINT	NOT NULL				REFERENCES UserTypes,

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
	Id			INT		NOT NULL	IDENTITY	PRIMARY KEY,
	Total		MONEY	NOT NULL,

	Company_Id	INT		NOT	NULL				REFERENCES Companies
);
GO

CREATE TABLE FlowRecords
(
	Id				INT			NOT NULL	IDENTITY	PRIMARY KEY,
	Arrival			DATETIME	NOT NULL,
	Departure		DATETIME,

	"User_Id"		INT			NOT NULL				REFERENCES Users,
	Bus_Id			INT			NOT NULL				REFERENCES Buses,
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