namespace CacheDemo;

/// <summary>A deterministic stand-in for an expensive database lookup.</summary>
public sealed class ProductCatalog(TimeSpan simulatedLatency)
{
    public async Task<Product> FindAsync(int id)
    {
        await Task.Delay(simulatedLatency).ConfigureAwait(false);
        return CreateProduct(id);
    }

    public static Product CreateProduct(int id) =>
        new(id, $"Product-{id:D6}", 10m + (id % 10_000) / 100m);
}
