namespace CredVault.Base;
public interface ICredentialValidator
{
    /// <summary>
    /// Validates a password against the policy requirements.
    /// </summary>

    // validate password
    bool ValidatePassword(string password);
}