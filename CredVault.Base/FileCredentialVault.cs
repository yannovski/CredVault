using System.Text.Json;

namespace CredVault.Base;

public class FileCredentialVault : ICredentialVault
{
    private readonly string _filePath;
    private readonly CredentialPolicy _defaultPolicy;
    private readonly ICredentialCache _credentialCache;

    public FileCredentialVault(string filePath, CredentialPolicy defaultPolicy, ICredentialCache credentialCache)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        _defaultPolicy = defaultPolicy ?? throw new ArgumentNullException(nameof(defaultPolicy));
        _credentialCache = credentialCache ?? throw new ArgumentNullException(nameof(credentialCache));

        if (!File.Exists(_filePath))
        {
            File.Create(_filePath).Dispose();
        }
    }

    public Credential? GetCredentialById(Guid id)
    {
        // Get credential from cache if it exists
        var cachedCredential = _credentialCache.GetCredentialById(id);
        if (cachedCredential != null)
        {
            return cachedCredential;
        }

        // Get all credentials from file
        var credentials = GetAllCredentials() ?? throw new InvalidOperationException($"Credential with ID {id} not found.");

        // Find credential by ID
        Credential returnCred = credentials.FirstOrDefault(c => c.CredentialId == id) ?? throw new InvalidOperationException($"Credential with ID {id} not found.");
        
        // Add credential to cache
        _credentialCache.AddCredential(returnCred);

        return returnCred;
    }

    public void AddCredential(Credential credential)
    {
        // Check if credential properties are null
        if (credential.Username == null || credential.GetPassword() == null)
        {
            throw new ArgumentNullException(nameof(credential));
        }

        // Validate password against policy requirements
        ValidatePassword(credential.GetPassword());

        // Check if credential already exists
        if (GetCredentialByUsername(credential.Username) != null)
        {
            throw new ArgumentException($"Credential with username {credential.Username} already exists.");
        }

        // Add credential to file
        List<Credential>? credentials = GetAllCredentials() ?? throw new InvalidOperationException($"Credential with username {credential.Username} not found.");
        credentials.Add(credential);
        SaveCredentialsToFile(credentials);
    }

    public void UpdateCredential(Credential credential)
    {
        // Check if credential properties are null
        if (credential.Username == null || credential.GetPassword() == null)
        {
            throw new ArgumentNullException(nameof(credential));
        }

        // Validate password against policy requirements
        ValidatePassword(credential.GetPassword());

        // Find credential to update
        var existingCredential = GetCredentialByUsername(credential.Username) ?? throw new ArgumentException($"Credential with username {credential.Username} not found.");

        // Update credential properties
        existingCredential.SetPassword(credential.GetPassword());
        existingCredential.SetExpiryDate(credential.ExpiryDate);
        existingCredential.Tags = credential.Tags;

        // remove credential from cache
        _credentialCache.DeleteCredential(existingCredential);

        // Save updated credentials to file
        SaveCredentialsToFile(GetAllCredentials());
    }

    public void DeleteCredential(Credential credential)
    {
        // Check if credential is null
        if (credential == null)
        {
            throw new ArgumentNullException(nameof(credential));
        }            

        // Delete credential from file
        List<Credential>? credentials = GetAllCredentials() ?? throw new InvalidOperationException($"Credential with username {credential.Username} not found.");
        credentials.Remove(credential);
        SaveCredentialsToFile(credentials);

        // remove credential from cache
        _credentialCache.DeleteCredential(credential);
    }

    public Credential? GetCredentialByUsername(string? username)
    {
        // Check if username is null
        if (username == null)
        {
            throw new ArgumentNullException(nameof(username));
        }

        // Get all credentials from file
        var credentials = GetAllCredentials() ?? throw new InvalidOperationException($"Credential with username {username} not found.");

        // Find credential by username
        return credentials.FirstOrDefault(c => c.Username == username);
    }

    private List<Credential>? GetAllCredentials()
    {
        // Read credentials from file
        var credentialsJson = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Credential>>(credentialsJson);
    }

    private void SaveCredentialsToFile(List<Credential>? credentials)
    {
        // Check if credentials is null
        if (credentials == null)
        {
            throw new ArgumentNullException(nameof(credentials));
        }
        // Serialize credentials to JSON
        var credentialsJson = JsonSerializer.Serialize(credentials);

        // Write credentials to file
        File.WriteAllText(_filePath, credentialsJson);
    }

    private void ValidatePassword(string? password)
    {
        // Check if password is null
        if (password == null)
        {
            throw new ArgumentNullException(nameof(password));
        }

        // Check if password meets policy requirements
        if (!_defaultPolicy.ValidatePassword(password))
        {
            throw new ArgumentException("Password does not meet policy requirements.");
        }
    }
}
