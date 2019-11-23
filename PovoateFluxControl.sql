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
VALUES (27322, 'FGH1234', GETDATE(), NULL, 1)

INSERT INTO Buses
VALUES (27330, 'IJK1234', GETDATE(), NULL, 2)
INSERT INTO Buses
VALUES (27331, 'LMN1234', GETDATE(), NULL, 2)






DELETE FROM FlowRecords WHERE Bus_Id = 10
DELETE FROM Buses WHERE Id = 10

INSERT INTO Buses 
VALUES (0002, 'ABC2357', GETDATE(), NULL, 2)

INSERT INTO Buses 
VALUES (0003, 'PLA0000', GETDATE(), NULL, 1)

SELECT * FROM Buses;










sp_help FlowRecords;
SELECT * FROM FlowRecords

INSERT INTO ProviderRules (IntervalMinutes, Tax) VALUES (1, 50);

sp_help ProviderRules;
select * from ProviderRules;

select * from Invoices