using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day17 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(int, int), char> _dict;
    private readonly List<(int, int)> dirs = [(1, 0), (-1, 0), (0, 1), (0, -1)];
    private readonly int maxI;
    private readonly int maxJ;

    public Day17()
    {
        _input =
        [
            .. File.ReadAllText(string.Concat(InputFilePath.AsSpan(0, InputFilePath.Length - 4)
, "test"
, "+"
, ".txt")
                        ).Split("\r\n")
,
        ];

        maxI = _input.Count;
        maxJ = _input[0].Length;
        _dict = new(maxI * maxJ);
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
        PriorityQueue<((int, int), (int, int), int), long> prio = new PriorityQueue<((int, int), (int, int), int), long>();
        prio.Enqueue(((0, 0), (0, 1), 0), 0);
        prio.Enqueue(((0, 0), (1, 0), 0), 0);

        Dictionary<((int, int), (int, int), int), long> mem = new Dictionary<((int, int), (int, int), int), long>();
        mem[((0, 0), (0, 1), 0)] = 0;
        mem[((0, 0), (1, 0), 0)] = 0;

        ((int, int) loc, (int, int) dir, int len) = prio.Dequeue();
        while( loc != (maxI -1, maxJ -1))
        {
            List<(int, int)> locDirs = [(1, 0), (-1, 0), (0, 1), (0, -1)];
            locDirs.Remove(Neg(dir));
            if ( len >= 3 )
            {
                locDirs.Remove(dir);
            }

            long loss = mem[(loc, dir, len)];

            foreach ( var newDir in  locDirs )
            {
                (int, int) newLoc = Add(loc, newDir);
                if (_dict.ContainsKey(newLoc))
                {
                    long newLoss = loss + int.Parse("" + _dict[newLoc]);
                    int newLen = newDir == dir ? len + 1 : 1;

                    long lowLoss = long.MaxValue;
                    for(int i = 0; i <= newLen; i++)
                    {
                        if(mem.ContainsKey((newLoc, newDir, i)))
                        {
                            lowLoss = Math.Min(lowLoss, mem[(newLoc, newDir, i)]);
                        }
                    }

                    if ( newLoss < lowLoss )
                    {
                        mem[(newLoc, newDir, newLen)] = newLoss;
                        prio.Enqueue((newLoc, newDir, newLen), newLoss + 2 * ManDis(newLoc, (maxI - 1, maxJ - 1)));
                    }
                }
            }
            (loc, dir, len) = prio.Dequeue();
        }
        result = mem[(loc, dir, len)];
        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 1");
    }

    public override ValueTask<string> Solve_2()
    {
        long result = 0;
        PriorityQueue<((int, int), (int, int), int), long> prio = new PriorityQueue<((int, int), (int, int), int), long>();
        prio.Enqueue(((0, 0), (0, 1), 0), 0);
        prio.Enqueue(((0, 0), (1, 0), 0), 0);

        Dictionary<((int, int), (int, int), int), long> mem = new Dictionary<((int, int), (int, int), int), long>();
        mem[((0, 0), (0, 1), 0)] = 0;
        mem[((0, 0), (1, 0), 0)] = 0;

        ((int, int) loc, (int, int) dir, int len) = prio.Dequeue();
        while (loc != (maxI - 1, maxJ - 1) || len < 4 )
        {
            List<(int, int)> locDirs = [(1, 0), (-1, 0), (0, 1), (0, -1)];
            locDirs.Remove(Neg(dir));
            if (len >= 10)
            {
                locDirs.Remove(dir);
            }

            long loss = mem[(loc, dir, len)];

            foreach (var newDir in locDirs)
            {
                if( newDir != dir && len < 4)
                {
                    continue;
                }
                (int, int) newLoc = Add(loc, newDir);
                if (_dict.ContainsKey(newLoc))
                {
                    long newLoss = loss + int.Parse("" + _dict[newLoc]);
                    int newLen = newDir == dir ? len + 1 : 1;

                    long lowLoss = long.MaxValue;
                    if (mem.ContainsKey((newLoc, newDir, newLen)))
                    {
                        lowLoss = Math.Min(lowLoss, mem[(newLoc, newDir, newLen)]);
                    }

                    if (newLoss < lowLoss)
                    {
                        mem[(newLoc, newDir, newLen)] = newLoss;
                        prio.Enqueue((newLoc, newDir, newLen), newLoss + 2 * ManDis(newLoc, (maxI - 1, maxJ - 1)));
                    }
                }
            }
            (loc, dir, len) = prio.Dequeue();
        }
        result = mem[(loc, dir, len)];
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
