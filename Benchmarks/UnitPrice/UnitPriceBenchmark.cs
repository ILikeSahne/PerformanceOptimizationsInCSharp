using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarks.UnitPrice;

public class UnitPriceBenchmark
{
    private List<PriceTier> _tiers = null!;
    private List<PriceTier> _tiersSortedAscending = null!;
    private List<PriceTier> _tiersSortedDescending = null!;
    private PriceTable _priceTable = null!;
    private int _quantity;

    [Params(3, 10, 100)]
    public int TierCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _tiers = Enumerable.Range(0, TierCount)
            .Select(i => new PriceTier(i * 10 + 1, 100m - i * 0.05m))
            .ToList();

        _tiersSortedAscending = _tiers.OrderBy(t => t.MinQuantity).ToList();
        _tiersSortedDescending = _tiers.OrderByDescending(t => t.MinQuantity).ToList();

        _priceTable = new PriceTable(_tiers);

        _quantity = (TierCount / 2) * 10 + 5;
    }

    [Benchmark(Baseline = true)]
    public decimal Linq() => UnitPriceLinq.GetUnitPrice(_quantity, _tiers);

    [Benchmark]
    public decimal SortedForLoop() => UnitPriceSorted.GetUnitPrice(_quantity, _tiersSortedAscending);

    [Benchmark]
    public decimal PriceTable() => _priceTable.GetUnitPrice(_quantity);
}
