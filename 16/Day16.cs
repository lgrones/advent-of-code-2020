using advent_of_code_2020.lib;

namespace advent_of_code_2020._16;

public class Day16 : IRunnable
{
    private readonly List<TicketField> ticketFields = [];
    private readonly List<List<int>> tickets = [];
    private readonly List<int> myTicket = [];

    public Day16(FileReader fileReader)
    {
        var section = 1;
        foreach (var line in fileReader.ReadFile().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
        {
            if (line.StartsWith("your") || line.StartsWith("nearby"))
            {
                section++;
                continue;
            }

            switch (section)
            {
                case 1:
                    ticketFields.Add(new TicketField(line));
                    continue;
                case 2:
                    myTicket = [.. line.Split(',').Select(int.Parse)];
                    continue;
                default:
                    tickets.Add([.. line.Split(',').Select(int.Parse)]);
                    break;
            }
        }
    }

    public object RunPartOne()
        => tickets.Sum(ticket
            => ticket.Where(number
                    => !ticketFields.Any(x => x.IsInRange(number)))
                .Sum());

    public object RunPartTwo()
    {
        var validTickets = tickets.Where(x
                => x.All(y => ticketFields.Any(z => z.IsInRange(y))))
            .Concat<List<int>>([myTicket]).ToList();

        var fieldMap = new Dictionary<int, List<string>>();

        for (var i = 0; i < validTickets.First().Count; i++)
        {
            var fields = ticketFields.Where(x => validTickets.All(y => x.IsInRange(y[i])))
                .Select(x => x.Name)
                .ToList();

            fieldMap.Add(i, fields);
        }

        var assigned = new HashSet<string>(fieldMap.Values.Where(x => x.Count == 1).SelectMany(x => x));
        while (fieldMap.Values.Any(x => x.Count > 1))
        {
            foreach (var (key, _) in fieldMap.Where(x => x.Value.Count > 1))
            {
                fieldMap[key].RemoveAll(assigned.Contains);
            }
            
            foreach (var specified in fieldMap.Values.Where(x => x.Count == 1).SelectMany(x => x))
            {
                assigned.Add(specified);
            }
        }

        return fieldMap.Where(x
                => x.Value.First().StartsWith("departure"))
            .Select(x => (long)myTicket[x.Key])
            .Aggregate((acc, curr) => acc * curr);
    }


    private class TicketField
    {
        public string Name { get; }
        private readonly List<(int min, int max)> ranges;

        public TicketField(string line)
        {
            var split = line.Split(": ");
            Name = split[0];
            ranges = split[1].Split(" or ").Select(x =>
            {
                var extremes = x.Split("-").Select(int.Parse).ToArray();
                return (extremes[0], extremes[1]);
            }).ToList();
        }

        public bool IsInRange(int number)
            => ranges.Any(x => x.min <= number && number <= x.max);
    }
}