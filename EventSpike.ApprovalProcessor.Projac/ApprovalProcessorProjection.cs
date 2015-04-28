using EventSpike.ApprovalProcessor.Projac.DataDefinition;
using EventSpike.Common;
using EventSpike.Common.ApprovalEvents;
using Paramol.SqlClient;
using Projac;

namespace EventSpike.ApprovalProcessor.Projac
{
    internal class ApprovalProcessorProjection
    {
        internal static readonly SqlProjection Instance = new SqlProjectionBuilder()
            .When<CreateSchema>(_ => TSql.NonQueryStatement(
                @"IF NOT EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='ApprovalProcess' AND XTYPE='U')
                BEGIN
                    CREATE TABLE [ApprovalProcess] (
                        [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_ApprovalProcess PRIMARY KEY,
                        [CausationId] UNIQUEIDENTIFIER NOT NULL,
                        [Dispatched] DATETIME NULL,
                        [DispatchAcknowledged] BIT NOT NULL DEFAULT 0)
                END"))
            .When<DropSchema>(_ => TSql.NonQueryStatement(@"IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='ApprovalProcess' AND XTYPE='U') DROP TABLE [ApprovalProcess]"))
            .When<DeleteData>(_ => TSql.NonQueryStatement(@"IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='ApprovalProcess' AND XTYPE='U') DELETE FROM [ApprovalProcess]"))
            .When<Envelope<ApprovalInitiated>>(@event => TSql.NonQueryStatement(
                @"INSERT INTO [ApprovalProcess] ([Id], [CausationId]) VALUES (@P1, @P2)",
                new
                {
                    P1 = TSql.UniqueIdentifier(@event.Message.Id),
                    P2 = TSql.UniqueIdentifier(@event.Headers[Constants.CausationIdKey].ToGuid())
                }))
            .When<Envelope<ApprovalAccepted>>(@event => TSql.NonQueryStatement(@"UPDATE [ApprovalProcess] SET [DispatchAcknowledged] = 1 WHERE [Id] = @P1", new { P1 = TSql.UniqueIdentifier(@event.Message.Id) }))
            .Build();
    }
}
