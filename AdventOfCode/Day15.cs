using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day15 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(int, int), string> _dict = new Dictionary<(int, int), string>();
    private readonly int maxI;
    private readonly int maxJ;

    public Day15()
    {
        _input =
        [
            .. File.ReadAllText(string.Concat(InputFilePath.AsSpan(0, InputFilePath.Length - 4)
//, "test"
, ".txt")
                        ).Split(",")
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
        foreach ( var s in _input)
        {
            long cv = 0;
            var bytes = Encoding.ASCII.GetBytes(s);

            foreach (int b in bytes)
            {
                cv += b;
                cv *= 17;
                cv %= 256;
            }
            result += cv;
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 1");
    }

    public override ValueTask<string> Solve_2()
    {
        long result = 0;
        Dictionary<int, List<(string, int)>> boxes = new Dictionary<int, List<(string, int)>>();

        for(int i = 0; i < 256; i++)
        {
            boxes[i] = new List<(string, int)>();
        }


        foreach ( var s in _input)
        {
            if (s.EndsWith("-"))
            {
                var parts = s.Split('-');
                int h = Hash(parts[0]);
                for (int i = 0; i < boxes[h].Count; i++)
                {
                    if (boxes[h][i].Item1 == parts[0])
                    {
                        boxes[h].Remove((boxes[h][i].Item1, boxes[h][i].Item2));
                    }
                }

            } else
            {
                var parts = s.Split('=');
                int h = Hash(parts[0]);
                if (boxes[h].Any(x => x.Item1 == parts[0]))
                {
                    for(int i = 0; i < boxes[h].Count; i++ )
                    {
                        if (boxes[h][i].Item1 == parts[0])
                        {
                            boxes[h][i] = (parts[0], int.Parse(parts[1]));
                        }
                    }
                }
                else
                {
                    boxes[h].Add((parts[0], int.Parse(parts[1])));
                }
            }
        }

        
        for(int i = 1; i <= boxes.Count; i++)
        {
            for (int j = 1; j <= boxes[i -1].Count; j++)
            {
                result += i * j * boxes[i - 1][j - 1].Item2;
            }
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 2");
    }

    int Hash(string s)
    {
        int cv = 0;
        var bytes = Encoding.ASCII.GetBytes(s);

        foreach (int b in bytes)
        {
            cv += b;
            cv *= 17;
            cv %= 256;
        }
        return cv;
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
