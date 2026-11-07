using advent_of_code_2020.lib;

namespace advent_of_code_2020._18;

public class Day18 : IRunnable
{
    private readonly List<string> problems;

    public Day18(FileReader fileReader)
    {
        problems = fileReader.ReadFile().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public object RunPartOne()
    {
        var shuntingYard = new ShuntingYard(Precedence.ParensLeftToRight);
        return problems.Sum(problem => Evaluator.Evaluate(shuntingYard.Parse(problem)));
    }

    public object RunPartTwo()
    {
        var shuntingYard = new ShuntingYard(Precedence.AdditionBeforeMultiplication);
        return problems.Sum(problem => Evaluator.Evaluate(shuntingYard.Parse(problem)));
    }

    private enum Precedence
    {
        ParensLeftToRight,
        AdditionBeforeMultiplication
    }

    private class ShuntingYard
    {
        private readonly Dictionary<char, int> precedences;

        public ShuntingYard(Precedence precedence)
        {
            precedences = precedence switch
            {
                Precedence.ParensLeftToRight => new Dictionary<char, int> { { '+', 1 }, { '*', 1 } },
                Precedence.AdditionBeforeMultiplication => new Dictionary<char, int> { { '+', 2 }, { '*', 1 } },
                _ => throw new ArgumentOutOfRangeException(nameof(precedence), precedence, null)
            };
        }

        public Queue<char> Parse(string input)
        {
            var output = new Queue<char>();
            var operators = new Stack<char>();

            foreach (var token in input.Replace(" ", string.Empty))
            {
                if (char.IsDigit(token))
                {
                    output.Enqueue(token);
                    continue;
                }

                if (token == '(')
                {
                    operators.Push(token);
                    continue;
                }

                if (token == ')')
                {
                    while (operators.TryPeek(out var op) && op != '(')
                    {
                        output.Enqueue(operators.Pop());
                    }

                    operators.Pop();
                    continue;
                }

                while (operators.TryPeek(out var op) && op != '(' && precedences[op] >= precedences[token])
                {
                    output.Enqueue(operators.Pop());
                }

                operators.Push(token);
            }

            while (operators.TryPop(out var op))
                output.Enqueue(op);

            return output;
        }
    }

    private class Evaluator
    {
        public static long Evaluate(Queue<char> tokens)
        {
            var stack = new Stack<long>();

            while (tokens.TryDequeue(out var token))
            {
                if (char.IsDigit(token))
                {
                    stack.Push(long.Parse(token.ToString()));
                    continue;
                }

                var operands = new List<long>([stack.Pop(), stack.Pop()]);

                switch (token)
                {
                    case '+':
                        stack.Push(operands.Sum());
                        break;
                    case '*':
                        stack.Push(operands.Aggregate((x, y) => x * y));
                        break;
                }
            }

            return stack.Pop();
        }
    }
}