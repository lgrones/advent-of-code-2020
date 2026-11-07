using System.Globalization;
using advent_of_code_2020.lib;

namespace advent_of_code_2020._15;

public class Day15 : IRunnable
{
    private readonly List<int> startingNumbers;

    public Day15(FileReader fileReader)
    {
        startingNumbers = fileReader.ReadFile()
            .Split(',')
            .Select(int.Parse)
            .ToList();
    }

    public object RunPartOne()
        => Play(2020);

    public object RunPartTwo()
        => Play(30_000_000);

    private int Play(int turns)
    {
        var cache = new int[turns];

        for (var i = 0; i < startingNumbers.Count; i++)
        {
            cache[startingNumbers[i]] = i + 1;
        }
        
        var lastNumber = startingNumbers.Last();

        for (var turn = startingNumbers.Count; turn < turns; turn++)
        {
            var lastSeen = cache[lastNumber];
            var nextNumber = lastSeen > 0 ? turn - lastSeen : 0;
            cache[lastNumber] = turn;
            lastNumber = nextNumber;
        }

        return lastNumber;
    }
}