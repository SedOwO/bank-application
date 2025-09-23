# Bank API  

### Database

#### Tables

**1. Bank**
```
   Column   |            Type             | Collation | Nullable |           Default
------------+-----------------------------+-----------+----------+------------------------------
 bankid     | integer                     |           | not null | generated always as identity
 bankname   | character varying(100)      |           | not null |
 branchname | character varying(100)      |           |          |
 ifsccode   | character varying(20)       |           | not null |
 address    | character varying(200)      |           |          |
 createdat  | timestamp without time zone |           |          | CURRENT_TIMESTAMP
```

**2. Customer**
```
   Column   |            Type             | Collation | Nullable |           Default
------------+-----------------------------+-----------+----------+------------------------------
 customerid | integer                     |           | not null | generated always as identity
 firstname  | character varying(50)       |           | not null |
 lastname   | character varying(50)       |           |          |
 dob        | date                        |           |          |
 email      | character varying(100)      |           |          |
 phone      | character varying(15)       |           |          |
 address    | character varying(200)      |           |          |
 createdat  | timestamp without time zone |           |          | CURRENT_TIMESTAMP
```

**3. Accounts**

```
    Column     |            Type             | Collation | Nullable |           Default
---------------+-----------------------------+-----------+----------+------------------------------
 accountid     | integer                     |           | not null | generated always as identity
 customerid    | integer                     |           | not null |
 bankid        | integer                     |           | not null |
 accountnumber | character varying(20)       |           | not null |
 accounttype   | character varying(20)       |           |          |
 balance       | numeric(15,2)               |           |          | 0.00
 createdat     | timestamp without time zone |           |          | CURRENT_TIMESTAMP
```


#### Functions  
##### Bank Functions
```sql
-- Create
CREATE OR REPLACE FUNCTION CreateBank(
    p_BankName VARCHAR,
    p_BranchName VARCHAR,
    p_IFSCCode VARCHAR,
    p_Address VARCHAR
)
RETURNS INT 
AS $$
DECLARE
    newBankID INT;
BEGIN
    INSERT INTO Bank (BankName, BranchName, IFSCCode, Address)
    VALUES (p_BankName, p_BranchName, p_IFSCCode, p_Address)
    RETURNING BankID INTO newBankID;

    RETURN newBankID;
END;
$$ 
LANGUAGE plpgsql;
```

```sql
-- Read
CREATE OR REPLACE FUNCTION GetBankByID(p_BankID INT)
RETURNS TABLE (
    BankID INT,
    BankName VARCHAR,
    BranchName VARCHAR,
    IFSCCode VARCHAR,
    Address VARCHAR,
    CreatedAt TIMESTAMP
) 
AS $$
BEGIN
    RETURN QUERY
    SELECT BankID, BankName, BranchName, IFSCCode, Address, CreatedAt
    FROM Bank
    WHERE BankID = p_BankID;
END;
$$ 
LANGUAGE plpgsql;
```

```sql
-- Update
CREATE OR REPLACE FUNCTION UpdateBank(
    p_BankID INT,
    p_BankName VARCHAR,
    p_BranchName VARCHAR,
    p_IFSCCode VARCHAR,
    p_Address VARCHAR
)
RETURNS VOID 
AS $$
BEGIN
    UPDATE Bank
    SET BankName  = p_BankName,
        BranchName = p_BranchName,
        IFSCCode  = p_IFSCCode,
        Address   = p_Address
    WHERE BankID = p_BankID;
END;
$$ 
LANGUAGE plpgsql;
```
```sql
-- Delete
CREATE OR REPLACE FUNCTION DeleteBank(p_BankId INT)
RETURNS BOOLEAN AS $$
BEGIN
    IF EXISTS (SELECT 1 FROM Bank WHERE BankId = p_BankId) THEN
        DELETE FROM Bank WHERE BankId = p_BankId;
        RETURN TRUE;
    ELSE
        RETURN FALSE;
    END IF;
END;
$$ LANGUAGE plpgsql;
```

##### Customer
```sql
-- Create
CREATE OR REPLACE FUNCTION CreateCustomer(
    p_FirstName VARCHAR,
    p_LastName VARCHAR,
    p_DOB DATE,
    p_Email VARCHAR,
    p_Phone VARCHAR,
    p_Address VARCHAR
)
RETURNS INT 
AS $$
DECLARE
    newCustomerID INT;
BEGIN
    INSERT INTO Customer (FirstName, LastName, DOB, Email, Phone, Address)
    VALUES (p_FirstName, p_LastName, p_DOB, p_Email, p_Phone, p_Address)
    RETURNING CustomerID INTO newCustomerID;

    RETURN newCustomerID;
END;
$$ 
LANGUAGE plpgsql;
```

```sql
-- Read
CREATE OR REPLACE FUNCTION GetCustomerByID(p_CustomerID INT)
RETURNS TABLE (
    CustomerID INT,
    FirstName VARCHAR,
    LastName VARCHAR,
    DOB DATE,
    Email VARCHAR,
    Phone VARCHAR,
    Address VARCHAR,
    CreatedAt TIMESTAMP
) 
AS $$
BEGIN
    RETURN QUERY
    SELECT CustomerID, FirstName, LastName, DOB, Email, Phone, Address, CreatedAt
    FROM Customer
    WHERE CustomerID = p_CustomerID;
END;
$$ 
LANGUAGE plpgsql;
```

```sql
-- Update
CREATE OR REPLACE FUNCTION UpdateCustomer(
    p_CustomerID INT,
    p_FirstName VARCHAR,
    p_LastName VARCHAR,
    p_DOB DATE,
    p_Email VARCHAR,
    p_Phone VARCHAR,
    p_Address VARCHAR
)
RETURNS VOID 
AS $$
BEGIN
    UPDATE Customer
    SET FirstName = p_FirstName,
        LastName  = p_LastName,
        DOB       = p_DOB,
        Email     = p_Email,
        Phone     = p_Phone,
        Address   = p_Address
    WHERE CustomerID = p_CustomerID;
END;
$$ 
LANGUAGE plpgsql;
```

```sql
-- Delete
CREATE OR REPLACE FUNCTION DeleteCustomer(p_CustomerID INT)
RETURNS VOID 
AS $$
BEGIN
    DELETE FROM Bank WHERE CustomerID = p_CustomerID;
END;
$$ 
LANGUAGE plpgsql;
```

##### Account
```sql
-- Create
CREATE OR REPLACE FUNCTION CreateAccount(
    p_CustomerID INT,
    p_BankID INT,
    p_AccountType VARCHAR,
    p_InitialBalance DECIMAL
)
RETURNS INT AS $$
DECLARE
    newAccountID INT;
BEGIN
    INSERT INTO Account (CustomerID, BankID, AccountNumber, AccountType, Balance)
    VALUES (
        p_CustomerID,
        p_BankID,
        CONCAT('ACC', EXTRACT(EPOCH FROM NOW())::BIGINT), 
        p_AccountType,
        p_InitialBalance
    )
    RETURNING AccountID INTO newAccountID;

    RETURN newAccountID;
END;
$$ 
LANGUAGE plpgsql;
```


```sql
-- Read
CREATE OR REPLACE FUNCTION GetAccountByID(p_AccountID INT)
RETURNS TABLE (
    AccountID INT,
    CustomerID INT,
    BankID INT,
    AccountNumber VARCHAR,
    AccountType VARCHAR,
    Balance DECIMAL,
    CreatedAt TIMESTAMP
) 
AS $$
BEGIN
    RETURN QUERY
    SELECT AccountID, CustomerID, BankID, AccountNumber, AccountType, Balance, CreatedAt
    FROM Account
    WHERE AccountID = p_AccountID;
END;
$$ 
LANGUAGE plpgsql;
```


```sql
-- Update
CREATE OR REPLACE FUNCTION UpdateAccount(
    p_AccountID INT,
    p_AccountType VARCHAR,
    p_Balance DECIMAL
)
RETURNS VOID 
AS $$
BEGIN
    UPDATE Account
    SET AccountType = p_AccountType,
        Balance = p_Balance
    WHERE AccountID = p_AccountID;
END;
$$ 
LANGUAGE plpgsql;
```


```sql
-- Delete
CREATE OR REPLACE FUNCTION DeleteAccount(p_AccountID INT)
RETURNS VOID 
AS $$
BEGIN
    DELETE FROM Account WHERE AccountID = p_AccountID;
END;
$$ 
LANGUAGE plpgsql;
```




-------------------------------------------------------------
### WORK IN PROGRESS
#### API Design
#### Bank
GET: /api/banks                 DONE  
GET: /api/banks/{id}            DONE  
POST: /api/banks                DONE  
PUT: /api/banks/{id}  
DELETE: /api/banks/{id}  


#### Customer
GET: /api/customers  
GET: /api/customers/{id}  
POST: /api/customers  
PUT: /api/customers/{id}  
DELETE: /api/customers/{id}  


#### Account
GET: /api/accounts/  
GET: /api/accounts/{id}  
\*GET: /api/customers/{customer\_id}/accounts
POST: /api/accounts  
PUT: /api/accounts/{id}  
DELETE: /api/accounts{id}