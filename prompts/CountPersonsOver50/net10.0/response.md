The LINQ version is already fairly good on .NET 9/10 (`Where(...).Count()` is special-cased so it doesn't materialize anything), but there's still overhead you can remove: the `Where` iterator allocation, the delegate invocation per element, and — because the parameter is `IEnumerable<Person>` — interface-dispatched enumeration if the code path doesn't get specialized.

Since you know it's called with a `List<Person>`, the fastest general approach is to detect the concrete type and loop over a span:

```csharp
using System.Runtime.InteropServices;

public static int WhereCount(IEnumerable<Person> persons)
{
    if (persons is List<Person> list)
        return CountOver50(CollectionsMarshal.AsSpan(list));

    if (persons is Person[] array)
        return CountOver50(array);

    int count = 0;
    foreach (var p in persons)
        if (p.Age > 50) count++;
    return count;
}

private static int CountOver50(ReadOnlySpan<Person> span)
{
    int count = 0;
    foreach (var p in span)
        if (p.Age > 50) count++;
    return count;
}
```

Or, if you can change the signature, just accept `List<Person>` / `ReadOnlySpan<Person>` directly and skip the type checks. The span loop has no allocation, no delegate call, no bounds checks after JIT, and no enumerator versioning checks. Expect roughly 2–4x over the LINQ version on typical sizes; measure with BenchmarkDotNet to confirm on your machine.

Two things worth knowing:

* The remaining cost is memory, not compute. `Person` is a `record` (a class), so each `p.Age` is a pointer chase to a separate heap object. With Bogus-generated data the objects are allocated sequentially, so locality is decent, but a loop over a class array can't be vectorized. If you actually control the type and this is hot, `readonly record struct Person` (or keeping ages in a separate `int[]`) makes the same loop SIMD-friendly and several times faster again. That's a bigger refactor, so probably out of scope for a micro-optimization.
* `CollectionsMarshal.AsSpan` is safe as long as the list isn't modified while you iterate — fine for a pure count.

If you want to stay with LINQ and just trim one allocation, `persons.Count(x => x.Age > 50)` is the idiomatic form, but don't assume it's faster than `Where().Count()` on a `List` — the `Where` path is type-specialized for lists, `Count(predicate)` may not be. Benchmark both if you care about that comparison.
