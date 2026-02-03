-- Initialize PetWorld Database Schema
CREATE DATABASE IF NOT EXISTS PetWorldDb;
USE PetWorldDb;

-- Products Table
CREATE TABLE IF NOT EXISTS Products (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    Description VARCHAR(1000),
    Category VARCHAR(100) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    PetType VARCHAR(50) NOT NULL,
    Brand VARCHAR(100),
    InStock TINYINT(1) NOT NULL DEFAULT 1,
    StockQuantity INT NOT NULL DEFAULT 0,
    ImageUrl VARCHAR(500),
    Tags VARCHAR(500),
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- ChatHistories Table
CREATE TABLE IF NOT EXISTS ChatHistories (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Data DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Pytanie TEXT NOT NULL,
    Odpowiedz TEXT NOT NULL,
    LiczbaIteracji INT NOT NULL,
    RecommendedProducts TEXT,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Insert Product Catalog
INSERT INTO Products (Name, Description, Category, Price, PetType, Brand, InStock, StockQuantity, CreatedAt, UpdatedAt)
VALUES
-- Karma dla psów
('Royal Canin Adult Dog 15kg', 'Premium karma dla dorosłych psów średnich ras', 'Karma', 289.00, 'Pies', 'Royal Canin', 1, 45, NOW(), NOW()),

-- Karma dla kotów
('Whiskas Adult Kurczak 7kg', 'Sucha karma dla dorosłych kotów z kurczakiem', 'Karma', 129.00, 'Kot', 'Whiskas', 1, 78, NOW(), NOW()),
('Brit Premium Kitten 8kg', 'Karma dla kociąt do 12 miesiąca życia', 'Karma', 159.00, 'Kot', 'Brit', 1, 52, NOW(), NOW()),

-- Akwarystyka
('Tetra AquaSafe 500ml', 'Uzdatniacz wody do akwarium, neutralizuje chlor', 'Akwarystyka', 45.00, 'Ryby', 'Tetra', 1, 120, NOW(), NOW()),
('JBL ProFlora CO2 Set', 'Kompletny zestaw CO2 dla roślin akwariowych', 'Akwarystyka', 549.00, 'Ryby', 'JBL', 1, 15, NOW(), NOW()),

-- Akcesoria dla kotów
('Trixie Drapak XL 150cm', 'Wysoki drapak z platformami i domkiem', 'Akcesoria', 399.00, 'Kot', 'Trixie', 1, 22, NOW(), NOW()),

-- Zabawki i akcesoria dla psów
('Kong Classic Large', 'Wytrzymała zabawka do napełniania smakołykami', 'Zabawki', 69.00, 'Pies', 'Kong', 1, 65, NOW(), NOW()),
('Flexi Smycz automatyczna 8m', 'Smycz zwijana dla psów do 50kg', 'Akcesoria', 119.00, 'Pies', 'Flexi', 1, 38, NOW(), NOW()),

-- Gryzonie
('Ferplast Klatka dla chomika', 'Klatka 60x40cm z wyposażeniem', 'Klatki i transportery', 189.00, 'Gryzoń', 'Ferplast', 1, 28, NOW(), NOW()),
('Vitapol Siano dla królików 1kg', 'Naturalne siano łąkowe, podstawa diety', 'Karma', 25.00, 'Gryzoń', 'Vitapol', 1, 150, NOW(), NOW());

SELECT 'Database initialized successfully!' as Status;
SELECT COUNT(*) as TotalProducts FROM Products;
