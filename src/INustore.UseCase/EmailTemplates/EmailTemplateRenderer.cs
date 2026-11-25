using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using INuStore.UseCase.Abstractions.Notifications;

namespace INuStore.UseCase.EmailTemplates;

public sealed class EmailTemplateRenderer : IEmailTemplateRenderer
{
    public string Render(string templateName, IReadOnlyDictionary<string, string>? parameters = null)
    {
        if (string.IsNullOrWhiteSpace(templateName) || !Regex.IsMatch(templateName, "^[A-Za-z][A-Za-z0-9]*$"))
        {
            throw new ArgumentException("Invalid email template name.", nameof(templateName));
        }

        var resourceName = $"INuStore.UseCase.EmailTemplates.{templateName}.html";

        using var stream = typeof(EmailTemplateRenderer)
            .Assembly.GetManifestResourceStream(resourceName) ?? throw new InvalidOperationException($"Email template resource '{resourceName}' was not found.");

        using var reader = new StreamReader(stream);

        var html = reader.ReadToEnd();

        return Regex.Replace(
            html,
            @"\{\{(?<name>[A-Za-z][A-Za-z0-9]*)\}\}",
            match =>
            {
                var name = match.Groups["name"].Value;

                if (parameters is null || !parameters.TryGetValue(name, out var value))
                {
                    throw new InvalidOperationException(
                        $"Email template '{templateName}' requires parameter '{name}'.");
                }

                return HtmlEncoder.Default.Encode(value);
            });
    }
}
