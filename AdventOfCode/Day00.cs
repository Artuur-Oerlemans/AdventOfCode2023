using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text.RegularExpressions;
using static System.Reflection.Metadata.BlobBuilder;

namespace AdventOfCode;

public class Day00 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(int, int), string> _dict = new Dictionary<(int, int), string>();
    private readonly int maxI;
    private readonly int maxJ;

    public Day00()
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
        for (int i = 0; i < _input.Count; i++)
        {
            for (int j = 0; j < _input[i].Length; j++)
            {
                if (_input[i][j] != '.')
                {
                    _dict.Add((i, j), "" + _input[i][j]);
                }
            }
        }

    }
    public override ValueTask<string> Solve_1()
    {
        long result = 0;
        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 1");
    }

    public override ValueTask<string> Solve_2()
    {
        long result = 0;
        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 2");
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
    private (int, int) Add((int, int) a, (int, int) b)
    {
        return (a.Item1 + b.Item1, a.Item2 + b.Item2);
    }

    private int ManDis((int, int) a, (int, int) b)
    {
        return Math.Abs(a.Item1 - a.Item1) + Math.Abs(b.Item2 - b.Item2);
    }

    private (int, int) Neg((int, int) p)
    {
        return (-p.Item1, -p.Item2);
    }
}
