namespace Benchmarks.CountPersons;

public static class CountPersonsOver50List
{
    public static int WhereCount(List<Person> persons)
    {
        return persons.Where(x => x.Age > 50).Count();
    }

    public static int Count(List<Person> persons)
    {
        return persons.Count(x => x.Age > 50);
    }

    public static int ForEach(List<Person> persons)
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
