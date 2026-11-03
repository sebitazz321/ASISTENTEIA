using AsistenteIA.Models;

namespace AsistenteIA.Tests;

public class AiProviderCatalogTests
{
    [Fact]
    public void Catalog_HasGeminiAndPollinations()
    {
        Assert.Equal(2, AiProviderCatalog.All.Count);
        Assert.Contains(AiProviderCatalog.All, p => p.Id == AiProvider.Gemini);
        Assert.Contains(AiProviderCatalog.All, p => p.Id == AiProvider.Pollinations);
    }

    [Theory]
    [InlineData("Pollinations", AiProvider.Pollinations)]
    [InlineData("pollinations", AiProvider.Pollinations)]
    [InlineData("Gemini", AiProvider.Gemini)]
    [InlineData("", AiProvider.Gemini)]
    [InlineData(null, AiProvider.Gemini)]
    [InlineData("OpenAI", AiProvider.Gemini)]
    [InlineData("7", AiProvider.Gemini)]
    public void Parse_FallsBackToGemini(string? value, AiProvider expected) =>
        Assert.Equal(expected, AiProviderCatalog.Parse(value));

    [Fact]
    public void EachProvider_HasItsOwnSecureStorageKeyAndModelPreference()
    {
        Assert.NotEqual(AiProviderCatalog.Gemini.SecureStorageKey, AiProviderCatalog.Pollinations.SecureStorageKey);
        Assert.NotEqual(AiProviderCatalog.Gemini.ModelPreferenceKey, AiProviderCatalog.Pollinations.ModelPreferenceKey);
        // Compatibilidad con la versión anterior (las claves ya guardadas no se pierden).
        Assert.Equal("ai_api_key", AiProviderCatalog.Gemini.SecureStorageKey);
        Assert.Equal("SelectedModel", AiProviderCatalog.Gemini.ModelPreferenceKey);
    }

    [Fact]
    public void DefaultModel_IsAmongSuggested()
    {
        foreach (AiProviderInfo p in AiProviderCatalog.All)
            Assert.Contains(p.DefaultModel, p.SuggestedModels);
    }

    [Theory]
    [InlineData("models/gemini-3.6-flash", "gemini-3.6-flash")]
    [InlineData("  gemini-3.8-flash ", "gemini-3.8-flash")]
    [InlineData("", "gemini-3.5-flash-lite")]
    [InlineData(null, "gemini-3.5-flash-lite")]
    public void NormalizeModel_CleansPrefixAndDefaults(string? input, string expected) =>
        Assert.Equal(expected, AiProviderCatalog.NormalizeModel(AiProviderCatalog.Gemini, input));

    [Fact]
    public void BuildEndpoint_InsertsModelOnlyForGemini()
    {
        Assert.Equal(
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-3.6-flash:generateContent",
            AiProviderCatalog.BuildEndpoint(AiProviderCatalog.Gemini, "gemini-3.6-flash"));
        Assert.Equal(
            "https://gen.pollinations.ai/v1/chat/completions",
            AiProviderCatalog.BuildEndpoint(AiProviderCatalog.Pollinations, "openai"));
    }

    [Fact]
    public void Keys_TravelInHeaders_NeverInUrl()
    {
        foreach (AiProviderInfo p in AiProviderCatalog.All)
        {
            Assert.False(string.IsNullOrWhiteSpace(p.KeyHeader));
            Assert.DoesNotContain("key=", p.Endpoint, StringComparison.OrdinalIgnoreCase);
        }
    }
}
