using EventSpike.ApprovalProcessor.Projac.DataDefinition;
using Paramol.SqlClient;
using Projac;

namespace EventSpike.ApprovalProcessor.Projac
{
    internal class StoreCheckpointProjection
    {
        internal static readonly SqlProjection Instance = new SqlProjectionBuilder()
            .When<CreateSchema>(_ => TSql.NonQueryStatement(
                @"IF NOT EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='StoreCheckpoint' AND XTYPE='U')
                BEGIN
                    CREATE TABLE [StoreCheckpoint] (
                        [StoreId] NVARCHAR(100) NOT NULL CONSTRAINT PK_StoreCheckpoint PRIMARY KEY,
                        [CheckpointToken] NVARCHAR(MAX) NOT NULL)
                END"))
            .When<DropSchema>(_ => TSql.NonQueryStatement(
                @"IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='StoreCheckpoint' AND XTYPE='U') DROP TABLE [StoreCheckpoint]"))
            .When<DeleteData>(_ => TSql.NonQueryStatement(
                @"IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='StoreCheckpoint' AND XTYPE='U') DELETE FROM [StoreCheckpoint]"))
            .When<SetCheckpoint>(@event => TSql.NonQueryStatement(
                @"MERGE [StoreCheckpoint] WITH (HOLDLOCK) AS Target
                USING (SELECT @P1 AS [StoreId]) AS Source ON (Source.[StoreId] = Target.[StoreId])
                WHEN MATCHED THEN UPDATE SET [CheckpointToken] = @P2
                WHEN NOT MATCHED THEN INSERT ([StoreId], [CheckpointToken]) VALUES (@P1, @P2);", new
                {
                    P1 = TSql.NVarChar(@event.StoreId, 100),
                    P2 = TSql.NVarCharMax(@event.CheckpointToken)
                }))
            .Build();
    }
}