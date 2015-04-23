using EventSpike.Common.ApprovalEvents;
using Paramol.SqlClient;
using Projac;

namespace EventSpike.ApprovalProcessor.Projac
{
    internal class CreateSchema
    {
    }

    public class DeleteData
    {
    }

    public class DropSchema
    {
    }
    
    public class SetCheckpoint
    {
        public readonly long Checkpoint;

        public SetCheckpoint(long checkpoint)
        {
            Checkpoint = checkpoint;
        }
    }

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
            .When<ApprovalInitiated>(@event =>
                TSql.NonQueryStatement(
                    @"INSERT INTO [ApprovalProcess] ([Id], [Description], [State]) VALUES (@P1, @P2, 0)",
                    new {P1 = @event.Id, P2 = @event.Description}))
            .Build();
    }
}
