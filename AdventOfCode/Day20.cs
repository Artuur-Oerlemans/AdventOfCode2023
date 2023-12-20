using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day20 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(long, long), char> _dict;
    private readonly List<(long, long)> dirs = [(1, 0), (-1, 0), (0, 1), (0, -1)];
    private readonly long maxI;
    private readonly long maxJ;

    public Day20()
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
                if (_input[i][j] != '.')
                {
                    _dict.Add((i, j), _input[i][j]);
                }
            }
        }

    }
    public override ValueTask<string> Solve_1()
    {
        long result = 0;
        Dictionary<string, string> toName = _input
            .Select(x => x.Split(" -> "))
            .ToDictionary(x => x[0].Substring(1), x => x[0]);
        Dictionary<string, bool> flipFlopState = _input
            .Select(x => x.Split(" -> "))
            .Where(x => x[0].StartsWith("%"))
            .ToDictionary(x => x[0], x => false);
        Dictionary<string, Dictionary<string, bool>> conjunctionState = _input
            .Select(x => x.Split(" -> "))
            .Where(x => x[0].StartsWith("&"))
            .ToDictionary(x => x[0], x => new Dictionary<string, bool>());
        Dictionary<string, List<string>> modules = _input
            .Select(x => x.Split(" -> "))
            .ToDictionary(x => x[0], x => x[1].Split(", ").Select(x => { 
                if(toName.ContainsKey(x))
                {
                    return toName[x];
                } else
                {
                    return "not exist";
                }
                    }).ToList());

        foreach ((var mod, var targets) in modules)
        {
            foreach (var target in targets)
            {
                if (target.StartsWith("&"))
                {
                    conjunctionState[target][mod] = false;
                }
            }
        }

        long lowPulse = 0;
        long highPulse = 0;


        for(int i = 0; i< 1000; i++)
        {

            lowPulse++;
            Queue<(string, string, bool)> pulses = new Queue<(string, string, bool)>();
            pulses.Enqueue(("broadcaster", "button", false));

            while (pulses.Count > 0)
            {
                (string mod, string origin, bool freq) = pulses.Dequeue();
                List<string> targets;
                if (mod != "not exist")
                {
                    targets = modules[mod];
                } else
                {
                    targets = new List<string> ();
                }

                if (mod == "broadcaster")
                {
                    foreach (var target in targets)
                    {
                        lowPulse++;
                        pulses.Enqueue((target, mod, freq));
                    }
                }
                else if (mod.StartsWith("&"))
                {
                    conjunctionState[mod][origin] = freq;
                    bool newFreq = !conjunctionState[mod].Values.All(x => x);
                    foreach (var target in targets)
                    {
                        if (newFreq)
                        {
                            highPulse++;
                        }
                        else
                        {
                            lowPulse++;
                        }
                        pulses.Enqueue((target, mod, newFreq));
                    }
                }
                else if (mod.StartsWith("%"))
                {
                    if (!freq)
                    {
                        flipFlopState[mod] = !flipFlopState[mod];
                        foreach (var target in targets)
                        {
                            if (flipFlopState[mod])
                            {
                                highPulse++;
                            }
                            else
                            {
                                lowPulse++;
                            }
                            pulses.Enqueue((target, mod, flipFlopState[mod]));
                        }
                    }
                }
                else if (mod == "not exist")
                {

                } else 
                {
                    throw new Exception();
                }
            }
        }


        return new ValueTask<string>($"Solution to {ClassPrefix} {lowPulse * highPulse}, part 1");
    }

    public override ValueTask<string> Solve_2()
    {
        long result = 0;
        Dictionary<string, string> toName = _input
            .Select(x => x.Split(" -> "))
            .ToDictionary(x => x[0].Substring(1), x => x[0]);
        Dictionary<string, bool> flipFlopState = _input
            .Select(x => x.Split(" -> "))
            .Where(x => x[0].StartsWith("%"))
            .ToDictionary(x => x[0], x => false);
        Dictionary<string, Dictionary<string, bool>> conjunctionState = _input
            .Select(x => x.Split(" -> "))
            .Where(x => x[0].StartsWith("&"))
            .ToDictionary(x => x[0], x => new Dictionary<string, bool>());
        Dictionary<string, List<string>> modules = _input
            .Select(x => x.Split(" -> "))
            .ToDictionary(x => x[0], x => x[1].Split(", ").Select(x => {
                if (toName.ContainsKey(x))
                {
                    return toName[x];
                }
                else if(x == "rx")
                {
                    return "rx";
                } else
                {
                    return "not exist";
                }
            }).ToList());

        foreach ((var mod, var targets) in modules)
        {
            foreach (var target in targets)
            {
                if (target.StartsWith("&"))
                {
                    conjunctionState[target][mod] = false;
                }
            }
        }

        HashSet<string> forBearers = ["&dh", "&mk", "&vf", "&rn"];

        List<long> loops = [];

        for (long i = 1; loops.Count < 4; i++)
        {

            Queue<(string, string, bool)> pulses = new Queue<(string, string, bool)>();
            pulses.Enqueue(("broadcaster", "button", false));

            while (pulses.Count > 0)
            {
                (string mod, string origin, bool freq) = pulses.Dequeue();
                List<string> targets;
                if (modules.ContainsKey(mod))
                {
                    targets = modules[mod];
                }
                else
                {
                    targets = new List<string>();
                }

                if (mod == "broadcaster")
                {
                    foreach (var target in targets)
                    {
                        pulses.Enqueue((target, mod, freq));
                    }
                }
                else if (mod.StartsWith("&"))
                {
                    conjunctionState[mod][origin] = freq;
                    bool newFreq = !conjunctionState[mod].Values.All(x => x);
                    foreach (var target in targets)
                    {
                        if(newFreq && forBearers.Contains(mod))
                        {
                            L(i + " " + mod);
                            loops.Add(i);
                        }
                        pulses.Enqueue((target, mod, newFreq));
                    }
                }
                else if (mod.StartsWith("%"))
                {
                    if (!freq)
                    {
                        flipFlopState[mod] = !flipFlopState[mod];
                        foreach (var target in targets)
                        {
                            pulses.Enqueue((target, mod, flipFlopState[mod]));
                        }
                    }
                }
                else if (mod == "rx")
                {
                    if(!freq)
                    {
                        return new ValueTask<string>($"Solution to {ClassPrefix} {i}, part 2");
                    }
                }
                else if (mod == "not exist")
                {

                }
                else
                {
                    throw new Exception();
                }
            }
        }

        long lcm = 1;

        foreach ( var l in loops )
        {
            lcm = FindLCM(lcm, l);
        }

        return new ValueTask<string>($"Solution to {ClassPrefix} {lcm}, part 2");
    }
    public static long FindLCM(long a, long b)
    {
        long num1, num2;

        if (a > b)
        {
            num1 = a;
            num2 = b;
        }
        else
        {
            num1 = b;
            num2 = a;
        }

        for (long i = 1; i <= num2; i++)
        {
            if ((num1 * i) % num2 == 0)
            {
                return i * num1;
            }
        }
        return num2;
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
}
