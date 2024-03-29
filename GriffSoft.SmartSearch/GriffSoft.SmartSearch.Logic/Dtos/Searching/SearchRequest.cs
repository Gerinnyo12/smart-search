﻿using System.Collections.Generic;

namespace GriffSoft.SmartSearch.Logic.Dtos.Searching;
public class SearchRequest
{
    public List<SearchFilter> Filters { get; set; } = new List<SearchFilter>();

    public List<SearchOr> Ors { get; set; } = new List<SearchOr>();

    public List<SearchAnd> Ands { get; set; } = new List<SearchAnd>();

    public List<SearchSort> Sorts { get; set; } = new List<SearchSort>();

    public int Size { get; set; }

    public int Offset { get; set; }
}
