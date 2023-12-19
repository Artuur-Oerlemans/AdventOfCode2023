using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day18 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(int, int), char> _dict;
    private readonly List<(long, long)> dirs = [(1, 0), (-1, 0), (0, 1), (0, -1)];
    private readonly int maxI;
    private readonly int maxJ;

    public Day18()
    {
        _input =
        [
            .. File.ReadAllText(string.Concat(InputFilePath.AsSpan(0, InputFilePath.Length - 4)
//, "test"
//, "test2"
//, "test3"
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
        var inp = _input.Select(x => x.Split(" "))
            .Select(x => (x[0].ToCharArray()[0], int.Parse(x[1]), x[2]))
            .ToList();

        var trench = new HashSet<(long, long)>();
        trench.Add((0, 0));

        (long, long) loc = (0, 0);
        foreach (var ins in inp)
        {
            (int, int) dir;
            switch (ins.Item1)
            {
                case 'U':
                    dir = (-1, 0);
                    break;
                case 'D':
                    dir = (1, 0);
                    break;
                case 'R':
                    dir = (0, 1);
                    break;
                case 'L':
                    dir = (0, -1);
                    break;
                default:
                    throw new Exception();
            }

            for (int i = 0; i < ins.Item2; i++)
            {
                loc = Add(loc, dir);
                trench.Add(loc);
            }
        }

        var contents = new List<(long, long)>();
        var locations = new Queue<(long, long)>();
        locations.Enqueue((1, 1));

        while (locations.Count > 0)
        {
            loc = locations.Dequeue();

            if (contents.Contains(loc))
            {
                continue;
            }
            else
            {
                contents.Add(loc);
            }

            foreach (var dir in dirs)
            {
                (long, long) newLoc = Add(dir, loc);
                if (!trench.Contains(newLoc) && !contents.Contains(newLoc))
                {
                    locations.Enqueue(newLoc);
                }
            }
        }

        L(trench.Count);
        L(LocationSetToString(trench));
        L(LocationSetToString(contents.ToHashSet()));

        return new ValueTask<string>($"Solution to {ClassPrefix} {trench.Count + contents.Count}, part 1");
    }

    char toChar(int a)
    {
        switch (a)
        {
            case 0:
                return 'R';
            case 1:
                return 'D';
            case 2:
                return 'L';
            case 3:
                return 'U';
            default:
                throw new Exception("" + a);
        }
    }

    public override ValueTask<string> Solve_2()
    {
        long result = 0;
        var temp = _input.ToList();
        temp.Add(_input[0]);
        var inp = temp.Select(x => x.Split(" "))
            //.Select(x => (x[0].ToCharArray()[0], int.Parse(x[1]), x[2]))
            .Select(x => x[2].Substring(2, 6))
            .Select(x => (x.Substring(0, 5), x.Substring(5)))
            .Select(x => (toChar(int.Parse(x.Item2)), Convert.ToInt64(x.Item1, 16)))
            .ToList();

        var corners = new PriorityQueue<((long, long), string), long>();

        (long, long) loc = (0, 0);
        (long, long) oldDir = (0, 0);
        foreach (var ins in inp)
        {
            (long, long) dir;
            switch (ins.Item1)
            {
                case 'U':
                    dir = (-1, 0);
                    break;
                case 'D':
                    dir = (1, 0);
                    break;
                case 'R':
                    dir = (0, 1);
                    break;
                case 'L':
                    dir = (0, -1);
                    break;
                default:
                    throw new Exception();
            }


            if (oldDir == (0, 0))
            {
                loc = Add(loc, Mul(dir, ins.Item2));
                oldDir = dir;
                continue;
            }
            string corner;
            if ((oldDir == (-1, 0) && dir == (0, 1)) || (oldDir == (0, -1) && dir == (1, 0)))
            {
                corner = "UL";
            }
            else if ((oldDir == (0, 1) && dir == (1, 0)) || (oldDir == (-1, 0) && dir == (0, -1)))
            {
                corner = "UR";
            }
            else if ((oldDir == (1, 0) && dir == (0, 1)) || (oldDir == (0, -1) && dir == (-1, 0)))
            {
                corner = "DL";
            }
            else if ((oldDir == (0, 1) && dir == (-1, 0)) || (oldDir == (1, 0) && dir == (0, -1)))
            {
                corner = "DR";
            }
            else
            {
                throw new Exception();
            }
            corners.Enqueue((loc, corner), loc.Item1);
            loc = Add(loc, Mul(dir, ins.Item2));

            oldDir = dir;
        }
        long contents = 0;

        ((long, long) nextLoc, string nextCor) = corners.Dequeue();
        List<((long, long), string)> oldCorners = new List<((long, long), string)>();
        List<((long, long), string)> oldCornersPlus = new List<((long, long), string)>();
        HashSet<long> linesGoingDown = new HashSet<long>();
        long trenchCount = 0;

        while (corners.Count > 0)
        {
            long curI = nextLoc.Item1;
            List<((long, long), string)> currentCorners = new List<((long, long), string)>();
            while (nextLoc.Item1 == curI)
            {
                currentCorners.Add((nextLoc, nextCor));
                if (corners.Count <= 0)
                {
                    break;
                }
                (nextLoc, nextCor) = corners.Dequeue();
            }
            currentCorners = currentCorners.OrderBy(x => x.Item1.Item2).ToList();

            var hors = new List<(long, long)>();

            for (int i = 0; i < currentCorners.Count; i++)
            {
                ((long, long) curLoc, string curCor) = currentCorners[i];
                if (curCor == "UL")
                {
                    long l = curLoc.Item2;
                    long r = currentCorners[i + 1].Item1.Item2;
                    trenchCount += r - l;
                    hors.Add((l, r));
                }
                else if (curCor == "DL")
                {
                    long l = curLoc.Item2;
                    long r = currentCorners[i + 1].Item1.Item2;
                    trenchCount += currentCorners[i + 1].Item1.Item2 - curLoc.Item2;
                    hors.Add((l, r));
                    long above = oldCorners.Where(x => curLoc.Item2 == x.Item1.Item2).Select(x => x.Item1.Item1).Max();
                    trenchCount += curLoc.Item1 - above;
                }
                else if (curCor == "DR")
                {
                    long above = oldCorners.Where(x => curLoc.Item2 == x.Item1.Item2).Select(x => x.Item1.Item1).Max();
                    trenchCount += curLoc.Item1 - above;
                }
            }
            hors = hors.OrderBy(x => x.Item1).ToList();

            var orderLGD = linesGoingDown.OrderBy(x => x).ToList();
            if (orderLGD.Count % 2 != 0)
            {
                throw new Exception();
            }

            var skipList = new List<int>();
            for (int i = 0; i < currentCorners.Count; i++)
            {
                if (skipList.Contains(i))
                {
                    continue;
                }

                ((long, long) curLoc, string curCor) = currentCorners[i];
                if (curCor == "DR" || curCor == "DL")
                {
                    int j = 0;
                    while (orderLGD[j] != curLoc.Item2)
                    {
                        j++;
                    }
                    long aboveL;
                    long aboveR;
                    long above;
                    long otherLine;
                    if (j % 2 == 0)
                    {
                        otherLine = orderLGD[j + 1];
                        aboveL = oldCornersPlus.Where(x => curLoc.Item2 == x.Item1.Item2).Select(x => x.Item1.Item1).Max();
                        aboveR = oldCornersPlus.Where(x => otherLine == x.Item1.Item2).Select(x => x.Item1.Item1).Max();
                        above = Math.Max(aboveL, aboveR);

                    } else
                    {
                        otherLine = orderLGD[j - 1];
                        aboveL = oldCornersPlus.Where(x => otherLine == x.Item1.Item2).Select(x => x.Item1.Item1).Max();
                        aboveR = oldCornersPlus.Where(x => curLoc.Item2 == x.Item1.Item2).Select(x => x.Item1.Item1).Max();
                        above = Math.Max(aboveL, aboveR);
                    }
                    contents += (curLoc.Item1 - above - 1) * (Math.Abs(curLoc.Item2 - otherLine) -1);
                    long l = Math.Min(otherLine, curLoc.Item2);
                    long r = Math.Max(otherLine, curLoc.Item2);
                    for (int k = 0; k < currentCorners.Count; k++)
                    {
                        if (l <=currentCorners[k].Item1.Item2 && currentCorners[k].Item1.Item2 <= r)
                        {
                            skipList.Add(k);
                        }
                    }
                    oldCornersPlus.Add(((curLoc.Item1, l), "UL"));
                    oldCornersPlus.Add(((curLoc.Item1, r), "UR"));

                    while ( l < r)
                    {
                        var overLapping = hors.Where(x => x.Item1 <= l && x.Item2 > l).ToList();
                        if(overLapping.Count > 0)
                        {
                            l = overLapping[0].Item2;
                            continue;
                        }

                        var ToOverLapping = hors.Where(x => x.Item1 >= l && x.Item1 < r).OrderBy(x => x.Item1).ToList();
                        if(ToOverLapping.Count > 0)
                        {
                            contents += ToOverLapping[0].Item1 - l - 1;
                            l = ToOverLapping[0].Item2;
                            continue;
                        } else
                        {
                            contents += r - l - 1;
                            break;
                        }
                    }
                } else
                {
                    for(int leftLineIndex = 0; leftLineIndex < orderLGD.Count; leftLineIndex += 2)
                    {
                        if(orderLGD[leftLineIndex] < curLoc.Item2 && curLoc.Item2 < orderLGD[leftLineIndex + 1])
                        {

                            int j = leftLineIndex;
                            long l = orderLGD[leftLineIndex];
                            long r = orderLGD[leftLineIndex + 1];
                            long aboveL;
                            long aboveR;
                            long above;
                            long otherLine;
                            if (j % 2 == 0)
                            {
                                otherLine = orderLGD[j + 1];
                                aboveL = oldCornersPlus.Where(x => l == x.Item1.Item2).Select(x => x.Item1.Item1).Max();
                                aboveR = oldCornersPlus.Where(x => otherLine == x.Item1.Item2).Select(x => x.Item1.Item1).Max();
                                above = Math.Max(aboveL, aboveR);

                            }
                            else
                            {
                                otherLine = orderLGD[j - 1];
                                aboveL = oldCornersPlus.Where(x => otherLine == x.Item1.Item2).Select(x => x.Item1.Item1).Max();
                                aboveR = oldCornersPlus.Where(x => curLoc.Item2 == x.Item1.Item2).Select(x => x.Item1.Item1).Max();
                                above = Math.Max(aboveL, aboveR);
                                throw new Exception();
                            }
                            contents += (curLoc.Item1 - above - 1) * (Math.Abs(l - otherLine) - 1);
                            for (int k = 0; k < currentCorners.Count; k++)
                            {
                                if (l <= currentCorners[k].Item1.Item2 && currentCorners[k].Item1.Item2 <= r)
                                {
                                    skipList.Add(k);
                                }
                            }
                            oldCornersPlus.Add(((curLoc.Item1, l), "UL"));
                            oldCornersPlus.Add(((curLoc.Item1, r), "UR"));

                            while (l < r)
                            {
                                var overLapping = hors.Where(x => x.Item1 <= l && x.Item2 > l).ToList();
                                if (overLapping.Count > 0)
                                {
                                    l = overLapping[0].Item2;
                                    continue;
                                }

                                var ToOverLapping = hors.Where(x => x.Item1 >= l && x.Item1 < r).OrderBy(x => x.Item1).ToList();
                                if (ToOverLapping.Count > 0)
                                {
                                    contents += ToOverLapping[0].Item1 - l - 1;
                                    l = ToOverLapping[0].Item2;
                                    continue;
                                }
                                else
                                {
                                    contents += r - l - 1;
                                    break;
                                }
                            }
                        }
                    }
                }
            }


            for (int i = 0; i < currentCorners.Count; i++)
            {
                ((long, long) curLoc, string curCor) = currentCorners[i];
                if (curCor == "UL")
                {
                    linesGoingDown.Add(curLoc.Item2);
                }
                else if (curCor == "UR")
                {
                    linesGoingDown.Add(curLoc.Item2);
                }
                else if (curCor == "DL")
                {
                    linesGoingDown.Remove(curLoc.Item2);
                }
                else if (curCor == "DR")
                {
                    linesGoingDown.Remove(curLoc.Item2);
                }
            }

            oldCorners.AddRange(currentCorners);
            oldCornersPlus.AddRange(currentCorners);
        }
        L(trenchCount);

        return new ValueTask<string>($"Solution to {ClassPrefix} {contents + trenchCount}, part 1");
    }

    private string DictToString(Dictionary<(long, long), char> dict)
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

    private String LocationSetToString(HashSet<(long, long)> energized)
    {
        string r = "";
        for (int i = -1; i < 50; i++)
        {
            for (int j = -1; j < 50; j++)
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
