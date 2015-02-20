ALTER DATABASE [$(DatabaseName)]
    ADD FILE (NAME = [AppAdmin_Log_dat], FILENAME = '$(DefaultDataPath)$(DatabaseName).mdf', FILEGROWTH = 10 %) TO FILEGROUP [PRIMARY];

