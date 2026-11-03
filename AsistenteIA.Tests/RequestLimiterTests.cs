using AsistenteIA.Services;

namespace AsistenteIA.Tests;

public class RequestLimiterTests
{
    private sealed class FakeClock
    {
        public DateTimeOffset Now { get; set; } = new(2026, 1, 1, 8, 0, 0, TimeSpan.Zero);
    }

    [Fact]
    public void Allows_requests_up_to_the_limit()
    {
        var clock = new FakeClock();
        var limiter = new RequestLimiter(3, TimeSpan.FromMinutes(1), () => clock.Now);

        Assert.True(limiter.TryAcquire(out _));
        Assert.True(limiter.TryAcquire(out _));
        Assert.True(limiter.TryAcquire(out _));
    }

    [Fact]
    public void Blocks_the_request_over_the_limit_and_reports_the_wait()
    {
        var clock = new FakeClock();
        var limiter = new RequestLimiter(2, TimeSpan.FromSeconds(60), () => clock.Now);

        limiter.TryAcquire(out _);
        clock.Now += TimeSpan.FromSeconds(10);
        limiter.TryAcquire(out _);
        clock.Now += TimeSpan.FromSeconds(10);

        Assert.False(limiter.TryAcquire(out TimeSpan wait));
        Assert.Equal(TimeSpan.FromSeconds(40), wait);
    }

    [Fact]
    public void Allows_again_after_the_window_passes()
    {
        var clock = new FakeClock();
        var limiter = new RequestLimiter(1, TimeSpan.FromSeconds(30), () => clock.Now);

        Assert.True(limiter.TryAcquire(out _));
        Assert.False(limiter.TryAcquire(out _));

        clock.Now += TimeSpan.FromSeconds(31);
        Assert.True(limiter.TryAcquire(out _));
    }
}
