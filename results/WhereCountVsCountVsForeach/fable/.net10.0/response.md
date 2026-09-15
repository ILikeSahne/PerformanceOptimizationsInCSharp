Two levels, depending on how far you want to go.

**1. Drop the `Where` — use the predicate overload of `Count`**

```csharp
public int CountPersonsOver50(IEnumerable<Person> persons)
    => persons.Count(x => x.Age > 50);
```

`Where(...).Count()` allocates a `WhereEnumerableIterator` object and goes through `MoveNext`/`Current` per element; `Count(predicate)` is a plain `foreach` with a delegate call and no intermediate iterator. The non-capturing lambda is cached as a static delegate, so nothing is allocated per call. This is the change I'd actually make in most code.

**2. Bypass LINQ and the delegate entirely, with a span fast path for the common concrete types**

```csharp
using System.Runtime.InteropServices;

public int CountPersonsOver50(IEnumerable<Person> persons)
{
    if (persons is Person[] array)
        return CountOver50(array);
    if (persons is List<Person> list)
        return CountOver50(CollectionsMarshal.AsSpan(list));

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

This removes the indirect delegate call per element and lets the JIT inline the comparison, bounds-check-eliminate, and (for the span path) run a tight loop over contiguous memory. For arrays/lists of a few thousand items, expect roughly 3–5× over the original; for other `IEnumerable<Person>` sources the gain is smaller, since the interface-based enumeration dominates.

Two things to keep in mind with version 2:

- LINQ throws `ArgumentNullException` for a null `persons`; the hand-rolled version throws `NullReferenceException` at the `foreach`. Add an explicit `ArgumentNullException.ThrowIfNull(persons)` if you want identical behavior.
- If the data is a coin flip around 50, the branch is unpredictable. `count += p.Age > 50 ? 1 : 0` sometimes coaxes the JIT into a branchless `cmov`; worth a BenchmarkDotNet run on your real distribution rather than assuming.

Outside the function itself: since `Person` is a class, each `.Age` read chases a pointer. If you control the type and it's hot, a `readonly record struct` (or a struct-of-arrays layout) would give a larger win than anything inside this method — but that's a change to the type, not the signature.