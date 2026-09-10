# Code Example

```cs
record Person(string Name, string Gender, int Age);

public int CountPersonsOver50(IEnumerable<Person> persons)
{
    return persons.Where(x => x.Age > 50).Count();
}
```

## How could we optimize this?

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

## So now the Code is faster no?

Answer: Probably, but we don't know

# BenchmarkDotNet

Short description of workings

## Link to CountPersonsOver50 Benchmark Project

## Result

[Image]

## Result - Huh?

# When to optimize

"Premature Optimization is the root of all evil" - Donald Knut

## No premature optimizations

Optimizing before we know that we actually need to (and potentially spending a lot of time on it)

Code itself may be harder to understand
