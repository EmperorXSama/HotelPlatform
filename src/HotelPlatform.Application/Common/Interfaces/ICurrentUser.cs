namespace HotelPlatform.Application.Common.Interfaces;
public interface ICurrentUser
{
    string? Id { get; }
    string? Email { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
    IReadOnlyList<string> Roles { get; }
    
    bool IsInRole(string role);
    bool HasAnyRole(params string[] roles);

}
