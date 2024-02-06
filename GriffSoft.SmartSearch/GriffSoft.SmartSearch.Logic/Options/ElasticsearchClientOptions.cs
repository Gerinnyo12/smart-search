using System;

namespace GriffSoft.SmartSearch.Logic.Options;
public class ElasticsearchClientOptions : IValidatable
{
    public string Url { get; init; } = "https://localhost:9200";

    public required string Fingerprint { get; init; }

    public required string Username { get; init; }

    public required string Password { get; init; }

    public required IndexOptions IndexOptions { get; set; }

    public void InvalidateIfIncorrect()
    {
        if (!Uri.TryCreate(Url, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new Exception("The URL provided for elastic client is not a valid URL.");
        }

        if (string.IsNullOrWhiteSpace(Fingerprint))
        {
            throw new Exception($"{nameof(Fingerprint)} must be provided.");
        }

        if (string.IsNullOrWhiteSpace(Username))
        {
            throw new Exception($"{nameof(Username)} must be provided.");
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            throw new Exception($"{nameof(Password)} must be provided.");
        }

        IndexOptions.InvalidateIfIncorrect();
    }
}
