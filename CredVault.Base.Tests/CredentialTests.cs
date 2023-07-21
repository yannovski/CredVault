using System;
using Xunit;

namespace CredVault.Base.Tests;

public class CredentialTests
{
    // test that the constructor sets the username and password
    [Fact]
    public void Credential_Constructor_Sets_Username_And_Password()
    {
        // Arrange
        var username = "username";
        var password = "password";

        // Act
        var credential = new Credential(username, password);

        // Assert
        Assert.Equal(username, credential.Username);
        Assert.Equal(password, credential.GetPassword());
    }

    // test password is set when using SetPassword
    [Fact]
    public void Credential_SetPassword_Sets_Password()
    {
        // Arrange
        var credential = new Credential("username", "password");
        var newPassword = "newPassword";

        // Act
        credential.SetPassword(newPassword);

        // Assert
        Assert.Equal(newPassword, credential.GetPassword());
    }

    // test that a tag can be removed
    [Fact]
    public void Credential_RemoveTag_Removes_Tag()
    {
        // Arrange
        var credential = new Credential("username", "password");
        CredentialTag tag = new()
        {
            TagName = "tag", 
            TagValue = "value"
        }; 
        credential.AddTag(tag);

        // Act
        credential.RemoveTag(tag);

        // Assert
        Assert.DoesNotContain(tag, credential.Tags);
    }

    // test that an exception is thrown when removing a tag that does not exist
    [Fact]
    public void Credential_RemoveTag_ThrowsException_WhenTagDoesNotExist()
    {
        // Arrange
        var credential = new Credential("username", "password");
        CredentialTag tag = new()
        {
            TagName = "tag", 
            TagValue = "value"
        }; 

        // Act
        Action act = () => credential.RemoveTag(tag);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    // test that an exception is thrown when removing a tag that is null
    [Fact]
    public void Credential_RemoveTag_ThrowsException_WhenTagIsNull()
    {
        // Arrange
        var credential = new Credential("username", "password");
        CredentialTag tag = new();

        // Act
        Action act = () => credential.RemoveTag(tag);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    // test that a tag can be added
    [Fact]
    public void Credential_AddTag_Adds_Tag()
    {
        // Arrange
        var credential = new Credential("username", "password");
        CredentialTag tag = new()
        {
            TagName = "tag", 
            TagValue = "value"
        }; 

        // Act
        credential.AddTag(tag);

        // Assert
        Assert.Contains(tag, credential.Tags);
    }

    // test that an exception is thrown when adding a tag that already exists
    [Fact]
    public void Credential_AddTag_ThrowsException_WhenTagAlreadyExists()
    {
        // Arrange
        var credential = new Credential("username", "password");
        CredentialTag tag = new()
        {
            TagName = "tag", 
            TagValue = "value"
        }; 
        credential.AddTag(tag);

        // Act
        Action act = () => credential.AddTag(tag);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    // test that an exception is thrown when adding a tag that is null
    [Fact]
    public void Credential_AddTag_ThrowsException_WhenTagIsNull()
    {
        // Arrange
        var credential = new Credential("username", "password");
        CredentialTag tag = new();

        // Act
        Action act = () => credential.AddTag(tag);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }
    
}