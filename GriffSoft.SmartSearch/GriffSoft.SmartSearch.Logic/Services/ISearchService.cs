﻿using GriffSoft.SmartSearch.Logic.Dtos;

using System.Threading.Tasks;

namespace GriffSoft.SmartSearch.Logic.Services;
public interface ISearchService
{
    // TODO DOCCOMMENT ABOUT EXCEPTION THROWING
    Task EnsureAvailableAsync();

    Task PrepareDataAsync();

    Task<SearchResult<T>> SearchAsync<T>(PaginatedSearchQuery paginatedSearchQuery) where T : class;
}
