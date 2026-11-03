using System.Net;
using AsistenteIA.Services;

namespace AsistenteIA.Tests;

public class AiErrorMapperTests
{
    private static string Map(HttpStatusCode? code, string detail = "error") =>
        AiErrorMapper.GetFriendlyMessage(new HttpRequestException(detail, null, code), false, "Gemini");

    [Fact]
    public void BadRequest_WithApiKeyDetail_IsAuthorizationError() =>
        Assert.Contains("clave de API no es válida", Map(HttpStatusCode.BadRequest, "API key not valid. Please pass a valid API key."));

    [Fact]
    public void BadRequest_Generic_SuggestsModel() =>
        Assert.Contains("Revise el modelo", Map(HttpStatusCode.BadRequest));

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized, "401")]
    [InlineData(HttpStatusCode.Forbidden, "403")]
    public void Unauthorized_And_Forbidden(HttpStatusCode code, string expected)
    {
        string msg = Map(code);
        Assert.Contains(expected, msg);
        Assert.Contains("Gemini", msg);
    }

    [Theory]
    [InlineData(HttpStatusCode.PaymentRequired, "402")]
    [InlineData(HttpStatusCode.NotFound, "404")]
    [InlineData(HttpStatusCode.RequestTimeout, "408")]
    [InlineData(HttpStatusCode.TooManyRequests, "429")]
    [InlineData(HttpStatusCode.InternalServerError, "500")]
    [InlineData(HttpStatusCode.BadGateway, "502")]
    [InlineData(HttpStatusCode.ServiceUnavailable, "503")]
    public void KnownCodes_IncludeTheCode(HttpStatusCode code, string expected) =>
        Assert.Contains(expected, Map(code));

    [Fact]
    public void NoStatus_IsConnectionProblem() =>
        Assert.Contains("No se pudo conectar", Map(null));

    [Fact]
    public void UnknownCode_IsGeneric() =>
        Assert.Equal("Error del servicio: 418.", Map((HttpStatusCode)418));

    [Fact]
    public void Cancellation_ByUser_vs_Timeout()
    {
        Assert.Equal("Consulta cancelada.", AiErrorMapper.GetFriendlyMessage(new OperationCanceledException(), cancelledByUser: true));
        Assert.Contains("Tiempo de espera", AiErrorMapper.GetFriendlyMessage(new TaskCanceledException()));
        Assert.Contains("Tiempo de espera", AiErrorMapper.GetFriendlyMessage(new TimeoutException()));
    }

    [Fact]
    public void ValidationMessages_ArePassedThrough() =>
        Assert.Equal("Sin conexión a internet.", AiErrorMapper.GetFriendlyMessage(new InvalidOperationException("Sin conexión a internet.")));

    [Fact]
    public void Messages_NeverContainTheKey()
    {
        const string key = "AIzaSyFAKEKEY1234567890abcdefghij";
        string msg = AiErrorMapper.GetFriendlyMessage(
            new HttpRequestException($"API key {key} not valid", null, HttpStatusCode.BadRequest));
        Assert.DoesNotContain(key, msg);
    }
}
