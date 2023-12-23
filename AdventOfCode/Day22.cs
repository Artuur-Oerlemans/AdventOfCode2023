using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day22 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(long, long), char> _dict;
    private readonly List<(long, long)> dirs = [(1, 0), (-1, 0), (0, 1), (0, -1)];
    private readonly long maxI;
    private readonly long maxJ;

    public Day22()
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
        var initialCoor = _input.Select(x => x.Split("~"))
            .Select(x => (ToCoor(x[0]), ToCoor(x[1])))
            .OrderBy( x => x.Item1.Item3)
            .ToList();
        Dictionary<int, List<int>> leansOn = [];

        for(int s = 0; s < initialCoor.Count; s++)
        {
            var brick = initialCoor[s];
            var newBrick = (Add(brick.Item1, (0, 0, -1)), Add(brick.Item2, (0, 0, -1)));
            var inter = Intersects(initialCoor, s, newBrick);
            while (newBrick.Item1.Item3 > 0 && inter.Count == 0)
            {
                brick = newBrick;
                newBrick = (Add(brick.Item1, (0, 0, -1)), Add(brick.Item2, (0, 0, -1)));
                inter = Intersects(initialCoor, s, newBrick);
            }

            initialCoor[s] = brick;
            leansOn[s] = inter;
        }

        var criticiallyLeanOn = leansOn.Values.Where(x => x.Count == 1).Select(x => x[0]).ToHashSet();

        for (int s = 0; s < initialCoor.Count; s++)
        {
            if(!criticiallyLeanOn.Contains(s))
            {
                result++;
            }
        }

        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 1");
    }

    List<int> Intersects(List<((long, long, long), (long, long, long))> initialCoor, int s, ((long, long, long), (long, long, long)) newBrick)
    {
        var inter = new  List<int>();
        for(int d = 0; d < s; d++)
        {
            var otherBrick = initialCoor[d];

            if(isOverlap(newBrick, otherBrick))
            {
                inter.Add(d);
            }
        }
        return inter;

    }

    bool isOverlap(((long, long, long) s, (long, long, long) e) a, ((long, long, long) s, (long, long, long) e) b)
    {
        return a.s.Item1 <= b.e.Item1 && b.s.Item1 <= a.e.Item1
            && a.s.Item2 <= b.e.Item2 && b.s.Item2 <= a.e.Item2
            && a.s.Item3 <= b.e.Item3 && b.s.Item3 <= a.e.Item3;
    }

    (long, long, long) ToCoor(string input)
    {
        var t = input.Split(",").Select(x => long.Parse(x)).ToList(); ;

        return (t[0], t[1], t[2]);
    }

    public override ValueTask<string> Solve_2()
    {
        long result = 0;
        var initialCoor = _input.Select(x => x.Split("~"))
            .Select(x => (ToCoor(x[0]), ToCoor(x[1])))
            .OrderBy(x => x.Item1.Item3)
            .ToList();
        Dictionary<int, HashSet<int>> leansOn = [];

        for (int s = 0; s < initialCoor.Count; s++)
        {
            var brick = initialCoor[s];
            var newBrick = (Add(brick.Item1, (0, 0, -1)), Add(brick.Item2, (0, 0, -1)));
            var inter = Intersects(initialCoor, s, newBrick);
            while (newBrick.Item1.Item3 > 0 && inter.Count == 0)
            {
                brick = newBrick;
                newBrick = (Add(brick.Item1, (0, 0, -1)), Add(brick.Item2, (0, 0, -1)));
                inter = Intersects(initialCoor, s, newBrick);
            }

            initialCoor[s] = brick;
            leansOn[s] = inter.ToHashSet();
        }

        for (int s = 0; s < initialCoor.Count; s++)
        {
            HashSet<int> bricks = new HashSet<int>();
            bricks.Add(s);
            bool added = true;
            while (added)
            {
                added = false;
                for (int d = 0; d < initialCoor.Count; d++)
                {
                    if (!bricks.Contains(d) && leansOn[d].Count > 0 && leansOn[d].IsSubsetOf(bricks))
                    {
                        bricks.Add(d);
                        added = true;
                    }
                }
            }

            result += bricks.Count - 1;
        }

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

    private (long, long, long) Add((long, long, long) a, (long, long, long) b)
    {
        return (a.Item1 + b.Item1, a.Item2 + b.Item2, a.Item3 + b.Item3);
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
