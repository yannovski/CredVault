namespace CredVault.Base;
public class CredentialPolicy : ICredentialValidator
{
    public readonly int MinimumLength;
    public readonly int MaximumLength;
    public readonly bool IsCacheable;

    // constructor
    public CredentialPolicy(int minimumLength, int maximumLength, bool isCacheable)
    {
        MinimumLength = minimumLength;
        MaximumLength = maximumLength;
        IsCacheable = isCacheable;
    }

    public bool ValidatePassword(string? password)
    {
        if (password == null)
        {
            return false;
        }
        
        if (password.Length < MinimumLength || password.Length > MaximumLength)
        {
            return false;
        }

        return true;
    }
}

