using System;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using CredVault.Base;

namespace CredVault.Base.Tests
{
    public class InMemoryCredentialVaultTests
    {
        private readonly Mock<IOptions<VaultCredentialOptions>> _optionsMock;

        public InMemoryCredentialVaultTests()
        {
            _optionsMock = new Mock<IOptions<VaultCredentialOptions>>();
            _optionsMock.SetupGet(o => o.Value).Returns(new VaultCredentialOptions());
        }

        [Fact]
        public void AddCredential_ShouldEncryptPassword()
        {
            // Arrange
            var credential = new Credential
            {
                Username = "testuser",
                Password = "testpassword"
            };

            var vault = new InMemoryCredentialVault(_optionsMock.Object);

            // Act
            vault.AddCredential(credential);

            // Assert
            var storedCredential = vault.GetCredentialById(credential.CredentialId);
            Assert.NotEqual(credential.Password, storedCredential.GetPassword());
        }

        [Fact]
        public void GetCredentialById_ShouldReturnCorrectCredential()
        {
            // Arrange
            var credential1 = new Credential
            {
                Username = "testuser1",
                Password = "testpassword1"
            };

            var credential2 = new Credential
            {
                Username = "testuser2",
                Password = "testpassword2"
            };

            var vault = new InMemoryCredentialVault(_optionsMock.Object);
            vault.AddCredential(credential1);
            vault.AddCredential(credential2);

            // Act
            var storedCredential = vault.GetCredentialById(credential1.CredentialId);

            // Assert
            Assert.Equal(credential1, storedCredential);
        }

        [Fact]
        public void GetCredentialByUsername_ShouldReturnCorrectCredential()
        {
            // Arrange
            var credential1 = new Credential
            {
                Username = "testuser1",
                Password = "testpassword1"
            };

            var credential2 = new Credential
            {
                Username = "testuser2",
                Password = "testpassword2"
            };

            var vault = new InMemoryCredentialVault(_optionsMock.Object);
            vault.AddCredential(credential1);
            vault.AddCredential(credential2);

            // Act
            var storedCredential = vault.GetCredentialByUsername(credential2.Username);

            // Assert
            Assert.Equal(credential2, storedCredential);
        }

        [Fact]
        public void UpdateCredential_ShouldEncryptPassword()
        {
            // Arrange
            var credential = new Credential
            {
                Username = "testuser",
                Password = "testpassword"
            };

            var vault = new InMemoryCredentialVault(_optionsMock.Object);
            vault.AddCredential(credential);

            // Act
            credential.Password = "newpassword";
            vault.UpdateCredential(credential);

            // Assert
            var storedCredential = vault.GetCredentialById(credential.CredentialId);
            Assert.NotEqual(credential.Password, storedCredential.GetPassword());
        }

        [Fact]
        public void DeleteCredential_ShouldRemoveCredentialFromList()
        {
            // Arrange
            var credential = new Credential
            {
                Username = "testuser",
                Password = "testpassword"
            };

            var vault = new InMemoryCredentialVault(_optionsMock.Object);
            vault.AddCredential(credential);

            // Act
            vault.DeleteCredential(credential);

            // Assert
            var storedCredential = vault.GetCredentialById(credential.CredentialId);
            Assert.Null(storedCredential);
        }
    }
}