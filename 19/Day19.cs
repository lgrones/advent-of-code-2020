using advent_of_code_2020.lib;

namespace advent_of_code_2020._19;

public class Day19 : IRunnable
{
    private readonly Rule rootRule;
    private readonly List<string> messages = [];

    public Day19(FileReader fileReader)
    {
        var rules = new List<string>();

        foreach (var line in fileReader.ReadFile().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
        {
            if (char.IsDigit(line[0]))
            {
                rules.Add(line);
                continue;
            }

            messages.Add(line);
        }

        rootRule = Graph.Create(rules);
    }

    public object RunPartOne() 
        => messages.Count(Traverse(rootRule).Contains);

    public object RunPartTwo()
    {
        return 0;
    }

    private static List<string> Traverse(Rule rule)
    {
        if (rule.IsLeaf)
            return [rule.literal!.ToString()!];
        
        var result = new List<string>();
        foreach (var subRule in rule.SubRules)
        {
            List<string> substrings = [string.Empty];

            foreach (var ruleNode in subRule)   
            {
                var partialStrings = Traverse(ruleNode);
                var extendedStrings = new List<string>();

                foreach (var substring in substrings)
                {
                    extendedStrings.AddRange(partialStrings.Select(x => substring + x));
                }
                
                substrings = extendedStrings;
            }
            
            result.AddRange(substrings);
        }

        return result;
    }

    private static class Graph
    {
        public static Rule Create(List<string> rules)
        {
            Dictionary<int, List<List<int>>> graph = new();
            List<Rule> ruleNodes = [];

            foreach (var rule in rules)
            {
                var parts = rule.Split(": ");
                var id = int.Parse(parts[0]);

                if (parts[1].Contains('\"'))
                {
                    var literal = char.Parse(parts[1].Replace("\"", string.Empty));
                    ruleNodes.Add(new Rule(id, literal));
                }
                else
                {
                    ruleNodes.Add(new Rule(id, null));

                    var branches = parts[1].Split(" | ");
                    graph[id] = [.. branches.Select(x => x.Split(" ").Select(int.Parse).ToList())];
                }
            }

            foreach (var connection in graph)
            {
                var rule = ruleNodes.Single(x => x.id == connection.Key);

                foreach (var branch in connection.Value)
                {
                    rule.AddSubRules(branch.Select(x => ruleNodes.Single(y => y.id == x)).ToList());
                }
            }

            return ruleNodes.Single(x => x.id == 0);
        }
    }

    private record Rule(int id, char? literal)
    {
        public List<List<Rule>> SubRules { get; } = [];

        public void AddSubRules(List<Rule> rules)
            => SubRules.Add(rules);

        public bool IsLeaf
            => literal is not null;
    }
}