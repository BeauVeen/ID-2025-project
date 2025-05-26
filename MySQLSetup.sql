USE matrixdb;

CREATE TABLE Gebruiker (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    Wachtwoord CHAR(60) NOT NULL,
    Rol VARCHAR(20),
    Naam VARCHAR(100) NOT NULL,
    Adres VARCHAR(255),
    Postcode VARCHAR(20),
    Woonplaats VARCHAR(100),
    Telefoon VARCHAR(20),
    Email VARCHAR(100) UNIQUE
);

CREATE TABLE Categorie (
    CategorieId INT AUTO_INCREMENT PRIMARY KEY,
    CategorieNaam VARCHAR(100) NOT NULL
);

CREATE TABLE Producten (
    ProductId INT AUTO_INCREMENT PRIMARY KEY,
    CategorieId INT,
    Naam VARCHAR(100) NOT NULL,
    Beschrijving VARCHAR(255),
    Prijs DECIMAL(10,2) NOT NULL,
    Voorraad INT NOT NULL DEFAULT 0,
    Afbeelding BLOB,
    FOREIGN KEY (CategorieId) REFERENCES Categorie(CategorieId)
);

CREATE TABLE Bestelling (
    BestelId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    Datum DATE NOT NULL,
    Status VARCHAR(50) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Gebruiker(UserId)
);

CREATE TABLE Bestelregel (
    BestelregelId INT AUTO_INCREMENT PRIMARY KEY,
    BestelId INT NOT NULL,
    ProductId INT NOT NULL,
    Aantal INT NOT NULL DEFAULT 1,
    Prijs DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (BestelId) REFERENCES Bestelling(BestelId),
    FOREIGN KEY (ProductId) REFERENCES Producten(ProductId)
);