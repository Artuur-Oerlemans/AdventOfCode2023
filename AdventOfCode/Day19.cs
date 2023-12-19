using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day19 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(long, long), char> _dict;
    private readonly List<(long, long)> dirs = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    public Day19()
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
        long result = 0;
        var workflows = _input[0].Split("\r\n").Select(x => x.Substring(0, x.Length - 1))
            .Select(x => x.Split("{"))
            .ToDictionary(x => (x[0]), x => (x[1].Split(",").ToList()));
        var objects = _input[1].Split("\r\n").Select(x => x.Substring(1, x.Length - 2))
            .Select(x => x.Split(",").ToDictionary(s => (s.Split("=")[0]), s => (long.Parse(s.Split("=")[1]))))
            .ToList();

        for (int i = 0; i < objects.Count; i++)
        {
            string workId = "in";
            var obj = objects[i];

            while (workId != "R" && workId != "A")
            {
                foreach (string step in workflows[workId])
                {
                    if (step.Contains(":"))
                    {
                        var temp = step.Split(":");
                        string newWorkId = temp[1];
                        string ins = temp[0];
                        if (ins.Contains("<"))
                        {
                            var values = ins.Split("<");
                            if (obj[values[0]] < long.Parse(values[1]))
                            {
                                workId = newWorkId;
                                break;
                            }
                        }
                        else
                        {
                            var values = ins.Split(">");
                            if (obj[values[0]] > long.Parse(values[1]))
                            {
                                workId = newWorkId;
                                break;
                            }
                        }
                    }
                    else
                    {
                        workId = step;
                        break;
                    }
                }
            }
            if (workId == "A")
            {
                result += obj["x"] + obj["m"] + obj["a"] + obj["s"];
            }
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 1");
    }

    public override ValueTask<string> Solve_2()
    {
        long result = 0;
        var workflows = _input[0].Split("\r\n").Select(x => x.Substring(0, x.Length - 1))
            .Select(x => x.Split("{"))
            .ToDictionary(x => (x[0]), x => (x[1].Split(",").ToList()));

        Queue<(string, Dictionary<string, (long, long)>)> objects = [];
        objects.Enqueue(("in", new Dictionary<string, (long, long)> { { "x", (1, 4000) }, { "m", (1, 4000) }, { "a", (1, 4000) }, { "s", (1, 4000) } }));
        List< Dictionary<string, (long, long)>> accepted = [];

        while (objects.Count > 0)
        {
            (var workId, var obj) = objects.Dequeue();
            if (workId == "A")
            {
                accepted.Add(obj);
                continue;
            }
            if (workId == "R")
            {
                continue;
            }
            foreach (string step in workflows[workId])
            {
                if (step.Contains(":"))
                {
                    var temp = step.Split(":");
                    string newWorkId = temp[1];
                    string ins = temp[0];
                    if (ins.Contains("<"))
                    {
                        var values = ins.Split("<");
                        string letter = values[0];
                        if (obj[letter].Item1 < long.Parse(values[1]))
                        {
                            var newObj = CopyDict(obj);
                            newObj[letter] = (obj[letter].Item1, Math.Min(long.Parse(values[1]) - 1, obj[letter].Item2));
                            objects.Enqueue((newWorkId, newObj));
                            if (obj[letter].Item2 < long.Parse(values[1]))
                            {
                                break;
                            } else
                            {
                                obj[letter] = (Math.Max(long.Parse(values[1]), obj[letter].Item1), obj[letter].Item2);
                            }
                        }
                    }
                    else
                    {
                        var values = ins.Split(">");
                        string letter = values[0];
                        if (obj[letter].Item2 > long.Parse(values[1]))
                        {
                            var newObj = CopyDict(obj);
                            newObj[letter] = (Math.Max(long.Parse(values[1]) + 1, obj[letter].Item1), obj[letter].Item2 );
                            objects.Enqueue((newWorkId, newObj));
                            if (obj[letter].Item1 > long.Parse(values[1]))
                            {
                                break;
                            } else
                            {
                                obj[letter] = (obj[letter].Item1, Math.Min(long.Parse(values[1]), obj[letter].Item2));
                            }
                        }
                    }
                }
                else
                {
                    objects.Enqueue((step, obj));
                    break;
                }
            }
        }

        foreach( var acc in accepted)
        {
            long accValue = 1;
            //foreach( var r in acc.Values)
            //{
            //    accValue += calcRange(r);
            //}

            foreach (var r in acc.Values)
            {
                accValue *= r.Item2 - r.Item1 + 1;
            }

            result += accValue;
        }


        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 2");
    }

    long calcRange((long, long) r)
    {
        return (r.Item2 * (r.Item2 + 1) - r.Item1 * (r.Item1 - 1)) / 2;
    }

    Dictionary<string, (long, long)> CopyDict(Dictionary<string, (long, long)> d)
    {
        return new Dictionary<string, (long, long)> { { "x", d["x"] }, { "m", d["m"] }, { "a", d["a"] }, { "s", d["s"] } };
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
