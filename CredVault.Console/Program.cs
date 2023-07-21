using System;
using CredVault.Base;
using Microsoft.Extensions.Options;

namespace CredVault.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = Options.Create(new VaultCredentialOptions
            {
                AllowDuplicateUsernames = false
            });

            var vault = new InMemoryCredentialVault(options);

            var credential = new Credential("testuser","testpassword");

            vault.AddCredential(credential);

            var storedCredential = vault.GetCredentialById(credential.CredentialId);

            Console.WriteLine($"Username: {storedCredential.Username}");
            Console.WriteLine($"Password: {storedCredential.GetPassword()}");

            Console.ReadLine();
        }
    }
}