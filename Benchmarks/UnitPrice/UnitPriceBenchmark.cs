using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarks.UnitPrice;

[MemoryDiagnoser]
public class UnitPriceBenchmark
{
    private List<PriceTier> _tiers = null!;
    private PriceTable _priceTable = null!;
    private int _quantity;

    [Params(3, 1000)]
    public int TierCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _tiers = Enumerable.Range(0, TierCount)
            .Select(i => new PriceTier(i * 10 + 1, 100m - i * 0.05m))
            .ToList();

        _priceTable = new PriceTable(_tiers);

        _quantity = (TierCount / 2) * 10 + 5;
    }

    [Benchmark(Baseline = true)]
    public decimal Linq() => UnitPriceLinq.GetUnitPrice(_quantity, _tiers);

    [Benchmark]
    public decimal PriceTable() => _priceTable.GetUnitPrice(_quantity);

    [Benchmark]
    public decimal PriceTableWithConstruction() => new PriceTable(_tiers).GetUnitPrice(_quantity);
}
