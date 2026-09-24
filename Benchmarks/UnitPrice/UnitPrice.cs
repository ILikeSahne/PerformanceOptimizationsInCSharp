namespace Benchmarks.UnitPrice;

public static class UnitPriceLinq
{
    public static decimal GetUnitPrice(int quantity, List<PriceTier> tiers)
    {
        return tiers
            .Where(t => quantity >= t.MinQuantity)
            .MaxBy(t => t.MinQuantity)!
            .UnitPrice;
    }
}

public static class UnitPriceSorted
{
    public static decimal GetUnitPrice(int quantity, List<PriceTier> tiersSortedDescending)
    {
        for (var i = 0; i < tiersSortedDescending.Count; i++)
        {
            if (quantity >= tiersSortedDescending[i].MinQuantity)
            {
                return tiersSortedDescending[i].UnitPrice;
            }
        }

        throw new InvalidOperationException("No matching tier");
    }
}

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
