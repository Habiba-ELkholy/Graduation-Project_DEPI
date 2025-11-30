-- «·ŒÿÊ… 1: ≈‰‘«¡ ﬁ«⁄œ… «·»Ì«‰« 
CREATE DATABASE c2cUniversiteesDB;
GO

-- «·ŒÿÊ… 2: «· »œÌ· ·«” Œœ«„Â«
USE c2cUniversiteesDB;
GO

-- «·ŒÿÊ… 3: ≈‰‘«¡ ÃœÊ· «·„” Œœ„Ì‰
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    [Password] NVARCHAR(128) NOT NULL, 
    CollegeName NVARCHAR(50) NOT NULL,
    ResetToken NVARCHAR(50) NULL,
    TokenExpiry DATETIME2 NULL 
);
GO

-- «·ŒÿÊ… 4: ≈‰‘«¡ ÃœÊ· «·„‰ Ã« 
CREATE TABLE Products (
    ProductId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(10, 2) NOT NULL,
    Category NVARCHAR(50),
    ImagePath NVARCHAR(255),
    IsSold BIT NOT NULL DEFAULT 0,
    PostedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    SellerId INT NOT NULL, 
    FOREIGN KEY (SellerId) REFERENCES Users(UserId)
);
GO