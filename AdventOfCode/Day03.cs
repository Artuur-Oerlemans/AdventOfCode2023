using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day03 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(int, int), string> _dict = new Dictionary<(int, int), string>();

    public Day03()
    {
        _input =
        [
            .. File.ReadAllText(string.Concat(InputFilePath.AsSpan(0, InputFilePath.Length - 4)
//, "test"
, ".txt")
                        ).Split("\r\n")
,
        ];
        for (int i = 0; i < _input.Count; i++)
        {
            for (int j = 0; j < _input[i].Length; j++)
            {
                if (_input[i][j]  != '.') 
                {
                    _dict.Add((i, j), "" + _input[i][j]);
                }
            }
        }
    }
    public override ValueTask<string> Solve_1()
    {
        int sum = 0;
        for (int i = 0; i < _input.Count; i++)
        {
            for (int j = 0; j < _input[i].Length; j++)
            {
                var re = new Regex(@"\d");
                var match = re.Match("" + _input[i][j]);
                if (!match.Success)
                {
                    continue;
                }
                int jEnd = j + 1;
                while (jEnd < _input[i].Length && re.Match("" + _input[i][jEnd]).Success)
                {
                    jEnd++;
                }

                bool notFound = true;
                for (int si = i - 1; si < i + 2 && notFound; si++)
                {
                    for (int sj = j - 1; sj < jEnd + 1 && notFound; sj++)
                    {
                        if (si == i && sj >= j && sj < j)
                        {
                            continue;
                        }
                        if (si < 0 || si >= _input.Count || sj < 0 || sj >= _input[i].Length)
                        {
                            continue;
                        }
                        var not = new Regex(@"^[^0-9.]$");
                        var special = not.Match("" + _input[si][sj]);
                        if (special.Success)
                        {
                            sum += int.Parse(_input[i].Substring(j, jEnd - j));
                            notFound = false;
                        }
                    }
                }
                j = jEnd - 1;
            }
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {sum}, part 1");
    }

    public override ValueTask<string> Solve_2()
    {
        int sum = 0;
        HashSet<(int, int)> used = new HashSet<(int, int)>();
        for (int i = 0; i < _input.Count; i++)
        {
            for (int j = 0; j < _input[i].Length; j++)
            {
                if (used.Contains((i, j)))
                {
                    continue;
                }
                var re = new Regex(@"\d");
                var match = re.Match("" + _input[i][j]);
                if (!match.Success)
                {
                    continue;
                }
                used.Add((i, j));
                int jEnd = j + 1;
                while (jEnd < _input[i].Length && re.Match("" + _input[i][jEnd]).Success)
                {
                    used.Add((i, jEnd));
                    jEnd++;
                }

                bool notFound = true;
                for (int si = i - 1; si < i + 2 && notFound; si++)
                {
                    for (int sj = j - 1; sj < jEnd + 1 && notFound; sj++)
                    {
                        if (si == i && sj >= j && sj < jEnd)
                        {
                            continue;
                        }
                        if (si < 0 || si >= _input.Count || sj < 0 || sj >= _input[i].Length)
                        {
                            continue;
                        }
                        var not = new Regex(@"^\*$");
                        var special = not.Match("" + _input[si][sj]);
                        if (special.Success)
                        {
                            for (int di = si - 1; di < si + 2 && notFound; di++)
                            {
                                for (int dj = sj - 1; dj < sj + 2 && notFound; dj++)
                                {
                                    if (used.Contains((di, dj)))
                                    {
                                        continue;
                                    }
                                    if (di < 0 || di >= _input.Count || dj < 0 || dj >= _input[i].Length)
                                    {
                                        continue;
                                    }
                                    if (re.Match("" + _input[di][dj]).Success)
                                    {
                                        int djStart = dj;
                                        used.Add((di, djStart));
                                        while (djStart - 1 >= 0 && re.Match("" + _input[di][djStart - 1]).Success)
                                        {
                                            djStart -= 1;
                                            used.Add((di, djStart));
                                        }
                                        int djEnd = dj + 1;
                                        while (djEnd <= _input[i].Length && re.Match("" + _input[di][djEnd]).Success)
                                        {
                                            used.Add((di, djEnd - 1));
                                            djEnd += 1;
                                        }
                                        // L(_input[i].Substring(j, jEnd - j) + " " + _input[di].Substring(djStart, djEnd - djStart));
                                        sum += int.Parse(_input[i].Substring(j, jEnd - j)) * int.Parse(_input[di].Substring(djStart, djEnd - djStart));
                                        notFound = false;
                                    }
                                }
                            }
                            notFound = false;
                        }
                    }
                }
                j = jEnd - 1;
            }
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {sum}, part 2");
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
