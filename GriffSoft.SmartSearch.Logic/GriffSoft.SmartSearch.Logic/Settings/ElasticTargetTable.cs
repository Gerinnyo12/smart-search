using GriffSoft.SmartSearch.Logic.Interfaces;

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace GriffSoft.SmartSearch.Logic.Settings;
public class ElasticTargetTable : IValidatable
{
    private const string SqlValidatorRegex = "^[a-zA-Z_][a-zA-Z0-9_]*$";

    public required string Table { get; init; }

    public required string[] Columns { get; init; }

    public required string[] Keys { get; init; }

    public void InvalidateIfIncorrect()
    {
        if (string.IsNullOrWhiteSpace(Table))
        {
            throw new Exception($"{nameof(Table)} must be provided.");
        }

        if (!Regex.IsMatch(Table, SqlValidatorRegex))
        {
            throw new Exception($"{nameof(Table)} is not a valid sql table name.");
        }

        if (!Columns.Any())
        {
            throw new Exception("At least 1 column must be provided.");
        }

        foreach (var column in Columns)
        {
            if (string.IsNullOrWhiteSpace(column))
            {
                throw new Exception($"Every {nameof(column)} name must be a valid string.");
            }

            if (!Regex.IsMatch(column, SqlValidatorRegex))
            {
                throw new Exception($"{nameof(column)} is not a valid sql column name.");
            }
        }

        if (!Keys.Any())
        {
            throw new Exception("The keys of the table must be provided.");
        }

        foreach (var key in Keys)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new Exception($"Every {nameof(key)} must be a valid string.");
            }

            if (!Regex.IsMatch(key, SqlValidatorRegex))
            {
                throw new Exception($"{nameof(key)} is not a valid sql key name.");
            }
        }
    }
}
