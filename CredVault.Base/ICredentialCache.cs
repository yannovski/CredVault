namespace CredVault.Base;
public interface ICredentialCache
{
    void AddCredential(Credential credential);
    void DeleteCredential(Credential credential);
    Credential? GetCredentialById(Guid id);
    void ClearCache();
}
