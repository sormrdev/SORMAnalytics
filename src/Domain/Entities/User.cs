namespace Domain.Entities;
public class User
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string IdentityId { get; set; } = string.Empty;
    private User() { }
    private User(
        string id,
        string name,
        string email,
        string identityId)
    {
        Id = ValidateId(id);
        Name = ValidateName(name);
        Email = ValidateEmail(email);
        IdentityId = ValidateIdentityId(identityId);

        CreatedAt = DateTime.UtcNow;
    }

    public static User Create(
        string id,
        string name,
        string email,
        string identityId)
    {
        return new User(id, name, email, identityId);
    }

    public void UpdateProfile(string name, string email)
    {
        Name = ValidateName(name);
        Email = ValidateEmail(email);
        Touch();
    }

    public void ChangeIdentity(string identityId)
    {
        IdentityId = ValidateIdentityId(identityId);
        Touch();
    }

    private void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    private static string ValidateId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("User Id cannot be empty.");

        return id.Trim();
    }

    private static string ValidateIdentityId(string identityId)
    {
        if (string.IsNullOrWhiteSpace(identityId))
            throw new ArgumentException("IdentityId cannot be empty.");

        return identityId.Trim();
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.");

        name = name.Trim();

        if (name.Length < 2)
            throw new ArgumentException("Name must have at least 2 characters.");

        if (name.Length > 100)
            throw new ArgumentException("Name cannot exceed 100 characters.");

        return name;
    }

    private static string ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.");

        email = email.Trim().ToLowerInvariant();

        if (email.Length > 256)
            throw new ArgumentException("Email is too long.");

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);

            if (addr.Address != email)
                throw new ArgumentException("Invalid email format.");
        }
        catch
        {
            throw new ArgumentException("Invalid email format.");
        }

        return email;
    }
}
