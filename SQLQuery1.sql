CREATE TABLE Requests (
    RequestId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    Message NVARCHAR(200),
    RequestType INT,
    
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
