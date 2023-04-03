use CafeWebsite
go

create table Product
(
	ProductId int identity(1,1) primary key,
	Name nvarchar(255),
	Price float,
	Image nvarchar(255),
	Summary nvarchar(255),
	Description nvarchar(max),
	Quantity int,
	CreatedDate datetime,
	UpdatedDate datetime,
	CreatedBy nvarchar(50),
	UpdatedBy nvarchar(50)
)

create table ProductCategory
(
	ProductCategoryId int identity(1,1) primary key,
	Name nvarchar(255),
	CreatedDate datetime,
	UpdatedDate datetime,
	CreatedBy nvarchar(50),
	UpdatedBy nvarchar(50)
)

create table ProductAndCategory
(
	ProductId int foreign key references Product(ProductId),
	ProductCategoryId int foreign key references ProductCategory(ProductCategoryId)
)

create table Role 
(
	RoleId int identity(1,1) primary key,
	Name nvarchar(50)
)

create table Account 
(
	AccountId int identity(1,1) primary key,
	Username varchar(50),
	DisplayName nvarchar(255),
	Password varchar(50),
	FirstName nvarchar(50),
	LastName nvarchar(50),
	Address nvarchar(255),
	Phone varchar(20),
	Email varchar(255),
	Introduction nvarchar(max),
	Role varchar(255),
	Image nvarchar(255)
)

alter table Account
drop column Role

alter table Account
add RoleId int foreign key references Role(RoleId)

create table News 
(
	NewsId int identity(1,1) primary key,
	Title nvarchar(255),
	Summary nvarchar(255),
	Content nvarchar(max),
	Comments int,
	IsIntroduction bit,
	CreatedDate datetime,
	UpdatedDate datetime,
	CreatedBy nvarchar(50),
	UpdatedBy nvarchar(50)
)

alter table News
add AccountId int foreign key references Account(AccountId)

alter table News
drop column Comments

alter table News 
drop column CreatedBy

alter table News 
add Image nvarchar(200) 

create table NewsCategory
(
	NewsCategoryId int identity(1,1) primary key,
	Name nvarchar(50),
	NumberOfPosts int,
	CreatedDate datetime,
	UpdatedDate datetime,
	CreatedBy nvarchar(50),
	UpdatedBy nvarchar(50)
)


create table NewsAndCategory
(
	NewsId int foreign key references News(NewsId),
	NewsCategoryId int foreign key references NewsCategory(NewsCategoryId)
)

create table NewsTag
(
	NewsTagId int identity(1,1) primary key,
	Name nvarchar(20),
	CreatedDate datetime,
	UpdatedDate datetime,
	CreatedBy nvarchar(50),
	UpdatedBy nvarchar(50)
)

create table NewsAndTag 
(
	NewsId int foreign key references News(NewsId),
	NewsTagId int foreign key references NewsTag(NewsTagId)
)

create table Gallery 
(
	GalleryId int identity(1,1) primary key,
	Path nvarchar(255),
	Format varchar(20),
	AccountId int foreign key references Account(AccountId)
)

create table TableShop 
(
	TableId int identity(1,1) primary key,
	Slots int,
	Status varchar(20),
	BookTime datetime
)

alter table TableShop 
drop column BookTime

create table BookTime 
(
	BookTimeId int identity (1,1) primary key,
	StartTime datetime
)

drop table BookTime 

create table BookTime 
(
	BookTimeId int primary key,
	StartTime datetime
)

alter table BookTime
drop column StartTime

create table TableAndBookTime
(
	TableId int foreign key references TableShop(TableId),
	BookTimeId int foreign key references BookTime(BookTimeId)
)

drop table TableAndBookTime

create table TableAndBookTime
(
	TableId int foreign key references TableShop(TableId),
	BookTimeId int foreign key references BookTime(BookTimeId)
)

drop table TableAndBookTime

alter table TableShop 
add BookTimeId int foreign key references BookTime(BookTimeId)

create table TableShopAndAccount
(
	TableId int foreign key references TableShop(TableId),
	AccountId int foreign key references Account(AccountId)
)

create table Staff 
(
	StaffId int identity(1,1) primary key,
	FirstName nvarchar(50),
	LastName nvarchar(50),
	DateOfBirth datetime,
	Address nvarchar(255),
	Phone varchar(20)
)

create table SalesReceipt
(
	SalesReceiptId int identity(1,1) primary key,
	CreatedDate datetime,
	IsDelivered bit,
	ShippingFee float,
	StaffId int foreign key references Staff(StaffId)
)


alter table SalesReceipt
add AccountId int foreign key references Account(AccountId)

create table DiscountCode
(
	DiscountCodeId int identity(1,1) primary key,
	DecreaseAmount float,
	MinimumToApply float,
	Name varchar(20),
	CreatedBy nvarchar(255),
	CreatedDate datetime,
	ActiveDate datetime,
	ExpireDate datetime
)

create table SaleReceiptAndDiscount
(
	SalesReceiptId int foreign key references SalesReceipt(SalesReceiptId),
	DiscountCodeId int foreign key references DiscountCode(DiscountCodeId)
)

create table Customer
(
	CustomerId int identity(1,1) primary key,
	Name nvarchar(255),
	Phone varchar(20),
	CreatedDate datetime,
	UpdatedDate datetime,
	CreatedBy nvarchar(50),
	UpdatedBy nvarchar(50)
)

create table Cart 
(
	CartId int identity(1,1) primary key,
	Quantity int,
	ProductId int foreign key references Product(ProductId),
	AccountId int foreign key references Account(AccountId),
	CustomerId int foreign key references Customer(CustomerId),
	SalesReceiptId int foreign key references SalesReceipt(SalesReceiptId)
)

alter table Cart
add IsPlaced bit

create table Banner 
(
	BannerId int identity(1,1) primary key,
	Title nvarchar(50),
	Content nvarchar(255),
	Image nvarchar(255),
	CreatedDate datetime,
	UpdatedDate datetime,
	CreatedBy nvarchar(50),
	UpdatedBy nvarchar(50)
)

create table ContactMail
(
	ContactMailId int identity(1,1) primary key,
	FirstName nvarchar(255),
	LastName nvarchar(255),
	Email varchar(255),
	Phone varchar(20),
	Content nvarchar(max)
)

create table ProviderSide
(
	ProviderId int identity(1,1) primary key,
	Name nvarchar(255)
)

create table ImportReceipt
(
	ImportReceiptId int identity(1,1) primary key,
	ImportDate datetime,
	StaffId int foreign key references Staff(StaffId),
	ProviderId int foreign key references ProviderSide(ProviderId)
)

create table ImportedGood
(
	ImportedGoodId int identity(1,1) primary key,
	Name nvarchar(50)
)

create table ImportReceiptDetail
(
	ImportReceiptDetailId int identity(1,1) primary key,
	Quantity int,
	ImportReceiptId int foreign key references ImportReceipt(ImportReceiptId),
	ImportedGoodId int foreign key references ImportedGood(ImportedGoodId)
)

drop table ImportedGood
drop table ImportReceiptDetail
drop table ProviderSide
drop table ImportReceipt
drop table ContactMail

insert into Role values(N'Admin')

insert into Account values('admin', N'Admin', 'admin', N'Át', N'Min', null, null, null, null, null, 1)

insert into BookTime values(-1)
insert into BookTime values(8)
insert into BookTime values(10)
insert into BookTime values(12)
insert into BookTime values(14)
insert into BookTime values(16)
insert into BookTime values(18)
insert into BookTime values(20)

