using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day04 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(int, int), string> _dict = new Dictionary<(int, int), string>();

    public Day04()
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
        var t= _input.Select(s => s.Replace("  ", " "))
            .Select(s => s.Split(": ")[1])
            .Select(s => s.Split(" | "))
            .Select(s =>
            {
                var w = new HashSet<string>(s[0].Split(" "));
                var m = new HashSet<string>(s[1].Split(" "));
                m.IntersectWith(w);
                if ( m.Count == 0 )
                {
                    return 0;
                }
                return Math.Pow(2, m.Count -1);
            })
            .Sum();
        return new ValueTask<string>($"Solution to {ClassPrefix} {t}, part 1");
    }

    public override ValueTask<string> Solve_2()
    {
        var t = _input.Select(s => s.Replace("  ", " "))
            .Select(s => s.Split(": ")[1])
            .Select(s => s.Split(" | "))
            .Select(s =>
            {
                var w = new HashSet<string>(s[0].Split(" "));
                var m = new HashSet<string>(s[1].Split(" "));
                m.IntersectWith(w);
                return m.Count();
            }).ToList();
        var dic = new Dictionary<int, int>();
        for (int i = 0; i < t.Count; i++)
        {
            dic[i] = 1;
        }

        for (int i = 0; i < t.Count; i ++)
        {
            for (int j = 1; j <= t[i] && j < t.Count; j++ )
            {
                dic[i + j] += dic[i];
            }
        }
        var tot = dic.Values.ToList().Sum();
        return new ValueTask<string>($"Solution to {ClassPrefix} {tot}, part 2");
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
