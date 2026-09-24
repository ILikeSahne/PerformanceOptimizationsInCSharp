A few levels, from cheapest change to fastest:

**1. Fold the predicate into `Count`**

```csharp
return persons.Count(x => x.Age > 50);
```

Removes the intermediate `WhereEnumerableIterator` allocation and one layer of enumerator calls. Small but free.

**2. Take advantage of the concrete type**

Your caller hands you a `List<Person>`, but the method only sees `IEnumerable<Person>`, so every `MoveNext`/`Current` goes through interface dispatch on a boxed enumerator, plus a delegate call per element. Either change the signature, or special-case it:

```csharp
public static int WhereCount(IEnumerable<Person> persons)
{
    if (persons is List<Person> list)
        return CountOver50(CollectionsMarshal.AsSpan(list));

    if (persons is Person[] array)
        return CountOver50(array);

    return persons.Count(x => x.Age > 50);
}

private static int CountOver50(ReadOnlySpan<Person> span)
{
    int count = 0;
    foreach (var p in span)
        if (p.Age > 50) count++;
    return count;
}
```

`CollectionsMarshal.AsSpan` (System.Runtime.InteropServices, available since .NET 5) gives you the list's backing array with no copy. The span loop is a plain bounds-check-eliminated `for` with no delegate and no interface calls — typically several times faster than the LINQ version for this size of work. Caveat: don't mutate the list while holding the span.

If you can change the signature to `List<Person>` or `ReadOnlySpan<Person>` directly, do that instead and drop the type check.

**3. Data layout, if this is really hot**

`Person` is a reference type, so the loop chases a pointer per element to read `Age`. If this query dominates and you control the model, keeping ages in a parallel `int[]` (or making `Person` a small `readonly record struct`) makes the loop cache-friendly and lets you go further with `Vector<int>` compare-and-count in .NET 6. That's a bigger change than the question implies, so only worth it with a profiler pointing at it.

**4. Don't recompute**

If `WhereCount` is called repeatedly on the same generated list, cache the result or maintain the count incrementally — no loop beats not looping.

To see the actual numbers on your machine, put the variants in a BenchmarkDotNet project with `[Params(1_000, 100_000)]` on `count`; the gap between 1 and 2 is usually the surprising one.
