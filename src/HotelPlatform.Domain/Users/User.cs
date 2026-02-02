using HotelPlatform.Domain.Common.StronglyTypedIds;

namespace HotelPlatform.Domain.Users;

public class User : AggregateRoot<UserId>
{
    public string IdentityId { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    
    private User() : base() { }

    private User(
        UserId id,
        string identityId,
        string email,
        string displayName) : base(id)
    {
        IdentityId = identityId;
        Email = email;
        DisplayName = displayName;
    }

    public static User Create(
        string identityId,
        string email,
        string displayName)
    {
        return new User(
            UserId.New(),
            identityId,
            email.ToLowerInvariant(),
            displayName);
    }

    public void UpdateProfile(string displayName, string email , DateTime utcNow)
    {
        DisplayName = displayName;
        Email = email.ToLowerInvariant();
        SetUpdated(utcNow);
    }
}