using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CacheDemo;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

BenchmarkRunner.Run<ProductLookupBenchmarks>();

[MemoryDiagnoser]
public class ProductLookupBenchmarks
{
    private const int ProductId = 42;
    private ProductCatalog _catalog = null!;
    private IMemoryCache _memoryCache = null!;
    private MemoryCachedProductCatalog _memoryCatalog = null!;
    private RedisCachedProductCatalog _redisCatalog = null!;
    private IConnectionMultiplexer _redis = null!;

    [GlobalSetup]
    public async Task Setup()
    {
        var latencyMs = int.TryParse(Environment.GetEnvironmentVariable("SOURCE_LATENCY_MS"), out var value)
            ? Math.Clamp(value, 0, 1000)
            : 2;
        _catalog = new ProductCatalog(TimeSpan.FromMilliseconds(latencyMs));
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _memoryCatalog = new MemoryCachedProductCatalog(_catalog, _memoryCache);

        var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "localhost:6379";
        _redis = await ConnectionMultiplexer.ConnectAsync(redisConnection);
        _redisCatalog = new RedisCachedProductCatalog(_catalog, _redis.GetDatabase());

        // Seed both caches before measurement: these benchmark methods measure warm-cache hits.
        await _memoryCatalog.FindAsync(ProductId);
        await _redisCatalog.FindAsync(ProductId);
    }

    [Benchmark(Baseline = true)]
    public Task<Product> NoCache() => _catalog.FindAsync(ProductId);

    [Benchmark]
    public Task<Product> ProcessLocalMemory() => _memoryCatalog.FindAsync(ProductId);

    [Benchmark]
    public Task<Product> Redis() => _redisCatalog.FindAsync(ProductId);

    [GlobalCleanup]
    public void Cleanup()
    {
        _memoryCache.Dispose();
        _redis.Dispose();
    }
}
