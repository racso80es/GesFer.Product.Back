using System.Text.Json.Serialization;

namespace GesFer.Product.Back.Infrastructure.DTOs.Geo;

/// <summary>
/// DTOs de lectura alineados al contrato Admin (<c>/api/geolocation/*</c>).
/// </summary>
public sealed class CountryGeoReadDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}

public sealed class StateGeoReadDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("countryId")]
    public Guid CountryId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string? Code { get; set; }
}

public sealed class CityGeoReadDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("stateId")]
    public Guid StateId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public sealed class PostalCodeGeoReadDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("cityId")]
    public Guid CityId { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
}
