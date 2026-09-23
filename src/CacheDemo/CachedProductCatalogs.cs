using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using System.Text.Json;

namespace CacheDemo;

public sealed class MemoryCachedProductCatalog(ProductCatalog catalog, IMemoryCache cache)
{
    public async Task<Product> FindAsync(int id)
    {
        var key = $"product:{id}";
        if (cache.TryGetValue(key, out Product? product) && product is not null)
            return product;

        product = await catalog.FindAsync(id).ConfigureAwait(false);
        cache.Set(key, product);
        return product;
    }
}

public sealed class RedisCachedProductCatalog(ProductCatalog catalog, IDatabase database)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<Product> FindAsync(int id)
    {
        var key = (RedisKey)$"product:{id}";
        var cached = await database.StringGetAsync(key).ConfigureAwait(false);
        if (cached.HasValue)
            return JsonSerializer.Deserialize<Product>((string)cached!, JsonOptions)!;

        var product = await catalog.FindAsync(id).ConfigureAwait(false);
        await database.StringSetAsync(key, JsonSerializer.Serialize(product, JsonOptions)).ConfigureAwait(false);
        return product;
    }
}
