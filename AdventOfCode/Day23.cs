using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day23 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(long, long), char> _dict;
    private readonly List<(long, long)> dirs = [(1, 0), (-1, 0), (0, 1), (0, -1)];
    private readonly long maxI;
    private readonly long maxJ;

    public Day23()
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
                _dict.Add((i, j), _input[i][j]);
            }
        }

    }
    public override ValueTask<string> Solve_1()
    {
        long result = 0;

        Stack<((long, long), HashSet<(long, long)>)> que = [];
        que.Push(((0, 1), [(0, 1)]));

        while (que.Count > 0)
        {
            ((long, long) loc, HashSet<(long, long)> history) = que.Pop();
            List<(long, long)> locDirs;

            if (loc == (maxI - 1, maxJ - 2))
            {
                result = Math.Max(history.Count - 1, result);
            }

            if (_dict[loc] == '<')
            {
                locDirs = [(0, -1)];
            }
            else if (_dict[loc] == '^')
            {
                locDirs = [(-1, 0)];
            }
            else if (_dict[loc] == '>')
            {
                locDirs = [(0, 1)];
            }
            else if (_dict[loc] == 'v')
            {
                locDirs = [(1, 0)];
            }
            else
            {
                locDirs = dirs;
            }


            foreach (var dir in locDirs)
            {
                var newLoc = Add(loc, dir);

                if (_dict.ContainsKey(newLoc) && _dict[newLoc] != '#' && !history.Contains(newLoc))
                {
                    var newHistory = history.ToHashSet();
                    newHistory.Add(newLoc);
                    que.Push((newLoc, newHistory));
                }
            }
        }

        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 1");
    }

    public override ValueTask<string> Solve_2()
    {
        long result = 0;

        var graph = new Dictionary<(long, long), Dictionary<(long, long), long>>
        {
            { (0, 1), new Dictionary<(long, long), long>() }
        };

        Queue<((long, long), List<(long, long)>, (long, long), long)> que = [];
        que.Enqueue(((0, 1), [(0, 1)], (0, 1), 0));

        while (que.Count > 0)
        {
            ((long, long) loc, List<(long, long)> history, var lastKnot, var lastKnotDis) = que.Dequeue();

            List<((long, long), List<(long, long)>)> nextRoutes = [];
            foreach (var dir in dirs)
            {
                var newLoc = Add(loc, dir);

                if (_dict.ContainsKey(newLoc) && _dict[newLoc] != '#' && (history.Count == 1 || history[^2] != newLoc))
                {
                    var newHistory = history.ToList();
                    newHistory.Add(newLoc);
                    nextRoutes.Add((newLoc, newHistory));
                }
            }

            if (loc == (maxI - 1, maxJ - 2))
            {
                graph[lastKnot].Add((maxI - 1, maxJ - 2), lastKnotDis);
                continue;
            } else if (nextRoutes.Count > 1)
            {
                if (!graph.ContainsKey(loc)) { 
                    graph[loc] = new Dictionary<(long, long), long>();
                    foreach (var route in nextRoutes)
                    {
                        que.Enqueue((route.Item1, route.Item2, loc, 1));
                    }
                } 

                if (!graph[lastKnot].ContainsKey(loc) || graph[lastKnot][loc] < lastKnotDis)
                {
                    graph[lastKnot][loc] = lastKnotDis;
                    graph[loc][lastKnot] = lastKnotDis;
                }
            }
            if (nextRoutes.Count == 1)
            {
                que.Enqueue((nextRoutes[0].Item1, nextRoutes[0].Item2, lastKnot, lastKnotDis + 1));
            }
        }

        Stack<((long, long), HashSet<(long, long)>, long)> paths = [];
        paths.Push(((0, 1), [(0, 1)], 0));

        while(paths.Count > 0)
        {
            ((long, long) loc, HashSet<(long, long)> history, long dist) = paths.Pop();
            if(loc == (maxI - 1, maxJ - 2))
            {
                if(dist > result)
                {
                    result = dist;
                    //L(dist + " remaining path: " + paths.Count );
                }
                continue;
            }

            foreach ((long, long) newLoc in graph[loc].Keys)
            {
                if(!history.Contains(newLoc))
                {
                    var newHistory = history.ToHashSet();
                    newHistory.Add(newLoc);
                    paths.Push((newLoc, newHistory, dist + graph[loc][newLoc]));
                }
            }
        }


        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 2");
    }

    public ValueTask<string> Solve_2_old()
    {
        long result = 0;

        Queue<((long, long), HashSet<(long, long)>)> que = [];
        que.Enqueue(((0, 1), [(0, 1)]));

        while (que.Count > 0)
        {
            ((long, long) loc, HashSet<(long, long)> history) = que.Dequeue();
            List<(long, long)> locDirs;

            if (loc == (maxI - 1, maxJ - 2))
            {
                result = Math.Max(history.Count - 1, result);
            }

            locDirs = dirs;


            foreach (var dir in locDirs)
            {
                var newLoc = Add(loc, dir);

                if (_dict.ContainsKey(newLoc) && _dict[newLoc] != '#' && !history.Contains(newLoc))
                {
                    var newHistory = history.ToHashSet();
                    newHistory.Add(newLoc);
                    que.Enqueue((newLoc, newHistory));
                }
            }
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
