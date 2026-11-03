using AsistenteIA.Models;

namespace AsistenteIA.Tests;

public class UserProfileTests
{
    [Theory]
    [InlineData("", "?")]
    [InlineData("   ", "?")]
    [InlineData("juan", "J")]
    [InlineData("Juan Carlos Díaz", "JD")]
    [InlineData("ana  maría", "AM")]
    public void Initials_are_built_from_first_and_last_name(string name, string expected)
    {
        Assert.Equal(expected, new UserProfile { Name = name }.Initials);
    }
}
