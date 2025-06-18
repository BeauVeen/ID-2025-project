USE matrixdb;

CREATE TABLE Roles (
    RoleId INT AUTO_INCREMENT PRIMARY KEY,
    RoleName VARCHAR(50) UNIQUE NOT NULL
);

CREATE TABLE Users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    PasswordHash VARCHAR(100) NOT NULL,
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

CREATE TABLE OrderStatus (
    Status VARCHAR(50) PRIMARY KEY
);

INSERT INTO OrderStatus (Status) VALUES
    ('In behandeling'),
    ('Klaar voor verzending'),
    ('Onderweg'),
    ('Afgeleverd'),
    ('Geannuleerd'),
    ('Niet aanwezig');

CREATE TABLE Orders (
    OrderId INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Status VARCHAR(50) NOT NULL DEFAULT 'In behandeling',
    Signature MEDIUMBLOB NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (Status) REFERENCES OrderStatus(Status)
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

CREATE TABLE Container (
    ContainerId INT AUTO_INCREMENT PRIMARY KEY,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Status VARCHAR(50) NOT NULL DEFAULT 'In behandeling',
    FOREIGN KEY (Status) REFERENCES OrderStatus(Status)
);

CREATE TABLE ContainerOrders (
    ContainerOrderId INT AUTO_INCREMENT PRIMARY KEY,
    ContainerId INT NOT NULL,
    OrderId INT NOT NULL UNIQUE,
    FOREIGN KEY (ContainerId) REFERENCES Container(ContainerId) ON DELETE CASCADE,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE
);

INSERT INTO Roles (RoleName) VALUES
    ("Klant"),
    ("Bezorger"),
    ("Administrator"),
    ("Magazijn medewerker");

INSERT INTO Users (PasswordHash, RoleId, Name, Address, Zipcode, City, PhoneNumber, Email)
VALUES (
    '$2y$10$WJ.SVMhzHDUMvIAZR4hQc..DrWtl.KtnzBFsmOQgAhGjCC3OHbs8K',
    1,
    'Klant',
    'Test street 2A',
    '1234AB',
    'Test city',
    '0612345678',
    'klant@example.com'
),
(
    '$2y$10$WJ.SVMhzHDUMvIAZR4hQc..DrWtl.KtnzBFsmOQgAhGjCC3OHbs8K',
    2,
    'Bezorger',
    'Test street 2A',
    '1234AB',
    'Test city',
    '0612345678',
    'bezorger@example.com'
),
(
    '$2y$10$WJ.SVMhzHDUMvIAZR4hQc..DrWtl.KtnzBFsmOQgAhGjCC3OHbs8K',
    3,
    'Admin',
    'Test street 2A',
    '1234AB',
    'Test city',
    '0612345678',
    'admin@example.com'
),
(
    '$2y$10$WJ.SVMhzHDUMvIAZR4hQc..DrWtl.KtnzBFsmOQgAhGjCC3OHbs8K',
    4,
    'Magazijn medewerker',
    'Test street 2A',
    '1234AB',
    'Test city',
    '0612345678',
    'mm@example.com'
);