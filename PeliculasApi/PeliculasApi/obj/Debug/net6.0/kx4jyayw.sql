BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230728144853_AdminData', N'6.0.20');
GO

COMMIT;
GO

