# Redis cache benchmark

A small C# teaching project comparing the same deterministic product lookup with no cache, process-local memory cache, and Redis.

## Stack and workload

- .NET 10 LTS, selected for its current long-term support window.
- `Microsoft.Extensions.Caching.Memory` for the process-local cache.
- StackExchange.Redis 3.0.11 for Redis access.
- BenchmarkDotNet 0.15.8 for warmup, repeated measurements, throughput-oriented operation rates, latency statistics, and environment reporting.
- The lookup returns a deterministic `Product` for product ID 42. The source simulates an asynchronous backing-store delay, 2 ms by default, controlled by `SOURCE_LATENCY_MS` (0–1000 ms).

The benchmark seeds both caches during setup. Its measurements therefore compare an uncached source lookup against warm memory and Redis hits; connection setup and cache population are outside the measured methods. This first workload focuses on the warm-hit path. BenchmarkDotNet's reports include latency distributions and throughput statistics; results describe only the machine and Redis deployment used for that run.

## Prerequisites

- .NET 10 SDK
- Docker with the Compose plugin (Redis scenarios only)

## Run

Start Redis:

```powershell
docker compose up -d
```

Run the benchmarks:

```powershell
dotnet run -c Release --project benchmarks/CacheDemo.Benchmarks
```

Set `REDIS_CONNECTION` to override `localhost:6379`. Redis is required for this combined benchmark run; the `ProductCatalog` and memory-cache example can be used without Redis.

Stop Redis when finished:

```powershell
docker compose down
```

BenchmarkDotNet writes reports under `BenchmarkDotNet.Artifacts`. Compare the reported means, median and tail latency percentiles, and operation rates. The artificial source delay is a teaching parameter, not a claim about real database performance.
