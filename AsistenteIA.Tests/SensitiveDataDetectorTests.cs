using AsistenteIA.Services;

namespace AsistenteIA.Tests;

public class SensitiveDataDetectorTests
{
    [Fact]
    public void Plain_academic_text_has_no_findings()
    {
        Assert.Empty(SensitiveDataDetector.Detect("Explica con un ejemplo qué es la fotosíntesis."));
    }

    [Fact]
    public void Years_and_short_numbers_are_not_flagged()
    {
        Assert.Empty(SensitiveDataDetector.Detect("Compara los modelos de 2024 y 2025 con 12 ejemplos."));
    }

    [Fact]
    public void Null_or_blank_text_has_no_findings()
    {
        Assert.Empty(SensitiveDataDetector.Detect(null));
        Assert.Empty(SensitiveDataDetector.Detect("   "));
    }

    [Fact]
    public void Email_is_detected()
    {
        Assert.Contains("correo electrónico",
            SensitiveDataDetector.Detect("Escríbeme a juan.perez@correo.edu.co por favor"));
    }

    [Fact]
    public void Valid_card_number_is_detected()
    {
        // Número de prueba público de Visa (pasa el algoritmo de Luhn).
        Assert.Contains("número de tarjeta",
            SensitiveDataDetector.Detect("Mi tarjeta es 4111 1111 1111 1111"));
    }

    [Fact]
    public void Sixteen_digits_failing_luhn_are_not_a_card()
    {
        Assert.DoesNotContain("número de tarjeta",
            SensitiveDataDetector.Detect("Código 4111 1111 1111 1112"));
    }

    [Fact]
    public void Colombian_mobile_number_is_detected()
    {
        Assert.Contains("número de teléfono o identificación",
            SensitiveDataDetector.Detect("Llámame al 300 123 4567"));
    }

    [Fact]
    public void Long_identification_number_is_detected()
    {
        Assert.Contains("número de teléfono o identificación",
            SensitiveDataDetector.Detect("Mi cédula es 1020304050"));
    }

    [Theory]
    [InlineData("mi contraseña: abc12345")]
    [InlineData("password = hunter2")]
    [InlineData("api_key: xyz")]
    public void Credentials_are_detected(string text)
    {
        Assert.Contains("contraseña o credencial", SensitiveDataDetector.Detect(text));
    }

    [Fact]
    public void Api_key_like_string_is_detected()
    {
        Assert.Contains("clave de API",
            SensitiveDataDetector.Detect("usa AIzaSyA1234567890abcdefghijklmnopqrstu para probar"));
    }

    [Fact]
    public void Several_kinds_are_reported_once_each()
    {
        var found = SensitiveDataDetector.Detect("correo a@b.com y otro c@d.com, celular 3001234567");
        Assert.Equal(1, found.Count(f => f == "correo electrónico"));
        Assert.Contains("número de teléfono o identificación", found);
    }

    [Theory]
    [InlineData("4111111111111111", true)]
    [InlineData("4111-1111-1111-1111", true)]
    [InlineData("4111111111111112", false)]
    [InlineData("1234", false)]
    [InlineData("abcd efgh ijkl mnop", false)]
    public void Luhn_algorithm_works(string number, bool expected)
    {
        Assert.Equal(expected, SensitiveDataDetector.PassesLuhn(number));
    }
}
