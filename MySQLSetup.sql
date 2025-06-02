USE matrixdb;

CREATE TABLE Roles (
    RoleId INT AUTO_INCREMENT PRIMARY KEY,
    RoleName VARCHAR(50) UNIQUE NOT NULL
);

CREATE TABLE Users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    Password VARCHAR(100) NOT NULL,
    RoleId INT DEFAULT 1,
    Name VARCHAR(100) NOT NULL,
    Address VARCHAR(255),
    Zipcode VARCHAR(10),
    City VARCHAR(100),
    PhoneNumber VARCHAR(20),
    Email VARCHAR(100) UNIQUE,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);

CREATE TABLE Categories (
    CategoryId INT AUTO_INCREMENT PRIMARY KEY,
    CategoryName VARCHAR(100) NOT NULL
);

CREATE TABLE Products (
    ProductId INT AUTO_INCREMENT PRIMARY KEY,
    CategoryId INT NOT NULL,
    Name VARCHAR(100) NOT NULL,
    Description VARCHAR(255),
    Price DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL DEFAULT 0,
    Picture MEDIUMBLOB,
    IsDeleted BOOLEAN NOT NULL DEFAULT FALSE,
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
);

CREATE TABLE Orders (
    OrderId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Status VARCHAR(50) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE TABLE Orderlines (
    OrderlineId INT AUTO_INCREMENT PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Amount INT NOT NULL DEFAULT 1,
    Price DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

INSERT INTO Roles (RoleName) VALUES
    ("Klant"),
    ("Bezorger"),
    ("Administrator");

INSERT INTO Users (Password, RoleId, Name, Address, Zipcode, City, PhoneNumber, Email)
VALUES (
    '$2y$10$WJ.SVMhzHDUMvIAZR4hQc..DrWtl.KtnzBFsmOQgAhGjCC3OHbs8K',
    3,
    'Admin',
    'Admin street',
    '1234AB',
    'Admin city',
    '0612345678',
    'admin@example.com'
);