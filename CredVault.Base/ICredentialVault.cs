namespace CredVault.Base;

public interface ICredentialVault
{
    public void AddCredential(Credential credential);
    public void DeleteCredential(Credential credential);
    public void UpdateCredential(Credential credential);
    public Credential? GetCredentialById(Guid id);
    public Credential? GetCredentialByUsername(string username);

}
