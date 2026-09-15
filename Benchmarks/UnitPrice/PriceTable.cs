using Benchmarks.Data;

namespace Benchmarks.UnitPrice;

public class PriceTable
{
    private readonly int[] _minQuantities;
    private readonly decimal[] _unitPrices;

    public PriceTable(List<PriceTier> tiers)
    {
        var sorted = tiers.OrderBy(t => t.MinQuantity).ToList();
        _minQuantities = sorted.Select(t => t.MinQuantity).ToArray();
        _unitPrices = sorted.Select(t => t.UnitPrice).ToArray();
    }

    public decimal GetUnitPrice(int quantity)
    {
        var index = Array.BinarySearch(_minQuantities, quantity);

        if (index < 0)
        {
            index = ~index - 1;
        }

        return _unitPrices[index];
    }
}
