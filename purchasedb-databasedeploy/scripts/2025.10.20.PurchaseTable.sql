
use PurchaseDB

GO

IF NOT EXISTS( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Purchase]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[Purchase](
		[PurchaseId] [UniqueIdentifier] NOT NULL DEFAULT NEWSEQUENTIALID(),
		[Description] [Nvarchar](255) NOT NULL,
		[TransactionDate] [datetime] NOT NULL,
		[TotalAmount] [decimal](18, 2) NOT NULL);
		
	ALTER TABLE [Purchase] ADD CONSTRAINT [PK_Purchase] PRIMARY KEY CLUSTERED (PurchaseId);

END

GO