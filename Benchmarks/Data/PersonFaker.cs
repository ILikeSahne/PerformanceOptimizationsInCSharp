using Bogus;

namespace Benchmarks.Data;

public static class PersonFaker
{
    public static IEnumerable<Person> Generate(int count)
    {
        return new Faker<Person>()
            .UseSeed(42)
            .CustomInstantiator(f => new Person(
                f.Name.FullName(),
                f.PickRandom("Male", "Female", "Other"),
                f.Random.Int(1, 100)))
            .Generate(count);
    }
}
