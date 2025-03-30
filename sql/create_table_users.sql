USE StockPortfolio;
-- CREATE TABLE UserAccounts (
--     UserID INT PRIMARY KEY IDENTITY(1,1),
--     Username NVARCHAR(50) NOT NULL UNIQUE,
--     Email NVARCHAR(100) NOT NULL UNIQUE,
--     Password NVARCHAR(255) NOT NULL,
--     CreatedAt DATETIME DEFAULT GETDATE(),
--     Cash DECIMAL(10,2) DEFAULT 100
-- );
-- CREATE TABLE Stocks (
--     StockId INT PRIMARY KEY IDENTITY(1,1),
--     TickerSymbol NVARCHAR(10) NOT NULL UNIQUE,
--     CompanyName NVARCHAR(100) NOT NULL,
--     CurrentPrice DECIMAL(18,2) NOT NULL
-- );
-- CREATE TABLE UserPortfolio (
--     PortfolioId INT PRIMARY KEY IDENTITY(1,1),
--     UserId INT NOT NULL,
--     StockId INT NOT NULL,
--     SharesOwned INT NOT NULL CHECK (SharesOwned >= 0)
-- );
UPDATE UserAccounts 
SET Cash = 1000 
WHERE Cash = 100;




