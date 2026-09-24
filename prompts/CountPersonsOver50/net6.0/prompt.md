How could I optimize this in .net6.0?

```csharp
public static int WhereCount(IEnumerable<Person> persons)
{
    return persons.Where(x => x.Age > 50).Count();
}
```

It is going to be called with this data:

```csharp
public static List<Person> Generate(int count)
{
    return new Faker<Person>()
        .UseSeed(42)
        .CustomInstantiator(f => new Person(
            f.Name.FullName(),
            f.PickRandom("Male", "Female", "Other"),
            f.Random.Int(1, 100)))
        .Generate(count);
}
```

Person looks like this:

```csharp
public record Person(string Name, string Gender, int Age);
```
