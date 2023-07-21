namespace CredVault.Base;

public class Credential
{
    public Guid CredentialId { get; set; }
    public string? Username { get; set; }
    private string? _password;
    public List<CredentialTag> Tags { get; set; }
    public DateTime? ExpiryDate { get; private set;}

    public Credential(string username, string password)
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
        _password = password ?? throw new ArgumentNullException(nameof(password));
        Tags = new List<CredentialTag>();
        CredentialId = new Guid();
    }

    public void AddTag(CredentialTag tagToAdd)
    {
        //check if tag properties are null
        if (tagToAdd.TagName == null || tagToAdd.TagValue == null)
        {
            throw new ArgumentNullException(nameof(tagToAdd));
        }

        if (Tags.Where(x => x.TagName == tagToAdd.TagName).Any())
        {
            throw new ArgumentException("Tag already exists");
        }

        Tags.Add(tagToAdd);
    }

    public string? GetPassword()
    {
        return _password;
    }

    // method to setpassword
    public void SetPassword(string? password)
    {
        _password = password ?? throw new ArgumentNullException(nameof(password));
    }

    public void SetExpiryDate(DateTime? expiryDate)
    {
        if (expiryDate.HasValue && expiryDate.Value < DateTime.Now)
        {
            throw new ArgumentException("Expiry date cannot be in the past");
        }

        ExpiryDate = expiryDate;
    }
}
