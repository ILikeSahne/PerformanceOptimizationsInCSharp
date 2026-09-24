# Intro

## Unit Price Example

You get a Story assigned to implement variable pricing for a product depending on the quantity ordered.

Example:
- > 1.000 units: 0.50€ per unit
- > 10.000 units: 0.40€ per unit
- > 100.000 units: 0.30€ per unit
- ...

### Linq

```cs
record PriceTier(int MinQuantity, decimal UnitPrice);

public decimal GetUnitPrice(int quantity, List<PriceTier> tiers)
{
    return tiers
        .Where(t => quantity >= t.MinQuantity)
        .MaxBy(t => t.MinQuantity)!
        .UnitPrice;
}
```

### PriceTable with BinarySearch

```cs
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
```

### Runtime changes

Got the Code down from O(n) to O(log n) but the code is more complex now.

PO comes to you and tells you that this is only gonna be used with 3 tiers and only for special, high volume customers.
--> No real need to optimize this, the Linq version is good enough and easier to read

Also we now introduced a possible failure for other developers and they might directly use `new PriceTable(tiers).GetUnitPrice(quantity)` removing all the optimizations we did.

## When to optimize

"Premature Optimization is the root of all evil" - Donald Knuth

### No premature optimizations

Optimizing before we know that we actually need to (and potentially spending a lot of time on it)

Code itself may be harder to understand

## C# vs Linq

```cs
record Person(string Name, string Gender, int Age);

public int CountPersonsOver50(IEnumerable<Person> persons)
{
    return persons.Where(x => x.Age > 50).Count();
}
```

How could we optimize this?

### Visual Studio already shows us:

![](results/WhereCountVsCountVsForeach/visualstudio/ShowsHintForOnlyCount.png)

![](results/WhereCountVsCountVsForeach/visualstudio/SimplifyHint.png)

```cs
public int CountPersonsOver50(IEnumerable<Person> persons)
{
    return persons.Count(x => x.Age > 50);
}
```

```cs
public int CountPersonsOver50(IEnumerable<Person> persons)
{
    var count = 0;

    foreach (var person in persons)
    {
        if (person.Age > 50)
        {
            count++;
        }
    }

    return count;
}
```

### So now the Code is faster no?

Answer: Probably, but we don't know

## BenchmarkDotNet

Short description of workings

### WhereCountVsCountVsForEach

```cs
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net60)]
public class CountPersonsOver50Benchmark
{
    private IEnumerable<Person> _persons = null!;

    [GlobalSetup]
    public void Setup()
    {
        _persons = PersonFaker.Generate(10_000);
    }

    [Benchmark(Baseline = true)]
    public int WhereCount() => CountPersonsOver50.WhereCount(_persons);

    [Benchmark]
    public int Count() => CountPersonsOver50.Count(_persons);

    [Benchmark]
    public int ForEach() => CountPersonsOver50.ForEach(_persons);
}
```

### Result

![](results/WhereCountVsCountVsForeach/ienumerable/benchmark.png)

Explanation how to read the result.

Ok, so now we have actually proofed that our optimization works (cut off, act confused).
... But wait, WhereCount was actually faster?

### Result - Huh?

Huh, why did that happen?

### Result - Trick

1. Used .net6.0, newer versions of .net highly optimized Linq using Spans in the background
2. Used IEnumerable which is a generic type and has to be handled in a generic way, so the compiler can't optimize it as good as it could with a List or an Array (only works like this in older .net versions)

### Result with .net10.0

![](results/WhereCountVsCountVsForeach/dotnet10/benchmark.png)

### Result with List

![](results/WhereCountVsCountVsForeach/list/benchmark.png)

## Optimization is Hard

You have to measure it, relying on intuition is not enough.

### First Problem vs Fable

Fable 5.1 High - 15.09.2026
Prompt:
In .net6.0, how could we optimize this (do not change the signature of the function)?
Also guess the speedup.

```markdown
record Person(string Name, string Gender, int Age);

public int CountPersonsOver50(IEnumerable<Person> persons)
{
    return persons.Where(x => x.Age > 50).Count();
}
```

### Result (Summarized)

Use Count directly (~1.2x - 1.5x)
Check if IEnumerabel is a List or an Array and then loop over it knowing that it is a List / an Array

### Results (Benchmark)

![](results/WhereCountVsCountVsForeach/fable/benchmark.png)

Solution with TypeSwitch is not bad in .net6.0.
Sadly the model outputs the same optimization suggestion for .net10.0 where Count and TypeSwitch has about the same performance with TypeSwitch being more complex than a simple Linq.

# Datastructures

## Struct vs Class

You have a PC somewhere at a customer. It crashes sometimes and you are not sure why.
Your idea is that the PC gets too hot, so you start measuring the temperature of the CPU for each Core and report the average tempature of each Phase of the Workflow.

### Loading

```cs
public record TemperatureReading(Phase Phase, double Celsius);

public List<TemperatureReading> Load(string[] lines)
{
    var readings = new List<TemperatureReading>();

    foreach (var line in lines)
    {
        var parts = line.Split(';');

        readings.Add(new TemperatureReading(
            Enum.Parse<Phase>(parts[0]),
            double.Parse(parts[1], CultureInfo.InvariantCulture)));
    }

    return readings;
}
```

### Dictionary

```cs
public Dictionary<Phase, int> Linq(List<TemperatureReading> readings)
{
    var averages = readings
        .GroupBy(r => r.Phase)
        .ToDictionary(g => g.Key, g => g.Average(r => r.Celsius));

    return readings
        .Where(r => r.Celsius > averages[r.Phase] + Spike.Threshold)
        .GroupBy(r => r.Phase)
        .ToDictionary(g => g.Key, g => g.Count());
}
```

You might write something like this as your first solution.
(The dictionary is already created in the correct format, is done this way to better compare benchmarks)

This uses the dictionary directly for Lookups and looks pretty good.

Taking a closer look at how the Enum is defined, we can see that it only uses the numbers 0 to 5.

```cs
public enum Phase
{
    Startup = 0,
    Import = 1,
    Processing = 2,
    Export = 3,
    Idle = 4,
    Shutdown = 5,
}
```

With that knowledge we can get rid of the Dictionary and use a simple Array instead.

### AggregateBy

```cs
public Dictionary<Phase, int> FastLinq(List<TemperatureReading> readings)
{
    var averages = readings
        .AggregateBy(
            r => r.Phase,
            (Sum: 0.0, Count: 0),
            (total, r) => (total.Sum + r.Celsius, total.Count + 1))
        .ToDictionary(kv => kv.Key, kv => kv.Value.Sum / kv.Value.Count);

    return readings
        .Where(r => r.Celsius > averages[r.Phase] + Spike.Threshold)
        .CountBy(r => r.Phase)
        .ToDictionary();
}
```

Now there is no need for Dictionary Lookups, the Phase can be used directly as an index into the array.

![](results/CoreTemperature/DictionaryVsArray/benchmark.png)

That made it 20% faster, not bad and it even uses 10% less memory now.
For now it seems like this is all we can do here, lets look at the loading part, maybe we can optimize that too.

### Struct

So currently it uses a class, but the data is immutable and small, this sounds like a perfect use case for a struct.

```cs
public readonly record struct TemperatureReadingStruct(Phase Phase, double Celsius);

public TemperatureReadingStruct[] LoadStructs(string[] lines)
{
    var readings = new List<TemperatureReadingStruct>();

    foreach (var line in lines)
    {
        var parts = line.Split(';');

        readings.Add(new TemperatureReadingStruct(
            Enum.Parse<Phase>(parts[0]),
            double.Parse(parts[1], CultureInfo.InvariantCulture)));
    }

    return readings.ToArray();
}

public Dictionary<Phase, int> Struct(TemperatureReadingStruct[] readings)
{
    var sums = new Dictionary<Phase, double>();
    var counts = new Dictionary<Phase, int>();

    foreach (var reading in readings)
    {
        sums[reading.Phase] = sums.GetValueOrDefault(reading.Phase) + reading.Celsius;
        counts[reading.Phase] = counts.GetValueOrDefault(reading.Phase) + 1;
    }

    var averages = sums.ToDictionary(s => s.Key, s => s.Value / counts[s.Key]);

    var spikes = new Dictionary<Phase, int>();

    foreach (var reading in readings)
    {
        if (reading.Celsius > averages[reading.Phase] + Spike.Threshold)
        {
            spikes[reading.Phase] = spikes.GetValueOrDefault(reading.Phase) + 1;
        }
    }

    return spikes;
}
```

![](results/CoreTemperature/ClassVsStruct/benchmark.png)

Oh, wow almost 50% faster, that is a huge improvement. The memory usage is about the same, but the GC has to do less work now.

But we still use a lot of memory, although, where does this even come from? The List and the Array can't be that big.
readings: 100_000 x 16 bytes = 1.6 MB
readings.ToArray(): 100_000 x 16 bytes = 1.6 MB

That is 3.2 MB out of ~18 MB, where does the rest come from?

## Perf View

Using [EventPipeProfiler(EventPipeProfile.GcVerbose)], we can collect a trace of the application and analyze it with PerfView.

### Span Split

```cs
public TemperatureReadingStruct[] LoadStructs(string[] lines)
{
    var readings = new List<TemperatureReadingStruct>();
    Span<Range> parts = stackalloc Range[2];

    foreach (var line in lines)
    {
        var span = line.AsSpan();
        span.Split(parts, ';');

        readings.Add(new TemperatureReadingStruct(
            Enum.Parse<Phase>(span[parts[0]]),
            double.Parse(span[parts[1]], CultureInfo.InvariantCulture)));
    }

    return readings.ToArray();
}
```

![](results/CoreTemperature/SpanSplit/benchmark.png)

Wow another 20% faster and now we are at only 5.5 MB, that is a huge improvement.



## Immutable Datstructures

## String Builder (vs string.Create)