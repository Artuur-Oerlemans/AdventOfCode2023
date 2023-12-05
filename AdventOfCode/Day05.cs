using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day05 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(int, int), string> _dict = new Dictionary<(int, int), string>();

    public Day05()
    {
        _input =
        [
            .. File.ReadAllText(string.Concat(InputFilePath.AsSpan(0, InputFilePath.Length - 4)
//, "test"
, ".txt")
                        ).Split("\r\n\r\n")
,
        ];
    }
    public override ValueTask<string> Solve_1()
    {
        var seeds = _input[0]
            .Split(": ")[1]
            .Split(" ")
            .Select(long.Parse)
            .ToList();
        var dic = _input.GetRange(1, _input.Count - 1)
            .Select(s => s.Split("\r\n"))
            .Select(s => s.Skip(1))
            .Select(e => e.Select(s => s.Split(' '))
            .Select(l => (long.Parse(l[0]), long.Parse(l[1]), long.Parse(l[2])))
            ).ToList();
        var final = new List<long>();
        foreach (var seed in seeds)
        {
            long source = seed;
            foreach (var step in dic)
            {
                var reached = false;
                foreach (var pos in step)
                {
                    if (source >= pos.Item2 && source < pos.Item2 + pos.Item3)
                    {
                        source = pos.Item1 + source - pos.Item2;
                        reached = true;
                        break;
                    }
                }
                if (!reached)
                {
                    L(seed);
                }
            }
            final.Add(source);
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {final.Min()}, part 1");
    }

    long lowest_loc = long.MaxValue;
    List<IOrderedEnumerable<(long, long, long)>> dics;
    public override ValueTask<string> Solve_2()
    {
        var seeds_inp = _input[0]
            .Split(": ")[1]
            .Split(" ")
            .Select(long.Parse)
            .ToList();
        var seed_ranges = new List<(long, long)>();
        for (int i = 0; i < seeds_inp.Count; i += 2)
        {
            seed_ranges.Add((seeds_inp[i], seeds_inp[i] + seeds_inp[i + 1]));
        }
        dics = _input.GetRange(1, _input.Count - 1)
            .Select(s => s.Split("\r\n"))
            .Select(s => s.Skip(1))
            .Select(e => e.Select(s => s.Split(' '))
                          .Select(l => (long.Parse(l[0]), long.Parse(l[1]), long.Parse(l[2])))
                          .OrderBy(x => x.Item2)
            )
            .ToList();
        seed_ranges.ForEach(x => UseSeeds(x, 0));
        return new ValueTask<string>($"Solution to {ClassPrefix} {lowest_loc}, part 2");
    }

    public void UseSeeds((long, long) range, int step)
    {
        if (step == dics.Count)
        {
            if (range.Item1 < lowest_loc)
            {
                lowest_loc = range.Item1;
            }
            return;
        }
        var s = dics[step].ToList();

        for(int i = 0; i < s.Count; i++)
        {
            if (range.Item1 < s[i].Item2)
            {
                var temp = (range.Item1, Math.Min(range.Item2, s[i].Item2));
                UseSeeds(temp, step + 1);
                range.Item1 = Math.Min(range.Item2, s[i].Item2);
                if (range.Item1 == range.Item2)
                {
                    return;
                }
            }
            if(range.Item1 < s[i].Item2 + s[i].Item3)
            {
                var temp = (range.Item1, Math.Min(range.Item2, s[i].Item2 + s[i].Item3));
                UseSeeds((temp.Item1 + s[i].Item1 - s[i].Item2, temp.Item2 + s[i].Item1 - s[i].Item2), step + 1);
                range.Item1 = temp.Item2;
                if (range.Item1 == range.Item2)
                {
                    return;
                }
            }
        }
        if(range.Item1 != range.Item2)
        {
            UseSeeds(range, step + 1);
        }
    }

    public  ValueTask<string> Solve_2_old()
    {
        var seeds_inp = _input[0]
            .Split(": ")[1]
            .Split(" ")
            .Select(long.Parse)
            .ToList();
        var dic = _input.GetRange(1, _input.Count - 1)
            .Select(s => s.Split("\r\n"))
            .Select(s => s.Skip(1))
            .Select(e => e.Select(s => s.Split(' '))
            .Select(l => (long.Parse(l[0]), long.Parse(l[1]), long.Parse(l[2])))
            ).ToList();
        long lowest = long.MaxValue;
        for (int i = 0; i < seeds_inp.Count; i += 2)
        {
            for (long j = seeds_inp[i]; j < seeds_inp[i] + seeds_inp[i + 1]; j++)
            {
                var source = j;
                foreach (var step in dic)
                {
                    var reached = false;
                    foreach (var pos in step)
                    {
                        if (source >= pos.Item2 && source < pos.Item2 + pos.Item3)
                        {
                            source = pos.Item1 + source - pos.Item2;
                            reached = true;
                            break;
                        }
                    }
                }
                if ( source < lowest )
                {
                    lowest = source;
                }
            }
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {lowest}, part 2");
    }

    private void L(Object o)
    {
        Console.WriteLine(o.ToString());
    }

    private (int, int) Add((int, int) a, (int, int) b)
    {
        return (a.Item1 + b.Item1, a.Item2 + b.Item2);
    }

    private void RegexYouKnow()
    {
        string temps =
@"move 1 to A
move 2 to B
move 3 to C";
        Regex re = new(@"^move (\d) to ([A-Z])$");
        temps.Split("\r\n")
            .Select(s => re.Match(s).Groups)
            .Select(g => (int.Parse(g[1].Value), g[2].Value))
            .ToList().ForEach(t => Console.WriteLine(t.Item2));
    }
}
