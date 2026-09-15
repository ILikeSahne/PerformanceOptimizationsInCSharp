Fable 5.1 High - 15.09.2026:
In .net6.0, how could we optimize this (do not change the signature of the function)?

```markdown
record Person(string Name, string Gender, int Age);

public int CountPersonsOver50(IEnumerable<Person> persons)
{
    return persons.Where(x => x.Age > 50).Count();
}
```