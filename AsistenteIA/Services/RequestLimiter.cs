namespace AsistenteIA.Services;

/// <summary>
/// Límite de uso del lado del cliente: como máximo N consultas por ventana de tiempo.
/// Protege la cuota de la clave del estudiante y evita ráfagas accidentales (uso responsable).
/// </summary>
public class RequestLimiter
{
    public const int DefaultMaxRequests = 8;
    public static readonly TimeSpan DefaultWindow = TimeSpan.FromMinutes(1);

    private readonly int _maxRequests;
    private readonly TimeSpan _window;
    private readonly Func<DateTimeOffset> _now;
    private readonly Queue<DateTimeOffset> _stamps = new();
    private readonly object _lock = new();

    public RequestLimiter() : this(DefaultMaxRequests, DefaultWindow, () => DateTimeOffset.UtcNow) { }

    public RequestLimiter(int maxRequests, TimeSpan window, Func<DateTimeOffset> now)
    {
        _maxRequests = maxRequests;
        _window = window;
        _now = now;
    }

    /// <summary>Intenta registrar una solicitud. Si se superó el límite, indica cuánto esperar.</summary>
    public bool TryAcquire(out TimeSpan wait)
    {
        lock (_lock)
        {
            DateTimeOffset now = _now();
            while (_stamps.Count > 0 && now - _stamps.Peek() >= _window)
                _stamps.Dequeue();

            if (_stamps.Count >= _maxRequests)
            {
                wait = _window - (now - _stamps.Peek());
                return false;
            }

            _stamps.Enqueue(now);
            wait = TimeSpan.Zero;
            return true;
        }
    }
}
