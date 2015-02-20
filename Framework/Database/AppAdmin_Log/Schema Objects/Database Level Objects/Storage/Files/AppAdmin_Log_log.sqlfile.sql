ALTER DATABASE [$(DatabaseName)]
    ADD LOG FILE (NAME = [AppAdmin_Log_log], FILENAME = '$(DefaultLogPath)$(DatabaseName)_1.ldf', FILEGROWTH = 10 %);

