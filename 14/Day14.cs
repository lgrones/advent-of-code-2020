using System.Globalization;
using advent_of_code_2020.lib;

namespace advent_of_code_2020._14;

public class Day14 : IRunnable
{
    private readonly List<string> instructions;

    public Day14(FileReader fileReader)
    {
        instructions = fileReader.ReadFile()
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .ToList();
    }

    public object RunPartOne()
    {
        var memory = new Dictionary<int, long>();
        Mask? mask = null;

        foreach (var instruction in instructions)
        {
            if (instruction.StartsWith("mask"))
            {
                mask = new Mask(instruction);
                continue;
            }

            var memoryValue = new MemoryValue(instruction);
            memory[memoryValue.Location] = mask!.Apply(memoryValue.Value);
        }

        return memory.Values.Sum();
    }

    public object RunPartTwo()
    {
        var memory = new Dictionary<long, long>();
        Decoder? decoder = null;

        foreach (var instruction in instructions)
        {
            if (instruction.StartsWith("mask"))
            {
                decoder = new Decoder(instruction);
                continue;
            }

            var memoryValue = new MemoryValue(instruction);

            foreach (var location in decoder!.Apply(memoryValue.Location))
            {
                memory[location] = memoryValue.Value;
            }
        }

        return memory.Values.Sum();
    }

    private class Mask
    {
        public Mask(string mask)
        {
            var clean = mask.Replace("mask = ", string.Empty);
            ones = long.Parse(clean.Replace('X', '0'), NumberStyles.BinaryNumber);
            zeroes = long.Parse(clean.Replace('X', '1'), NumberStyles.BinaryNumber);
        }

        private readonly long ones;
        private readonly long zeroes;

        public long Apply(long value)
            => (value | ones) & zeroes;
    }

    private record MemoryValue
    {
        public MemoryValue(string value)
        {
            var clean = value.Replace("mem[", string.Empty).Replace("]", string.Empty);
            var split = clean.Split(" = ");

            Location = int.Parse(split[0]);
            Value = long.Parse(split[1]);
        }

        public long Value { get; init; }

        public int Location { get; init; }
    }

    private class Decoder
    {
        public Decoder(string mask)
        {
            var clean = mask.Replace("mask = ", string.Empty);
            // Literal ones will be forced to ones
            ones = long.Parse(clean.Replace('X', '0'), NumberStyles.BinaryNumber);
            // X become ones, the rest 0 to denote floating positions
            floating = long.Parse(clean.Replace('1', '0').Replace('X', '1'), NumberStyles.BinaryNumber);
        }

        private readonly long ones;
        private readonly long floating;

        public List<long> Apply(long value)
        {
            // Force ones to be one and all floating positions to be 0
            var baseValue = (value | ones) & ~floating;
            // Starting from all floating positions at one
            var mask = floating;

            // Keep the base where each floating position is 0
            List<long> addresses = [baseValue];

            do
            {
                // Force all bits that are set in mask to one and keep the value
                addresses.Add(baseValue | mask);
                // Flip the lowest set bit to 0 (mask - 1)
                // and remove all resulting ones after that bit that are not also in the floating mask (& floating)
                mask = (mask - 1) & floating;
                // Essentially counting down by one, skipping all numbers that don't have bits set where floating does
            } while (mask != 0);

            return addresses;
        }
    }
}