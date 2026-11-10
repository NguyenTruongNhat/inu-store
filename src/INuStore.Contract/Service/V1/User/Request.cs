namespace INuStore.Contract.Service.V1.User;

/// <summary>Contains route models for user resources.</summary>
public static class Request
{
    /// <summary>Identifies the user whose role assignments are being managed.</summary>
    public sealed record UserRolesRoute(int UserId);
}
