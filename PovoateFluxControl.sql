USE FluxControl;
GO

INSERT INTO UserTypes
VALUES ('Transparência');
INSERT INTO UserTypes
VALUES ('Operador');
INSERT INTO UserTypes
VALUES ('Gerente');
INSERT INTO UserTypes
VALUES ('Administrador');
select * from UserTypes;

INSERT INTO Users 
VALUES ('Ottoniel Matheus', 100419, 'ottonielrp2008@gmail.com', '123456', 4, GETDATE(), NULL),
('Caio Santanna', 101419, 'caiorsantanna@gmail.com', '123456', 2, GETDATE(), NULL),
('Gabriel Borges', 102419, 'borges@gmail.com', '123456', 3, GETDATE(), NULL);
select * from Users;

INSERT INTO Companies
VALUES ('Itamarati', '/url/image1.png', 30, GETDATE(), NULL)
INSERT INTO Companies
VALUES ('Cometa', '/url/image2.png', 7, GETDATE(), NULL)
INSERT INTO Companies
VALUES ('Santa Luzia', '/url/image3.png', 15, GETDATE(), NULL)
GO
select * from Companies;

INSERT INTO Buses
VALUES (27320, 'ABC-1234', GETDATE(), NULL, 1)
INSERT INTO Buses
VALUES (27321, 'DEF-1234', GETDATE(), NULL, 1)
INSERT INTO Buses
VALUES (27322, 'FGH-1234', GETDATE(), NULL, 1)

INSERT INTO Buses
VALUES (27330, 'IJK-1234', GETDATE(), NULL, 2)
INSERT INTO Buses
VALUES (27331, 'LMN-1234', GETDATE(), NULL, 2)

INSERT INTO Buses
VALUES (27342, 'OPQ-1234', GETDATE(), NULL, 3)
select * from Buses;
INSERT INTO ProviderRules (IntervalMinutes, Tax) VALUES (1, 50);
select * from ProviderRules;

INSERT INTO FlowRecords (Arrival, Departure, Bus_Id, User_Id, Invoice_Id)
VALUES (GETDATE(), NULL, 1, 1, NULL);
select * from FlowRecords;

INSERT INTO Invoices (

select * from Invoices;
