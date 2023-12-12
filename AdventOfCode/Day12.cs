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

public class Day12 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(int, int), string> _dict = new Dictionary<(int, int), string>();
    private readonly int maxI;
    private readonly int maxJ;

    public Day12()
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
        var list = _input
            .Select(x => x.Split(" "))
            .Select(x => (x[0], x[1].Split(",").Select(int.Parse).ToList()))
            .ToList();
        int index = 0;
        foreach ((string mys, List<int> nums) in list)
        {
            index++;
            result += GetPossible(mys, nums, "");

        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 1");
    }
    Dictionary<string, long> mem = new Dictionary<string, long>();

    long GetPossible(string mys, List<int> nums, string soFar)
    {
        string stringy = mys + " * " + string.Join(",", nums);
        if ( mem.ContainsKey(stringy))
        {
            return mem[stringy];
        }

        long result = 0;
        for (int i = 0; i <= mys.Length; i++)
        {
            string made = "";
            for(int j = 0; j < i; j++)
            {
                made += ".";
            }
            for (int j = 0; j < nums[0]; j++)
            {
                made += "#";
            }
            made += ".";
            if ( Compatible(mys, made) )
            {
                if ( nums.Count -1 == 0)
                {
                    bool noSpa = true;
                    for(int k = made.Length; k < mys.Length; k++)
                    {
                        if (mys[k] == '#')
                        {
                            noSpa = false;
                        }
                    }
                    if( noSpa)
                    {
                        //L(soFar + made);
                        result += 1;
                    }
                } else if(made.Length < mys.Length)
                {
                    result += GetPossible(mys.Substring(made.Length), nums[1..], soFar + made);
                }
            }
        }
        mem.Add(stringy, result);
        return result;
    }

    bool Compatible(string mys, string made)
    {
        for(int i = 0; i < made.Length; i++)
        {
            if ( i >=  mys.Length && made[i] == '#')
            {
                return false;
            }
            else if (i < mys.Length && mys[i] != '?' && mys[i] !=  made[i])
            {
                return false;
            }
        }
        return true;
    }

    public override ValueTask<string> Solve_2()
    {
        long result = 0;
        var list = _input
            .Select(x => x.Split(" "))
            .Select(x => (x[0], x[1].Split(",").Select(int.Parse).ToList()))
            .ToList();
        int index = 0;
        foreach ((string mys, List<int> nums) in list)
        {
            string mys5 = mys + "?" + mys + "?" + mys + "?" + mys + "?" + mys;
            List<int> nums5 = new List<int>();
            nums5.AddRange(nums);
            nums5.AddRange(nums);
            nums5.AddRange(nums);
            nums5.AddRange(nums);
            nums5.AddRange(nums);
            //L(_input[index]);
            index++;
            result += GetPossible(mys5, nums5, "");

        }
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
