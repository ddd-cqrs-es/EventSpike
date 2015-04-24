using EventSpike.ApprovalProcessor.Projac.DataDefinition;
using EventSpike.Common;
using EventSpike.Common.ApprovalEvents;
using Paramol.SqlClient;
using Projac;

namespace EventSpike.ApprovalProcessor.Projac
{
    class ApprovalProcessorProjection
    {
        private static readonly SqlProjection Instance = new SqlProjectionBuilder()
            .When<CreateSchema>(_ =>
                TSql.NonQueryStatement(
                    @"IF NOT EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='ApprovalProcess' AND XTYPE='U')
                        BEGIN
                            CREATE TABLE [ApprovalProcess] (
                                [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_Portfolio PRIMARY KEY,
                                [Description] NVARCHAR(MAX) NOT NULL,
                                [State] TINYINT NOT NULL)
                        END"))
            .When<DropSchema>(_ =>
                TSql.NonQueryStatement(
                    @"IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='ApprovalProcess' AND XTYPE='U')
                        DROP TABLE [ApprovalProcess]"))
            .When<DeleteData>(_ =>
                TSql.NonQueryStatement(
                    @"IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='ApprovalProcess' AND XTYPE='U'
                        DELETE FROM [ApprovalProcess]"))
            .When<IEnvelope<ApprovalInitiated>>(@event =>
                TSql.NonQueryStatement(
                    @"INSERT INTO [ApprovalProcess] ([Id], [Description], [State]) VALUES (@P1, @P2, 0)",
                    new {P1 = TSql.UniqueIdentifier(@event.Body.Id), P2 = TSql.NVarCharMax(@event.Body.Description)}))
            .When<IEnvelope<ApprovalAccepted>>(@event =>
                TSql.NonQueryStatement(
                    @"DELETE FROM [ApprovalProcess] WHERE [Id] = @P1",
                    new {P1 = TSql.UniqueIdentifier(@event.Body.Id)}))
            .Build();
    }
}
