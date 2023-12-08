using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text.RegularExpressions;
using static System.Reflection.Metadata.BlobBuilder;

namespace AdventOfCode;

public class Day08 : BaseDay
{
    private readonly List<string> _input;
    private readonly List<string> _directions;
    private readonly Dictionary<string, (string, string)> _dic;


    public Day08()
    {
        _input =
        [
            .. File.ReadAllText(string.Concat(InputFilePath.AsSpan(0, InputFilePath.Length - 4)
//, "test"
, ".txt")
                        ).Split("\r\n\r\n")
,
        ];
        _directions = _input[0].Select(x => "" + x).ToList();
        _dic = _input[1].Split("\r\n")
            .Select(x => x.Split(" = ("))
            .ToDictionary(x => x[0], x => (x[1].Split(", ")[0], x[1].Split(", ")[1].Substring(0, 3)));

    }
    public override ValueTask<string> Solve_1()
    {
        int steps = 0;
        string loc = "AAA";
        while(loc != "ZZZ")
        {
            if (_directions[steps % _directions.Count] == "R")
            {
                loc = _dic[loc].Item2;
            }
            else
            {
                loc = _dic[loc].Item1;
            }
            steps++;
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {steps}, part 1");
    }

    public override ValueTask<string> Solve_2()
    {

        Dictionary<int, long> loops = new Dictionary<int, long>();
        Dictionary<int, List<(string, long)>> rz = new Dictionary<int, List<(string, long)>>();
        long steps = 0;

        List<string> locs = _dic.Keys.Where(x => x.EndsWith("A")).ToList();
        rz = Enumerable.Range(0, locs.Count).ToDictionary(x => x, x => new List<(string, long)>());
        List<string> starts = new List<string>(locs);
        while (loops.Count != starts.Count)
        {
            for( int i = 0; i < locs.Count; i++ )
            {

                if (_directions[(int)(steps % _directions.Count)] == "R")
                {
                    locs[i] = _dic[locs[i]].Item2;
                }
                else
                {
                    locs[i] = _dic[locs[i]].Item1;
                }
                if (locs[i].EndsWith("Z"))
                {
                    if (rz[i].Any( x => x.Item1 == locs[i] && (steps + 1 - x.Item2) % _directions.Count == 0))
                    {
                        if(!loops.ContainsKey(i))
                        {
                            var x = rz[i].First(x => x.Item1 == locs[i] && (steps + 1 - x.Item2) % _directions.Count == 0);
                            loops[i] = steps + 1 - x.Item2;
                        }
                    } else
                    {
                        rz[i].Add((locs[i], steps + 1));
                    }
                }
            }
            steps++;
        }
        long loop = loops[0];
        long stp = rz[0][0].Item2;
        while(!Enumerable.Range(0, locs.Count).Skip(1)
            .All(i => 
            (stp - rz[i][0].Item2 ) % loops[i] == 0)
            )
        {
            stp += loop;
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {stp}, part 2");
    }

    public ValueTask<string> Solve_2_old()
    {
        int steps = 0;

        List<string> locs = _dic.Keys.Where(x => x.EndsWith("A")).ToList();
        while (!locs.All(x => x.EndsWith("Z")))
        {
            for (int i = 0; i < locs.Count; i++)
            {

                if (_directions[steps % _directions.Count] == "R")
                {
                    locs[i] = _dic[locs[i]].Item2;
                }
                else
                {
                    locs[i] = _dic[locs[i]].Item1;
                }
            }
            steps++;
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {steps}, part 2");
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
