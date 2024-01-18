﻿using GriffSoft.SmartSearch.Logic.Dtos.Enums;

namespace GriffSoft.SmartSearch.Logic.Dtos;
public class SearchAnd
{
    public SearchMatchType MatchType { get; init; } = SearchMatchType.Prefix;

    public required string FieldName { get; init; }

    public required object FieldValue { get; init; }
}