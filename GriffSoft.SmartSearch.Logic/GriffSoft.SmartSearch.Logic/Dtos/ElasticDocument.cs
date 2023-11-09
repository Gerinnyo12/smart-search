using System.Collections.Generic;

namespace GriffSoft.SmartSearch.Logic.Dtos;
internal record ElasticDocument(
    string Server, 
    string Database, 
    string Table, 
    string Column, 
    Dictionary<string, object> Keys, 
    string Value);
