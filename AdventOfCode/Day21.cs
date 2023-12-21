using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day21 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(long, long), char> _dict;
    private readonly List<(long, long)> dirs = [(1, 0), (-1, 0), (0, 1), (0, -1)];
    private readonly long maxI;
    private readonly long maxJ;

    public Day21()
    {
        _input =
        [
            .. File.ReadAllText(string.Concat(InputFilePath.AsSpan(0, InputFilePath.Length - 4)
//, "test"
, ".txt")
                        ).Split("\r\n")
,
        ];

        maxI = _input.Count;
        maxJ = _input[0].Length;
        _dict = [];
        for (int i = 0; i < _input.Count; i++)
        {
            for (int j = 0; j < _input[i].Length; j++)
            {
                _dict.Add((i, j), _input[i][j]);
            }
        }

    }
    public override ValueTask<string> Solve_1()
    {
        long result = 0;
        var start = _dict.Where(x => x.Value == 'S').Select(x => x.Key).Single();

        HashSet<(long, long)> plotsNow = [];
        plotsNow.Add(start);
        for (int step = 0; step < 64; step++)
        {
            HashSet<(long, long)> plotsFuture = [];
            foreach(var p in plotsNow)
            {
                foreach(var dir in dirs)
                {
                    var newP = Add(p, dir);
                    if(_dict.ContainsKey(newP) && _dict[newP] != '#')
                    {
                        plotsFuture.Add(newP);
                    }
                }
            }
            plotsNow = plotsFuture;
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {plotsNow.Count}, part 1");
    }
    public override ValueTask<string> Solve_2()
    {
        long result = 0;
        var start = _dict.Where(x => x.Value == 'S').Select(x => x.Key).Single();

        HashSet<(long, long)> plotsNow = [];
        List<long> plotCounts = [0];
        plotsNow.Add(start);
        long goalTot = 16733044;
        long sampleSteps = (goalTot % maxI) + maxI * 3;
        for (int step = 0; step < sampleSteps; step++)
        {
            HashSet<(long, long)> plotsFuture = [];
            foreach (var p in plotsNow)
            {
                foreach (var dir in dirs)
                {
                    var newP = Add(p, dir);
                    var modP = Mod(newP);
                    if (_dict[modP] != '#')
                    {
                        plotsFuture.Add(newP);
                    }
                }
            }
            plotsNow = plotsFuture;
            L(step + " " + plotsNow.Count);
            plotCounts.Add(plotsNow.Count);
        }

        long stepC = sampleSteps;
        long plots = plotCounts[(int)stepC];

        long defaultIncrease = plots - plotCounts[(int)(stepC - maxI)];
        long defaultIncreaseMin = plotCounts[(int)(stepC - maxI)] - plotCounts[(int)(stepC - 2 * maxI)];

        long metaIncrease = defaultIncrease - defaultIncreaseMin;

        int d = 1;
        while(stepC < goalTot)
        {
            plots += defaultIncrease + metaIncrease * d;
            d++;
            stepC += maxI;
        }

        return new ValueTask<string>($"Solution to {ClassPrefix} {plots}, part 2");
    }

    public ValueTask<string> Solve_2_old()
    {
        long result = 0;
        var start = _dict.Where(x => x.Value == 'S').Select(x => x.Key).Single();

        Dictionary<(long, long), HashSet<(long, long)>> plotsNow = [];
        plotsNow.Add(start, new HashSet<(long, long)>{ (0, 0) });
        for (int step = 0; step < 10; step++)
        {
            Dictionary<(long, long), HashSet<(long, long)>> plotsFuture = [];
            foreach (var p in plotsNow)
            {
                foreach (var dir in dirs)
                {
                    var newP = Add(p.Key, dir);
                    var modP = Mod(newP);
                    var multi = p.Value;
                    if (_dict[modP] != '#')
                    {
                        if (!_dict.ContainsKey(newP))
                        {
                            if(newP.Item1 >= maxI)
                            {
                                multi = multi.Select(x => (x.Item1 + 1, x.Item2)).ToHashSet();
                            } else if (newP.Item2 >= maxJ)
                            {
                                multi = multi.Select(x => (x.Item1, x.Item2 + 1)).ToHashSet();
                            } else if (newP.Item1 < 0)
                            {
                                multi = multi.Select(x => (x.Item1 - 1, x.Item2)).ToHashSet();
                            } else if (newP.Item2 < 0)
                            {
                                multi = multi.Select(x => (x.Item1, x.Item2 - 1)).ToHashSet();
                            } else
                            {
                                throw new Exception();
                            }
                        }

                        if (plotsFuture.ContainsKey(modP))
                        {
                            plotsFuture[modP].UnionWith(multi);
                        }
                        else
                        {
                            plotsFuture[modP] = multi.ToHashSet();
                        }
                    }
                }
            }
            plotsNow = plotsFuture;
        }

        result = plotsNow.Select(x => (long)x.Value.Count).Sum();
        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 2");
    }

    private string DictToString(Dictionary<(int, int), char> dict)
    {
        string r = "";
        for (int i = 0; i < maxI; i++)
        {
            for (int j = 0; j < maxJ; j++)
            {
                if (dict.ContainsKey((i, j)))
                {
                    r += dict[(i, j)];
                }
                else
                {
                    r += '.';
                }
            }
            r += "\r\n";
        }
        return r;
    }

    private String LocationSetToString(HashSet<(int, int)> energized)
    {
        string r = "";
        for (int i = 0; i < maxI; i++)
        {
            for (int j = 0; j < maxJ; j++)
            {
                if (energized.Contains((i, j)))
                {
                    r += "#";
                }
                else
                {
                    r += '.';
                }
            }
            r += "\r\n";
        }
        return r;
    }

    private void L(Object o)
    {
        Console.WriteLine(o.ToString());
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

    private (long, long) Add((long, long) a, (long, long) b)
    {
        return (a.Item1 + b.Item1, a.Item2 + b.Item2);
    }

    private (long, long) Mul((long, long) a, long x)
    {
        return (a.Item1 * x, a.Item2 * x);
    }

    private long ManDis((long, long) a, (long, long) b)
    {
        return Math.Abs(a.Item1 - a.Item1) + Math.Abs(b.Item2 - b.Item2);
    }

    private (long, long) Neg((long, long) p)
    {
        return (-p.Item1, -p.Item2);
    }

    private (long, long) Mod((long, long) p)
    {
        return ((p.Item1 % maxI + maxI) % maxI, (p.Item2 % maxJ + maxJ) % maxJ);
    }
}
