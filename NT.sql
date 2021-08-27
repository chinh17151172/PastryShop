CREATE DATABASE PASTRY_SHOP
GO


USE PASTRY_SHOP
GO

-- Food
-- Table
-- FoodCategory
-- Account
-- Bill
-- Bill Information

CREATE TABLE TableFood
(
  id INT IDENTITY PRIMARY KEY,
  name_table NVARCHAR(100) NOT NULL DEFAULT N'Bàn chưa có tên',
  status_table NVARCHAR(100) NOT NULL DEFAULT N'Trống'  -- bàn trống || bàn có người
 )

 CREATE TABLE Account
(
   	UserName NVARCHAR(100) PRIMARY KEY,
	DisplayName NVARCHAR(100) NOT NULL DEFAULT N'Như Thanh',
	Password NVARCHAR(1000) NOT NULL DEFAULT 0,
	Type INT NOT NULL DEFAULT 0   -- 1: Admin && 0: Staff
)
GO

CREATE TABLE FoodCategory
(
   id INT IDENTITY PRIMARY KEY,
  name_food NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên'
)
GO

CREATE TABLE Food
(
  id INT IDENTITY PRIMARY KEY,
  name_food NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên',
  idCategory INT NOT NULL,
  price FLOAT NOT NULL DEFAULT 0

  FOREIGN KEY (idCategory) REFERENCES dbo.FoodCategory(id)
)
GO

CREATE TABLE Bill
(
    id INT IDENTITY PRIMARY KEY,
	DateCheckIn DATE NOT NULL DEFAULT GETDATE(),
	DateCheckOut DATE ,
	idTable INT NOT NULL,
	status INT NOT NULL    -- 1 : đã thanh toán, 0 : chưa thanh toán

	FOREIGN KEY (idTable) REFERENCES dbo.TableFood(id)
)
GO

CREATE TABLE BillInfo
(
    id INT IDENTITY PRIMARY KEY,
	idBill INT NOT NULL,
	idFood INT NOT NULL,
	count INT NOT NULL DEFAULT 0
	 
	FOREIGN KEY (idBill) REFERENCES dbo.Bill(id),
	FOREIGN KEY (idFood) REFERENCES dbo.Food(id)
)
GO


SELECT f.name_food,bi.count,f.price,f.price*bi.count AS TotalPrice FROM dbo.BillInfo AS bi, dbo.Bill AS b,dbo.Food AS f
WHERE bi.idBill = b.id AND bi.idFood = f.id AND b.idTable = 5

SELECT * FROM dbo.Bill
SELECT * FROM dbo.BillInfo
--SELECT * FROM dbo.Food
SELECT * FROM  dbo.TableFood
GO

CREATE PROC USP_GetAccountByUserName -- Bảo mật thông tin
@userName NVARCHAR(100)
AS
BEGIN
 SELECT * FROM dbo.Account WHERE UserName = @userName
END
GO

EXEC dbo.USP_GetAccountByUserName @userName = N'NhuThanh' -- nvarchar(100)


CREATE PROC USP_LogIn
@userName NVARCHAR(100), @password NVARCHAR(100) 
AS
BEGIN
  SELECT * FROM dbo.Account WHERE UserName = @userName AND Password = @password
END
GO

 CREATE PROC USP_GetTableList
 AS SELECT * FROM dbo.TableFood 
 GO

 EXEC dbo.USP_GetTableList

 SELECT f.name_food,bi.count,f.price,f.price*bi.count AS TotalPrice FROM dbo.BillInfo AS bi, dbo.Bill AS b,dbo.Food AS f WHERE bi.idBill = b.id AND bi.idFood = f.id AND b.status = 0 AND b.idTable = 5

 SELECT * FROM dbo.Food
 Select * from Food where idCategory = 1
 go

 CREATE PROC USP_InsertBill
 @idTable INT   
 AS
 BEGIN
    INSERT dbo.Bill
    (
        DateCheckIn,
        DateCheckOut,
        idTable,
        status,
		discount
    )
    VALUES
    (   GETDATE(), -- DateCheckIn - date
        NULL,    -- DateCheckOut - date
        @idTable,       -- idTable - int
        0,        -- status - int
		0
        )
 END
 GO

ALTER PROC USP_InsertBillInfo
 @idBill INT , @idFood INT ,@count INT
 AS
 BEGIN
      
	  DECLARE @isExitsBillInfo INT;
	  DECLARE @foodCount INT = 1

	  SELECT  @isExitsBillInfo = id, @foodCount = b.count FROM dbo.BillInfo AS b WHERE idBill= @idBill AND idFood = @idFood
	  
	  IF (@isExitsBillInfo > 0 )
	  BEGIN
	     DECLARE @newcount INT = @foodCount + @count
		 IF (@newcount > 0)
	        UPDATE dbo.BillInfo SET count = @foodCount + @count  WHERE idFood =@idFood
		 ELSE	
		    DELETE dbo.BillInfo WHERE idBill = @idBill AND idFood = @idFood
	  END
	  ELSE
	  BEGIN
	    INSERT dbo.BillInfo
        ( idBill,idFood,count)
         VALUES
       (   @idBill,      -- idBill - int
          @idFood,      -- idFood - int
          @count -- count - int
          )
      END
 
 END
 GO
 




 CREATE TRIGGER UTG_UpdateBillInfo
 ON dbo.BillInfo FOR INSERT, UPDATE
 AS
 BEGIN
    DECLARE @idBill INT
	SELECT  @idBill = idBill FROM Inserted

	DECLARE @idTable INT
	SELECT @idTable = idTable FROM dbo.Bill WHERE id= @idBill AND status = 0

	UPDATE dbo.TableFood SET status_table = N'Có người' WHERE id = @idTable

 END 
 GO

 DELETE dbo.BillInfo
 DELETE dbo.Bill	



 CREATE  TRIGGER UTG_UpdateBill
 ON dbo.Bill FOR UPDATE
 AS
 BEGIN
		DECLARE @idBill INT 
		SELECT @idBill = id FROM Inserted

		DECLARE @idTable INT
		SELECT @idTable = idTable FROM dbo.Bill WHERE id=@idBill 

		DECLARE @count INT = 0
		SELECT @count = COUNT(*) FROM dbo.Bill WHERE idTable = @idTable AND status = 0

		IF (@count = 0)
		  UPDATE dbo.TableFood SET status_table = N'Trống'	 WHERE id = @idTable
 END
 GO
 
ALTER TABLE dbo.Bill
ADD discount INT 


ALTER TABLE dbo.Bill ADD totalPrice FLOAT

DELETE dbo.BillInfo
DELETE dbo.Bill
GO

CREATE PROC USP_GetListBillByDate
@checkIn DATE, @checkOut DATE
AS
BEGIN
	SELECT t.name_table AS [Tên bàn], b.totalPrice AS [Tổng tiền], DateCheckIn AS [Ngày vào], DateCheckOut AS [Ngày ra], discount AS [Giảm giá] 
	FROM dbo.Bill AS b, dbo.TableFood AS t
	WHERE DateCheckIn >= @checkIn  AND DateCheckOut <= @checkOut AND b.status =	1 
	AND t.id = b.idTable 
END 
GO

CREATE PROC USP_UpdateAccount
@userName NVARCHAR(100), @displayName NVARCHAR(100), @password NVARCHAR(100), @newPassword NVARCHAR(100)
AS
BEGIN
		DECLARE @isRightPass INT = 0
		SELECT @isRightPass = COUNT(*) FROM dbo.Account WHERE UserName = @userName AND Password = @password 

		IF(@isRightPass = 1)
		BEGIN
			IF (@newPassword = NULL OR @newPassword = '' )
			BEGIN
			  UPDATE dbo.Account SET DisplayName  = @displayName WHERE UserName = @userName
            END
			ELSE
               UPDATE dbo.Account SET DisplayName  = @displayName, Password = @newPassword WHERE UserName = @userName 
		END
END
GO

CREATE  TRIGGER UTG_DeleteBillInfo
ON dbo.BillInfo FOR DELETE
AS
BEGIN
		DECLARE @idBillInfo INT
		DECLARE @idBill INT
		SELECT @idBillInfo = id, @idBill  = Deleted.idBill FROM Deleted

		DECLARE @idTable INT 
		SELECT @idTable  = idTable FROM dbo.Bill WHERE id = @idBill

		DECLARE @count INT  =0 
		SELECT @count = COUNT(*) FROM dbo.BillInfo AS bi,dbo.Bill AS b WHERE b.id = bi.idBill AND bi.id = @idBill AND b.status = 0
		
		IF(@count = 0)
			UPDATE dbo.TableFood SET status_table = N'Trống' WHERE id = @idTable

END
GO


CREATE FUNCTION [dbo].[fuConvertToUnsign1] ( @strInput NVARCHAR(4000) ) RETURNS NVARCHAR(4000) AS BEGIN IF @strInput IS NULL RETURN @strInput IF @strInput = '' RETURN @strInput DECLARE @RT NVARCHAR(4000) DECLARE @SIGN_CHARS NCHAR(136) DECLARE @UNSIGN_CHARS NCHAR (136) SET @SIGN_CHARS = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệế ìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵý ĂÂĐÊÔƠƯÀẢÃẠÁẰẲẴẶẮẦẨẪẬẤÈẺẼẸÉỀỂỄỆẾÌỈĨỊÍ ÒỎÕỌÓỒỔỖỘỐỜỞỠỢỚÙỦŨỤÚỪỬỮỰỨỲỶỸỴÝ' +NCHAR(272)+ NCHAR(208) SET @UNSIGN_CHARS = N'aadeoouaaaaaaaaaaaaaaaeeeeeeeeee iiiiiooooooooooooooouuuuuuuuuuyyyyy AADEOOUAAAAAAAAAAAAAAAEEEEEEEEEEIIIII OOOOOOOOOOOOOOOUUUUUUUUUUYYYYYDD' DECLARE @COUNTER int DECLARE @COUNTER1 int SET @COUNTER = 1 WHILE (@COUNTER <=LEN(@strInput)) BEGIN SET @COUNTER1 = 1 WHILE (@COUNTER1 <=LEN(@SIGN_CHARS)+1) BEGIN IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1,1)) = UNICODE(SUBSTRING(@strInput,@COUNTER ,1) ) BEGIN IF @COUNTER=1 SET @strInput = SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)-1) ELSE SET @strInput = SUBSTRING(@strInput, 1, @COUNTER-1) +SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)- @COUNTER) BREAK END SET @COUNTER1 = @COUNTER1 +1 END SET @COUNTER = @COUNTER +1 END SET @strInput = replace(@strInput,' ','-') RETURN @strInput END

GO

SELECT * FROM dbo.Food WHERE dbo.fuConvertToUnsign1(name_food) LIKE N'%' + dbo.fuConvertToUnsign1(N'che') + '%'
GO

SELECT * FROM dbo.Account