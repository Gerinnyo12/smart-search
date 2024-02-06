using GriffSoft.SmartSearch.Database.Connection;

namespace GriffSoft.SmartSearch.Database.Dtos;
public class SqlQueryDto
{
    public required SqlConnector SqlConnector { get; init; }
}
