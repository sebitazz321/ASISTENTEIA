using AsistenteIA.Services;

namespace AsistenteIA.Tests;

public class PromptValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    public void Empty_prompt_is_rejected(string? prompt)
    {
        Assert.NotNull(PromptValidator.Validate(prompt));
    }

    [Fact]
    public void Too_short_prompt_is_rejected()
    {
        string? error = PromptValidator.Validate("hola");
        Assert.NotNull(error);
        Assert.Contains("corta", error);
    }

    [Fact]
    public void Prompt_with_minimum_length_is_accepted()
    {
        Assert.Null(PromptValidator.Validate(new string('a', PromptValidator.MinLength)));
    }

    [Fact]
    public void Spaces_around_the_text_do_not_count_as_length()
    {
        Assert.NotNull(PromptValidator.Validate("   hi   "));
    }

    [Fact]
    public void Prompt_with_maximum_length_is_accepted()
    {
        Assert.Null(PromptValidator.Validate(new string('a', PromptValidator.MaxLength)));
    }

    [Fact]
    public void Prompt_over_maximum_length_is_rejected()
    {
        string? error = PromptValidator.Validate(new string('a', PromptValidator.MaxLength + 1));
        Assert.NotNull(error);
        Assert.Contains("supera", error);
    }
}
