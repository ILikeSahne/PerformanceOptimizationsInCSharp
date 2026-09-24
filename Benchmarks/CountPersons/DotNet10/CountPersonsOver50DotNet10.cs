namespace Benchmarks.CountPersons;

public static class CountPersonsOver50DotNet10
{
    public static int WhereCount(IEnumerable<Person> persons)
    {
        return persons.Where(x => x.Age > 50).Count();
    }

    public static int Count(IEnumerable<Person> persons)
    {
        return persons.Count(x => x.Age > 50);
    }

    public static int ForEach(IEnumerable<Person> persons)
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
}
