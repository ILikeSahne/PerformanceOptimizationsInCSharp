using Bogus;

namespace Benchmarks.CountPersons;

public static class PersonFaker
{
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
}
