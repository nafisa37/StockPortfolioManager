USE StockPortfolio;
GO
-- INSERT INTO Users (Username, Email, PasswordHash)
-- VALUES ('testuser', 'test@example.com', 'hashedpassword123');
-- SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserAccounts';

-- ALTER TABLE UserAccounts
-- ADD Cash DECIMAL(18, 2) DEFAULT 1000;

-- UPDATE UserAccounts
-- SET Cash = 1000
-- WHERE Cash IS NULL;
-- CREATE TABLE UserAccounts (
--     Id INT IDENTITY(1,1) PRIMARY KEY,       -- Primary Key with auto-incrementing Id
--     Username NVARCHAR(20) NOT NULL,          -- Username with max 20 characters
--     Email NVARCHAR(100) NOT NULL,            -- Email with max 100 characters
--     Password NVARCHAR(20) NOT NULL,          -- Password with max 20 characters
--     Cash DECIMAL(18,2) DEFAULT 0.00          -- Cash with two decimal places and default value 0.00
-- );

DELETE FROM Stocks WHERE TickerSymbol IN ('apple', 'msft');







