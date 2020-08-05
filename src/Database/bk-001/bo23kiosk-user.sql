/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [bo23kiosk]    Script Date: 08/01/2017 17:49:25 ******/
CREATE LOGIN [bo23kiosk] WITH PASSWORD=N'Mí+~É/Ìì+B_QÐ9EÒìn4:xý,y3', DEFAULT_DATABASE=[dbBO23Kiosk], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO

ALTER LOGIN [bo23kiosk] DISABLE
GO

