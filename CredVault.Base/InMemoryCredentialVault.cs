using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace CredVault.Base
{
    public class InMemoryCredentialVault : ICredentialVault
    {
        private readonly List<Credential> _credentialList;
        private readonly VaultCredentialOptions _options;
        public CredentialPolicy DefaultPolicy { get; }
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public InMemoryCredentialVault(IOptions<VaultCredentialOptions> options, CredentialPolicy defaultPolicy)
        {
            _credentialList = new List<Credential>();
            _options = options.Value;

            // Generate a random key and IV for AES encryption
            using var aes = Aes.Create();
            _key = aes.Key;
            _iv = aes.IV;
            DefaultPolicy = defaultPolicy;
        }

        // Encrypts a password using AES encryption
        private string EncryptPassword(string password)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using var sw = new StreamWriter(cs);
                sw.Write(password);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        // Decrypts a password using AES encryption
        private string DecryptPassword(string encryptedPassword)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream(Convert.FromBase64String(encryptedPassword));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }

        public void AddCredential(Credential credential)
        {
            
            if (credential == null || credential.GetPassword() == null)
            {
                throw new ArgumentNullException(nameof(credential));
            }

            EnsureDuplicateCondition(credential);

            // Validate password against policy requirements
            if (!DefaultPolicy.ValidatePassword(credential.GetPassword()))
            {
                throw new ArgumentException("Password does not meet policy requirements");
            }

            var password = credential.GetPassword() ?? throw new ArgumentNullException(nameof(credential));

            credential.SetPassword(EncryptPassword(password));
            _credentialList.Add(credential);
        }

        public void DeleteCredential(Credential credential)
        {
            if (credential == null)
            {
                throw new ArgumentNullException(nameof(credential));
            }

            _credentialList.Remove(credential);
        }

        public Credential? GetCredentialById(Guid id)
        {
            var credential = _credentialList.FirstOrDefault(c => c.CredentialId == id);

            if (credential == null)
            {
                return null;
            }

            return DecryptCredential(credential);
        }

        public Credential? GetCredentialByUsername(string username)
        {
            var credential = _credentialList.FirstOrDefault(c => c.Username == username);

            if (credential == null)
            {
                return null;
            }

            return DecryptCredential(credential);
        }

        public void UpdateCredential(Credential credential)
        {
            if (credential == null || credential.GetPassword() == null)
            {
                throw new ArgumentNullException(nameof(credential));
            }

            // Validate password against policy requirements
            if (!DefaultPolicy.ValidatePassword(credential.GetPassword()))
            {
                throw new ArgumentException("Password does not meet policy requirements");
            }

            var existingCredential = GetCredentialById(credential.CredentialId) ?? throw new ArgumentException($"Credential with ID {credential.CredentialId} not found.");

            // Encrypt the password before updating the credential in the list, use setpassword method to encrypt
            var password = credential.GetPassword() ?? throw new ArgumentNullException(nameof(credential));
            credential.SetPassword(EncryptPassword(password));
            _credentialList.Remove(existingCredential);
            _credentialList.Add(credential);
        }

        private void EnsureDuplicateCondition(Credential credential)
        {
            if (!_options.AllowDuplicateUsernames)
            {
                if (credential.Username == null)
                {
                    throw new ArgumentNullException(nameof(credential));
                }

                var existingCredential = GetCredentialByUsername(credential.Username);

                if (existingCredential != null)
                {
                    throw new ArgumentException($"Credential with username {credential.Username} already exists.");
                }
            }
        }

        private Credential DecryptCredential(Credential credential)
        {
            var password = credential.GetPassword() ?? throw new InvalidOperationException(nameof(credential));
            credential.SetPassword(DecryptPassword(password));

            return credential;
        }

        
    }
}