namespace GriffSoft.SmartSearch.Logic.Dtos.Synchronization;
public class ElasticTablePurgeDto
{
    public required string Server { get; init; }

    public required string Database { get; init; }

    public required string Table { get; init; }
}
