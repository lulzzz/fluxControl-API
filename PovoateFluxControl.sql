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

INSERT INTO Users 
VALUES ('Ottoniel Matheus', 100419, 'ottonielrp2008@gmail.com', '123456', 3),
('Caio Santanna', 101419, 'caiorsantanna@gmail.com', '123456', 2),
('Gabriel Borges', 102419, 'borges@gmail.com', '123456', 3);

select * from Users

INSERT INTO Companies
VALUES ('Itamarati', '/url/image1.png', 30)
INSERT INTO Companies
VALUES ('Cometa', '/url/image2.png', 7)
INSERT INTO Companies
VALUES ('Santa Luzia', '/url/image3.png', 15)
GO

INSERT INTO Buses
VALUES (27320, 'ABC-1234', 1)
INSERT INTO Buses
VALUES (27321, 'DEF-1234', 1)
INSERT INTO Buses
VALUES (27322, 'FGH-1234', 1)

INSERT INTO Buses
VALUES (27330, 'IJK-1234', 2)
INSERT INTO Buses
VALUES (27331, 'LMN-1234', 2)

INSERT INTO Buses
VALUES (27342, 'OPQ-1234', 3)

SELECT * FROM Tokens

SELECT company.*, bus.Id Bus_Id, bus.Number, bus.LicensePlate, bus.Company_Id
FROM Companies company
JOIN Buses bus ON bus.Company_Id = company.Id
ORDER BY Company_Id