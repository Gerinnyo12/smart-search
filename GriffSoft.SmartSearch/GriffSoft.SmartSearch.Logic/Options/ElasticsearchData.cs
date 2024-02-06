using System;
using System.Linq;

namespace GriffSoft.SmartSearch.Logic.Options;
public class ElasticsearchData : IValidatable
{
    public required int BatchSize { get; init; }

    public required ElasticTarget[] ElasticTargets { get; init; }

    public void InvalidateIfIncorrect()
    {
        if (BatchSize <= 0)
        {
            throw new Exception($"{nameof(BatchSize)} must be greater than 0.");
        }

        if (!ElasticTargets.Any())
        {
            throw new Exception("At least 1 target must be provided.");
        }

        foreach (var elasticTarget in ElasticTargets)
        {
            elasticTarget.InvalidateIfIncorrect();
        }
    }
}
