namespace INuStore.UseCase.Abstractions.Notifications;

public interface IEmailTemplateRenderer
{
    string Render(string templateName, IReadOnlyDictionary<string, string>? parameters = null);
}
