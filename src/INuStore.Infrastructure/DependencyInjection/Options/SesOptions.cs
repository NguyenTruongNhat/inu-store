using System.ComponentModel.DataAnnotations;

namespace INuStore.Infrastructure.DependencyInjection.Options;

public sealed class SesOptions
{
    public const string SectionName = "Ses";

    [Required]
    public string Region { get; init; } = string.Empty;

    [Required]
    public string From { get; init; } = string.Empty;

    [Required]
    public string AccessKeyId { get; init; } = string.Empty;

    [Required]
    public string SecretAccessKey { get; init; } = string.Empty;
}
