The simplest win is to drop the `Where` and use the predicate overload of `Count`:

```csharp
public int CountPersonsOver50(IEnumerable<Person> persons)
{
    return persons.Count(x => x.Age > 50);
}
```

In .NET 6, `Where(...).Count()` allocates a `WhereEnumerableIterator` and then enumerates through that extra iterator layer (two `MoveNext` calls per element). `Count(predicate)` iterates the source directly, so you save one allocation and one level of indirection per element. Same result, same semantics, strictly less work.

If this sits on a hot path and the callers usually pass a `List<Person>` or `Person[]`, you can go further and skip LINQ's interface dispatch entirely:

```csharp
public int CountPersonsOver50(IEnumerable<Person> persons)
{
    int count = 0;

    if (persons is Person[] array)
    {
        foreach (var p in array)           // JIT knows the length, no bounds checks
            if (p.Age > 50) count++;
    }
    else if (persons is List<Person> list)
    {
        foreach (var p in list)            // struct enumerator, no allocation
            if (p.Age > 50) count++;
    }
    else
    {
        foreach (var p in persons)
            if (p.Age > 50) count++;
    }

    return count;
}
```

That removes the delegate call per element and lets the JIT devirtualize the enumeration. Whether that's worth the extra code depends on how often this runs and how large the collections are; for most cases `Count(predicate)` is the right stopping point. One caveat with the manual version: if a `List<Person>` is modified during enumeration you still get the usual `InvalidOperationException`, same as LINQ.

Outside the function itself, the other lever is `Person`: as a `record` (class) each element is a reference and `x.Age` is a pointer chase. If the type is small and immutable, `record struct Person(...)` keeps the data contiguous in an array/list and makes the loop cache-friendlier — but that changes the type, not just this method, so it's a separate decision.