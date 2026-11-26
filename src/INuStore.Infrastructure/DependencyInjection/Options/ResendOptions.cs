using System.ComponentModel.DataAnnotations;

namespace INuStore.Infrastructure.DependencyInjection.Options;

public sealed class ResendOptions
{
    public const string SectionName = "Resend";

    [Required(ErrorMessage = "Resend API key is required.")]
    public string ApiKey { get; init; } = string.Empty;

    [Required(ErrorMessage = "Resend sender address is required.")]
    public string From { get; init; } = string.Empty;
}
