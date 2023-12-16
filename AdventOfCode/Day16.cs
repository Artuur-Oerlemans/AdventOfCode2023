using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day16 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(int, int), char> _dict = new Dictionary<(int, int), char>();
    private readonly List<(int, int)> dirs = [(1, 0), (-1, 0), (0, 1), (0, -1)];
    private readonly int maxI;
    private readonly int maxJ;

    public Day16()
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
                _dict.Add((i, j), _input[i][j]);
            }
        }

    }
    public override ValueTask<string> Solve_1()
    {
        long result = 0;
        for( int i = 0; i < maxI; i++)
        {
            for ( int j = 0; j < maxJ; j++)
            {
                dirs.ForEach(x => result = Math.Max(result, shootBeam((i, j), x)));
            }
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 1");
    }

    private long shootBeam((int, int) initialLoc, (int, int) initialDir)
    {
        long result;
        Stack<((int, int), (int, int))> beamsNow = new Stack<((int, int), (int, int))>();

        beamsNow.Push((initialLoc, initialDir));
        HashSet<(int, int)> energized = new HashSet<(int, int)>();
        HashSet<((int, int), (int, int))> mem = new HashSet<((int, int), (int, int))>();


        while (beamsNow.Count > 0)
        {
            Stack<((int, int), (int, int))> beamsFuture = new Stack<((int, int), (int, int))>();

            while (beamsNow.Count > 0)
            {
                ((int, int) loc, (int, int) dir) = beamsNow.Pop();

                if (_dict.ContainsKey(loc))
                {
                    energized.Add(loc);
                    if (mem.Contains((loc, dir)))
                    {
                        continue;
                    }
                    else
                    {
                        mem.Add((loc, dir));
                    }

                    switch (_dict[loc])
                    {
                        case '-' when dir.Item1 == 0:
                        case '|' when dir.Item2 == 0:
                        case '.':
                            beamsFuture.Push(newBeam(loc, dir));
                            break;
                        case '|' when dir.Item1 == 0:
                            beamsFuture.Push((Add(loc, (1, 0)), (1, 0)));
                            beamsFuture.Push((Add(loc, (-1, 0)), (-1, 0)));
                            break;
                        case '-' when dir.Item2 == 0:
                            beamsFuture.Push((Add(loc, (0, 1)), (0, 1)));
                            beamsFuture.Push((Add(loc, (0, -1)), (0, -1)));
                            break;
                        case '/':
                            beamsFuture.Push(newBeam(loc, (-dir.Item2, -dir.Item1)));
                            break;
                        case '\\':
                            beamsFuture.Push(newBeam(loc, (dir.Item2, dir.Item1)));
                            break;
                        default:
                            throw new Exception();
                    }

                }
            }

            beamsNow = beamsFuture;
        }

        //L(print(energized));
        result = energized.Count;
        return result;
    }

    String print(HashSet<(int, int)> energized)
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

    private ((int, int), (int, int)) newBeam((int, int) loc, (int, int) dir)
    {
        return (Add(loc, dir), dir);
    }

    public override ValueTask<string> Solve_2()
    {
        long result = 0;
        for (int i = 0; i < maxI; i++)
        {
            for (int j = 0; j < maxJ; j++)
            {
                
                dirs.ForEach(x => result = Math.Max(result, shootBeam((i, j), x)));
            }
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
