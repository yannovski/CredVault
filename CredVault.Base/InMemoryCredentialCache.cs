namespace CredVault.Base;
public class InMemoryCredentialCache : ICredentialCache
{
    // dictionary to store credentials
    private Dictionary<Guid, Credential> _credentials = new();

    public void AddCredential(Credential credential)
    {
        // add credential to dictionary if it doesn't already exist
        if (!_credentials.ContainsKey(credential.CredentialId))
        {
            _credentials.Add(credential.CredentialId, credential);
        } // else update it
        else
        {
            _credentials[credential.CredentialId] = credential;
        } 
    }

    public void ClearCache() => _credentials = new Dictionary<Guid, Credential>();

    public void DeleteCredential(Credential credential)
    {
        // remove credential from dictionary if it exists
        if (_credentials.ContainsKey(credential.CredentialId))
        {
            _credentials.Remove(credential.CredentialId);
        }
    }

    public Credential? GetCredentialById(Guid id)
    {
        // get credential from dictionary if it exists
        if (_credentials.ContainsKey(id))
        {
            return _credentials[id];
        }

        return null;
        
    }

   
}
