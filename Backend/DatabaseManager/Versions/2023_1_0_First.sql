------------------------------------DATABASE TABLES----------------------------------
------------------------------------Users------------------------------
if not exists (select * from sysobjects where name='Users')
   create table Users (Id int identity (1,1), Firstname nvarchar (100), Lastname nvarchar (100),Birthdate bigint, Email nvarchar (100) UNIQUE, HashedPassword nvarchar (150),CreatedOn bigint, PhotoFilename nvarchar(500), PRIMARY KEY (Id))

------------------------------------UserRefreshTokens------------------------------
if not exists (select * from sysobjects where name='UserRefreshTokens')
   create table UserRefreshTokens (UserId int, RefreshToken nvarchar (1000),TokenCreated DATETIME, TokenExpires DATETIME, PRIMARY KEY (UserId),
   FOREIGN KEY (UserId) REFERENCES Users(Id))

------------------------------------Meetups------------------------------
if not exists (select * from sysobjects where name='Meetups')
   create table Meetups (Id int identity (1,1),TypeId int, MeetingTime bigint, YCoordinate nvarchar (100), XCoordinate nvarchar (100), Description nvarchar (500),AvailableSlots int, Level int, CreatedOn bigint, CreatedBy int, PRIMARY KEY (Id),
   FOREIGN KEY (CreatedBy) REFERENCES Users(Id))

   ------------------------------------UserMeetup------------------------------
if not exists (select * from sysobjects where name='UserMeetups')
   create table UserMeetups (Id int identity (1,1), UserId int, MeetupId int, PRIMARY KEY (Id),
   FOREIGN KEY (UserId) REFERENCES Users(Id),
   FOREIGN KEY (MeetupId) REFERENCES Meetups(Id))

------------------------------------STORED PROCEDURES------------------------------
------------------------------------GetUserByEmail_SP------------------------------

GO 
create or alter procedure GetUserByEmail_SP
	@Email nvarchar(100)
	as
	begin
	select * from Users where Email = @email
	end

------------------------------------AddUser_SP------------------------------
GO
create or alter procedure AddUser_SP
	@Firstname nvarchar(100),
	@Lastname nvarchar(100),
	@Birthdate bigint,
	@Email nvarchar(100),
	@HashedPassword nvarchar(100),
	@CreatedOn bigint,
	@PhotoFileName nvarchar(500)
	as
	begin
	INSERT INTO [dbo].[Users]
           ([Firstname]
           ,[Lastname]
		   ,[Birthdate]
           ,[Email]
           ,[HashedPassword]
           ,[CreatedOn]
           ,[PhotoFilename])
	 OUTPUT Inserted.Id
     VALUES
           (@Firstname,@Lastname,@Birthdate,@Email,@HashedPassword,@CreatedOn,@PhotoFileName)
	end

------------------------------------GetRefreshTokenForUser_SP------------------------------
GO 
create or alter procedure GetRefreshTokenForUser_SP
	@Id int
	as
	begin
	select * from UserRefreshTokens where UserId = @Id
	end
	
------------------------------------AddOrUpdateRefreshToken_SP------------------------------
GO
create or alter procedure AddOrUpdateRefreshToken_SP
	@Id int,
	@RefreshToken nvarchar(1000),
	@TokenCreated DATETIME,
	@TokenExpires DATETIME
	as
	begin
	IF EXISTS ( Select 1 from [dbo].[UserRefreshTokens] where UserId = @Id)
		begin
		update [dbo].[UserRefreshTokens] set UserId = @Id, RefreshToken = @RefreshToken, TokenCreated = @TokenCreated, TokenExpires = @TokenExpires Where UserId = @Id
		end
	Else
		begin
	INSERT INTO [dbo].[UserRefreshTokens]
		(UserId, RefreshToken, TokenCreated, TokenExpires) VALUES(@Id, @RefreshToken, @TokenCreated, @TokenExpires)
		end
	end
	
------------------------------------GetAllMeetups_SP------------------------------
GO 
create or alter procedure GetAllMeetups_SP
	as
	begin
	select * from Meetups left join Users on Meetups.CreatedBy = Users.Id
	end
	
	
------------------------------------GetMeetupById_SP------------------------------
GO 
create or alter procedure GetMeetupById_SP
	@Id int
	as
	begin
	select * from Meetups left join Users on Meetups.CreatedBy = Users.Id where Meetups.Id = @Id
	end
	

------------------------------------CreateMeetup_SP------------------------------
GO
create or alter procedure CreateMeetup_SP
	@TypeId int,
	@MeetingTime nvarchar(1000),
	@YCoordinate nvarchar(100),
	@XCoordinate nvarchar(100),
	@Description nvarchar(500),
	@AvailableSlots int,
	@Level int,
	@CreatedOn bigint,
	@CreatedBy int
	as
	begin
	INSERT INTO [dbo].[Meetups]
           ([TypeId]
           ,[MeetingTime]
           ,[YCoordinate]
           ,[XCoordinate]
           ,[Description]
           ,[AvailableSlots]
           ,[Level]
           ,[CreatedOn]
           ,[CreatedBy])
     VALUES
           (@TypeId,@MeetingTime,@YCoordinate,@XCoordinate,@Description,@AvailableSlots,@Level,@CreatedOn,@CreatedBy)
	end

------------------------------------UpdateMeetup_SP------------------------------
GO
create or alter procedure UpdateMeetup_SP
	@Id int,
	@TypeId int,
	@MeetingTime nvarchar(1000),
	@YCoordinate nvarchar(100),
	@XCoordinate nvarchar(100),
	@Description nvarchar(500),
	@AvailableSlots int,
	@Level int,
	@UserId int
	as
	begin
	UPDATE [dbo].[Meetups]
	   SET [TypeId] = @TypeId
		  ,[MeetingTime] = @MeetingTime
		  ,[YCoordinate] = @YCoordinate
		  ,[XCoordinate] = @XCoordinate
		  ,[Description] = @Description
		  ,[AvailableSlots] = @AvailableSlots
		  ,[Level] = @Level
	 WHERE Id = @Id AND CreatedBy = @UserId
	end

------------------------------------DeleteMeetup_SP------------------------------
GO
create or alter procedure DeleteMeetup_SP
	@Id int,
	@UserId int
	as
	begin
	DELETE FROM [dbo].[Meetups]
      WHERE Id = @Id And CreatedBy = @UserId
	end