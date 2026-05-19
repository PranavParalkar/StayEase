-- ============================================
-- StayEase Hotel Management System
-- SQL Server Database Script
-- ============================================

-- Create Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'StayEase')
BEGIN
    CREATE DATABASE StayEase;
END
GO

USE StayEase;
GO

-- ============================================
-- TABLE: ROLE (PHANQUYEN)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Role')
BEGIN
    CREATE TABLE [Role] (
        RoleID VARCHAR(20) PRIMARY KEY,
        RoleName NVARCHAR(50) NOT NULL
    );
END
GO

-- ============================================
-- TABLE: FEATURE (CHUCNANG)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Feature')
BEGIN
    CREATE TABLE Feature (
        FeatureID VARCHAR(20) PRIMARY KEY,
        FeatureName NVARCHAR(100) NOT NULL
    );
END
GO

-- ============================================
-- TABLE: ROLE_FEATURE (CHITIETCHUCNANG)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RoleFeature')
BEGIN
    CREATE TABLE RoleFeature (
        RoleID VARCHAR(20) NOT NULL,
        FeatureID VARCHAR(20) NOT NULL,
        PRIMARY KEY (RoleID, FeatureID),
        FOREIGN KEY (RoleID) REFERENCES [Role](RoleID),
        FOREIGN KEY (FeatureID) REFERENCES Feature(FeatureID)
    );
END
GO

-- ============================================
-- TABLE: EMPLOYEE (NHANVIEN)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Employee')
BEGIN
    CREATE TABLE Employee (
        EmployeeID VARCHAR(20) PRIMARY KEY,
        FullName NVARCHAR(50) NOT NULL,
        Gender SMALLINT DEFAULT 0,          -- 0 = Male, 1 = Female
        LeaveDays SMALLINT DEFAULT 0,
        Position SMALLINT DEFAULT 1,        -- 0 = Manager, 1 = Receptionist
        DateOfBirth DATE,
        HireDate DATE,
        Email VARCHAR(100),
        DailyWage INT DEFAULT 0,
        IsDeleted INT DEFAULT 0             -- 0 = Active, 1 = Soft-deleted
    );
END
GO

-- ============================================
-- TABLE: ACCOUNT (TAIKHOAN)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Account')
BEGIN
    CREATE TABLE Account (
        Username VARCHAR(20) PRIMARY KEY,
        EmployeeID VARCHAR(20) NOT NULL,
        Password VARCHAR(MAX) NOT NULL,
        Status INT DEFAULT 1,               -- 1 = Active
        RoleID VARCHAR(20) NOT NULL,
        IsDeleted INT DEFAULT 0,
        FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID),
        FOREIGN KEY (RoleID) REFERENCES [Role](RoleID)
    );
END
GO

-- ============================================
-- TABLE: ROOM (PHONG)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Room')
BEGIN
    CREATE TABLE Room (
        RoomID VARCHAR(20) PRIMARY KEY,
        RoomName NVARCHAR(20) NOT NULL,
        RoomType SMALLINT DEFAULT 1,        -- 0 = VIP, 1 = Standard
        Price INT DEFAULT 0,
        RoomDetail INT DEFAULT 0,           -- 0 = Single, 1 = Double, 2 = Family
        Status INT DEFAULT 0,               -- 0 = Available, 1 = Occupied, 2 = Not Cleaned, 3 = Under Repair
        Condition INT DEFAULT 0,            -- 0 = New, 1 = Old
        IsDeleted INT DEFAULT 0
    );
END
GO

-- ============================================
-- TABLE: AMENITY (TIENICH)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Amenity')
BEGIN
    CREATE TABLE Amenity (
        AmenityID VARCHAR(20) PRIMARY KEY,
        AmenityName NVARCHAR(30) NOT NULL,
        IsDeleted INT DEFAULT 0
    );
END
GO

-- ============================================
-- TABLE: ROOM_AMENITY (CHITIETTIENICH)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RoomAmenity')
BEGIN
    CREATE TABLE RoomAmenity (
        RoomID VARCHAR(20) NOT NULL,
        AmenityID VARCHAR(20) NOT NULL,
        Quantity INT DEFAULT 1,
        PRIMARY KEY (RoomID, AmenityID),
        FOREIGN KEY (RoomID) REFERENCES Room(RoomID),
        FOREIGN KEY (AmenityID) REFERENCES Amenity(AmenityID)
    );
END
GO

-- ============================================
-- TABLE: CUSTOMER (KHACHHANG)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Customer')
BEGIN
    CREATE TABLE Customer (
        CustomerID VARCHAR(20) PRIMARY KEY,
        FullName NVARCHAR(50) NOT NULL,
        IDCard VARCHAR(20),
        Gender SMALLINT DEFAULT 0,          -- 0 = Male, 1 = Female
        Phone VARCHAR(20),
        Address NVARCHAR(100),
        Nationality NVARCHAR(100) DEFAULT N'Indian',
        DateOfBirth DATE,
        IsDeleted INT DEFAULT 0
    );
END
GO

-- ============================================
-- TABLE: SERVICE (DICHVU)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Service')
BEGIN
    CREATE TABLE [Service] (
        ServiceID VARCHAR(20) PRIMARY KEY,
        ServiceName NVARCHAR(100) NOT NULL,
        ServiceType NVARCHAR(128),          -- Food & Beverage, Beauty, Entertainment, Party
        Price INT DEFAULT 0,
        Image NVARCHAR(MAX),                -- Base64-encoded image
        IsDeleted INT DEFAULT 0
    );
END
GO

-- ============================================
-- TABLE: BOOKING (CHITIETTHUE)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Booking')
BEGIN
    CREATE TABLE Booking (
        BookingID VARCHAR(20) PRIMARY KEY,
        CustomerID VARCHAR(20) NOT NULL,
        EmployeeID VARCHAR(20) NOT NULL,
        BookingDate DATETIME DEFAULT GETDATE(),
        Deposit INT DEFAULT 0,
        ProcessStatus INT DEFAULT 0,        -- 0 = Pending, 1 = Processed
        IsDeleted INT DEFAULT 0,
        FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
        FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID)
    );
END
GO

-- ============================================
-- TABLE: BOOKING_ROOM (CHITIETTHUEPHONG)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BookingRoom')
BEGIN
    CREATE TABLE BookingRoom (
        BookingID VARCHAR(20) NOT NULL,
        RoomID VARCHAR(20) NOT NULL,
        CheckInDate DATETIME NOT NULL,
        CheckOutDate DATETIME,
        ActualCheckOut DATETIME,
        RentalType INT DEFAULT 0,           -- 0 = By Day, 1 = By Hour, 2 = Flexible
        RentalPrice INT DEFAULT 0,
        Status INT DEFAULT 0,               -- 0 = Pending, 1 = Checked In, 2 = Checked Out
        PRIMARY KEY (BookingID, RoomID, CheckInDate),
        FOREIGN KEY (BookingID) REFERENCES Booking(BookingID),
        FOREIGN KEY (RoomID) REFERENCES Room(RoomID)
    );
END
GO

-- ============================================
-- TABLE: BOOKING_SERVICE (CHITIETTHUEDICHVU)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BookingService')
BEGIN
    CREATE TABLE BookingService (
        BookingID VARCHAR(20) NOT NULL,
        ServiceID VARCHAR(20) NOT NULL,
        UsageDate DATE NOT NULL,
        Quantity INT DEFAULT 1,
        Price INT DEFAULT 0,
        PRIMARY KEY (BookingID, ServiceID, UsageDate),
        FOREIGN KEY (BookingID) REFERENCES Booking(BookingID),
        FOREIGN KEY (ServiceID) REFERENCES [Service](ServiceID)
    );
END
GO

-- ============================================
-- TABLE: INVOICE (HOADON)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Invoice')
BEGIN
    CREATE TABLE Invoice (
        InvoiceID VARCHAR(20) PRIMARY KEY,
        BookingID VARCHAR(20) NOT NULL,
        Discount INT DEFAULT 0,             -- Discount percentage
        Surcharge INT DEFAULT 0,            -- Surcharge percentage
        PaymentDate DATETIME DEFAULT GETDATE(),
        PaymentMethod SMALLINT DEFAULT 0,   -- 0 = Cash, 1 = Bank Transfer
        IsDeleted INT DEFAULT 0,
        FOREIGN KEY (BookingID) REFERENCES Booking(BookingID)
    );
END
GO

-- ============================================
-- SEED DATA: Features (System Permissions)
-- ============================================
IF NOT EXISTS (SELECT * FROM Feature)
BEGIN
    INSERT INTO Feature (FeatureID, FeatureName) VALUES
    ('F001', N'Room Management'),
    ('F002', N'Service Management'),
    ('F003', N'Customer Management'),
    ('F004', N'Employee Management'),
    ('F005', N'Role Management'),
    ('F006', N'Booking Management'),
    ('F007', N'Invoice Management'),
    ('F008', N'View Statistics');
END
GO

-- ============================================
-- SEED DATA: Roles
-- ============================================
IF NOT EXISTS (SELECT * FROM [Role])
BEGIN
    INSERT INTO [Role] (RoleID, RoleName) VALUES
    ('R001', N'Admin'),
    ('R002', N'Receptionist');
END
GO

-- ============================================
-- SEED DATA: Role-Feature Mappings
-- Admin gets all features, Receptionist gets limited
-- ============================================
IF NOT EXISTS (SELECT * FROM RoleFeature)
BEGIN
    -- Admin: All features
    INSERT INTO RoleFeature (RoleID, FeatureID) VALUES
    ('R001', 'F001'), ('R001', 'F002'), ('R001', 'F003'), ('R001', 'F004'),
    ('R001', 'F005'), ('R001', 'F006'), ('R001', 'F007'), ('R001', 'F008');

    -- Receptionist: Room, Customer, Booking, Invoice
    INSERT INTO RoleFeature (RoleID, FeatureID) VALUES
    ('R002', 'F001'), ('R002', 'F003'), ('R002', 'F006'), ('R002', 'F007');
END
GO

-- ============================================
-- SEED DATA: Default Admin Employee & Account
-- Password: admin123 (SHA256 hashed)
-- ============================================
IF NOT EXISTS (SELECT * FROM Employee WHERE EmployeeID = 'NV190526001')
BEGIN
    INSERT INTO Employee (EmployeeID, FullName, Gender, LeaveDays, Position, DateOfBirth, HireDate, Email, DailyWage, IsDeleted)
    VALUES ('NV190526001', N'System Administrator', 0, 30, 0, '1990-01-01', '2024-01-01', 'admin@stayease.com', 5000, 0);
END
GO

IF NOT EXISTS (SELECT * FROM Account WHERE Username = 'admin')
BEGIN
    INSERT INTO Account (Username, EmployeeID, Password, Status, RoleID, IsDeleted)
    VALUES ('admin', 'NV190526001', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 1, 'R001', 0);
END
GO

-- ============================================
-- SEED DATA: Sample Amenities
-- ============================================
IF NOT EXISTS (SELECT * FROM Amenity)
BEGIN
    INSERT INTO Amenity (AmenityID, AmenityName, IsDeleted) VALUES
    ('TI190526001', N'Television', 0),
    ('TI190526002', N'Air Conditioner', 0),
    ('TI190526003', N'Hair Dryer', 0),
    ('TI190526004', N'Iron', 0),
    ('TI190526005', N'Mini Fridge', 0),
    ('TI190526006', N'Safe Box', 0),
    ('TI190526007', N'WiFi Router', 0),
    ('TI190526008', N'Electric Kettle', 0);
END
GO

-- ============================================
-- SEED DATA: Sample Rooms
-- ============================================
IF NOT EXISTS (SELECT * FROM Room)
BEGIN
    INSERT INTO Room (RoomID, RoomName, RoomType, Price, RoomDetail, Status, Condition, IsDeleted) VALUES
    ('P190526001', N'Room 101', 1, 1500, 0, 0, 0, 0),
    ('P190526002', N'Room 102', 1, 1500, 0, 0, 0, 0),
    ('P190526003', N'Room 201', 1, 2000, 1, 0, 0, 0),
    ('P190526004', N'Room 202', 0, 3500, 1, 0, 0, 0),
    ('P190526005', N'Room 301', 0, 5000, 2, 0, 0, 0),
    ('P190526006', N'Room 302', 0, 5000, 2, 0, 0, 0);
END
GO

-- ============================================
-- SEED DATA: Sample Services
-- ============================================
IF NOT EXISTS (SELECT * FROM [Service])
BEGIN
    INSERT INTO [Service] (ServiceID, ServiceName, ServiceType, Price, Image, IsDeleted) VALUES
    ('DV190526001', N'Breakfast Buffet', N'Food & Beverage', 500, NULL, 0),
    ('DV190526002', N'Spa Massage', N'Beauty Care', 1200, NULL, 0),
    ('DV190526003', N'Airport Pickup', N'Entertainment', 800, NULL, 0),
    ('DV190526004', N'Laundry Service', N'Party Services', 300, NULL, 0),
    ('DV190526005', N'Room Service Dining', N'Food & Beverage', 700, NULL, 0);
END
GO

PRINT '=== StayEase Database Created Successfully ===';
GO
