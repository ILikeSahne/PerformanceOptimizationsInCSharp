
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
