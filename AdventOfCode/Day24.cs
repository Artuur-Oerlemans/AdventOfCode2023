using Microsoft.VisualBasic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day24 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(long, long), char> _dict;
    private readonly List<(long, long)> dirs = [(1, 0), (-1, 0), (0, 1), (0, -1)];
    private readonly long maxI;
    private readonly long maxJ;

    public Day24()
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
        var coords = _input.Select(x => x.Split(" @ "))
            .Select(x => (ToCoor(x[0]), ToCoor(x[1]))).ToList();

        for (int i = 0; i < coords.Count; i++)
        {
            (var pa, var va) = coords[i];
            for (int j = i + 1; j < coords.Count; j++)
            {
                (var pb, var vb) = coords[j];
                if (pb.Item1 * pa.Item2 == pb.Item2 * pa.Item1)
                {
                    L("parallel");
                }

                var pWithAScaler = Add(pa, Neg(pb));
                var pWithBScaler = Add(pb, Neg(pa));

                if (calcPositiveAScaler(pWithAScaler, va, vb) && calcPositiveAScaler(pWithBScaler, vb, va)
                    && calcIntersectInTest(pWithAScaler, va, vb, pa))
                {
                    result++;
                    //L(coords[i] + " " + coords[j]);
                }

            }
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 1");
    }

    bool calcPositiveAScaler((long, long) p, (long, long) va, (long, long) vb)
    {
        return (p.Item2 * vb.Item1 - vb.Item2 * p.Item1) > 0 == (vb.Item2 * va.Item1 - va.Item2 * vb.Item1) > 0;
    }

    bool postiveIntersect((long, long) pa, (long, long) pb, (long, long) va, (long, long) vb)
    {
        return calcPositiveAScaler(Add(pa, Neg(pb)), va, vb) && calcPositiveAScaler(Add(pb, Neg(pa)), vb, va);
    }

    long lowerBound = 200000000000000;
    long upperBound = 400000000000000;

    //long lowerBound = 7;
    //long upperBound = 27;

    bool calcIntersectInTest((long, long) p, (long, long) va, (long, long) vb, (long, long) pa)
    {
        if (vb.Item2 * va.Item1 - va.Item2 * vb.Item1 == 0)
        {
            return false;
        }

        decimal scaler = ((decimal)(p.Item2 * vb.Item1 - vb.Item2 * p.Item1)) / ((decimal)(vb.Item2 * va.Item1 - va.Item2 * vb.Item1));

        var x = pa.Item1 + va.Item1 * scaler;
        var y = pa.Item2 + va.Item2 * scaler;
        return x >= lowerBound && x <= upperBound && y >= lowerBound && y <= upperBound;
    }

    //decimal calcScaler((long, long) pa, (long, long) pb, (long, long) va, (long, long) vb)
    //{
    //    var p = Add(pa, Neg(pb));
    //    if (vb.Item2 * va.Item1 - va.Item2 * vb.Item1 == 0)
    //    {
    //        return -1;
    //    }

    //    return ((decimal)(p.Item2 * vb.Item1 - vb.Item2 * p.Item1)) / ((decimal)(vb.Item2 * va.Item1 - va.Item2 * vb.Item1));
    //}

    decimal calcScaler(long pa, long pb, long va, long vb)
    {
        return ((decimal)(pa - pb)) / ((decimal)(vb - va));
    }

    (long, long) ToCoor(string s)
    {
        var temp = s.Split(", ").Select(long.Parse).ToList();
        return (temp[0], temp[1]);
    }

    (long, long, long) ToCooor(string s)
    {
        var temp = s.Split(", ").Select(long.Parse).ToList();
        return (temp[0], temp[1], temp[2]);
    }

    public override ValueTask<string> Solve_2()
    {
        long result = 0;
        var coords = _input.Select(x => x.Split(" @ "))
            .Select(x => (ToCooor(x[0]), ToCooor(x[1]))).OrderBy(x => x.Item1).Take(3).ToList();

        for (long st = 1; st < 1000000000; st++)
        {
            for (int h = 0; h < coords.Count; h++)
            {
                (var pa, var va) = coords[h];
                var firstHit = Add(pa, Mul(va, st));

                int max = 400;
                if (Math.Abs(pa.Item1 - 309991770591665L) < 50000L && Math.Abs(pa.Item2 - 460585296453281L) < 50000L)
                {
                    L("reached");
                }
                Dictionary<long, Dictionary<int, decimal>> posisbleI = GetPosibleI(coords, h, firstHit, max, st);

                if (posisbleI.Count == 0)
                {
                    continue;
                }
                Dictionary<long, Dictionary<int, decimal>> posisbleJ = GetPosibleJ(coords, h, firstHit, max, st);

                if (posisbleJ.Count == 0)
                {
                    continue;
                }

                foreach (long i in posisbleI.Keys)
                {
                    foreach (long j in posisbleJ.Keys)
                    {
                        bool allScalerHitij = true;
                        foreach (var oi in posisbleJ[j].Keys)
                        {
                            if (!posisbleI[i].ContainsKey(oi))
                            {
                                continue;
                            }
                            allScalerHitij = Math.Abs(posisbleI[i][oi] - posisbleJ[j][oi]) < (decimal)0.0001;
                            if (!allScalerHitij)
                            {
                                break;
                            }
                        }
                        if (!allScalerHitij)
                        {
                            continue;
                        }

                        Dictionary<long, Dictionary<int, decimal>> posisbleK = GetPosibleK(coords, h, firstHit, max, st);

                        if (posisbleK.Count == 0)
                        {
                            continue;
                        }

                        foreach (long k in posisbleK.Keys)
                        {
                            bool allScalerHitjk = true;
                            foreach (var oi in posisbleJ[j].Keys)
                            {
                                if (!posisbleK[k].ContainsKey(oi))
                                {
                                    continue;
                                }
                                allScalerHitjk = Math.Abs(posisbleK[k][oi] - posisbleJ[j][oi]) < (decimal)0.1;
                                if (!allScalerHitjk)
                                {
                                    break;
                                }
                            }
                            if (allScalerHitjk)
                            {
                                var start = Add(firstHit, Neg((i, j, k)));
                                result = start.Item1 + start.Item2 + start.Item3;
                                L(result);
                            }
                        }
                    }
                }
            }
        }

        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 1");
    }

    private Dictionary<long, Dictionary<int, decimal>> GetPosibleI(List<((long, long, long), (long, long, long))> coords, int h, (long, long, long) firstHit, int max, long time)
    {
        return GetPosible(coords.Select(x => (x.Item1.Item1, x.Item2.Item1)).ToList(), h, firstHit.Item1, max, time);
    }

    private Dictionary<long, Dictionary<int, decimal>> GetPosibleJ(List<((long, long, long), (long, long, long))> coords, int h, (long, long, long) firstHit, int max, long time)
    {
        return GetPosible(coords.Select(x => (x.Item1.Item2, x.Item2.Item2)).ToList(), h, firstHit.Item2, max, time);
    }

    private Dictionary<long, Dictionary<int, decimal>> GetPosibleK(List<((long, long, long), (long, long, long))> coords, int h, (long, long, long) firstHit, int max, long time)
    {
        return GetPosible(coords.Select(x => (x.Item1.Item3, x.Item2.Item3)).ToList(), h, firstHit.Item3, max, time);
    }

    private Dictionary<long, Dictionary<int, decimal>> GetPosible(List<(long, long)> coords, int h, long firstHit, int max, long time)
    {
        Dictionary<long, Dictionary<int, decimal>> posisbleJ = [];

        for (long j = -max; j < max; j++)
        {
            int failCount = 0;
            int maxFail = 1;
            Dictionary<int, decimal> scalers = [];
            for (int oi = 0; oi < coords.Count && failCount < maxFail; oi++)
            {
                if (oi == h)
                {
                    continue;
                }
                var ini = firstHit - j * time;
                long pb = coords[oi].Item1;
                long vb = coords[oi].Item2;

                if (pb == ini && vb == j)
                {
                    continue;
                }
                else if (pb != ini && vb == j)
                {
                    failCount++;
                    break;
                }
                decimal scale = calcScaler(pb, ini, vb, j);
                scalers[oi] = scale;
                if (scale <= 0)
                {
                    failCount++;

                    if ((pb > ini == vb < j) && ini != pb)
                    {
                        L("something wrong");
                    }
                }
            }

            if (failCount < maxFail)
            {
                posisbleJ[j] = scalers;
            }
        }

        return posisbleJ;
    }

    //private Dictionary<long, Dictionary<int, decimal>> GetPosible(List<(long, long)> coords, int h, long firstHit, int max)
    //{
    //    Dictionary<long, Dictionary<int, decimal>> posisbleJ = [];

    //    for (long j = -max; j < max; j++)
    //    {
    //        bool allHitj = true;
    //        Dictionary<int, decimal> scalers = [];
    //        for (int oi = 0; oi < coords.Count && allHitj; oi++)
    //        {
    //            if (oi == h)
    //            {
    //                continue;
    //            }
    //            var ini = firstHit - j;
    //            if (coords[oi].Item1 == ini && coords[oi].Item2 == j)
    //            {
    //                allHitj = true;
    //                continue;
    //            }
    //            else if (coords[oi].Item1 != ini && coords[oi].Item2 == j)
    //            {
    //                allHitj = false;
    //                break;
    //            }
    //            scalers[oi] = calcScaler(coords[oi].Item1, ini, coords[oi].Item2, j);
    //            allHitj = scalers[oi] > 0;
    //        }

    //        if (allHitj)
    //        {
    //            posisbleJ[j] = scalers;
    //        }
    //    }

    //    return posisbleJ;
    //}

    public ValueTask<string> Solve_2_old()
    {
        long result = 0;
        var coords = _input.Select(x => x.Split(" @ "))
            .Select(x => (ToCooor(x[0]), ToCooor(x[1]))).ToList();

        Dictionary<int, Dictionary<(long, long, long), int>> position = [];

        for (int i = 1; i < 100000; i++)
        {
            position[i] = [];
            for (int j = 0; j < coords.Count; j++)
            {
                (var pa, var va) = coords[j];
                var totV = Mul(va, i);
                position[i][Add(pa, totV)] = j;
            }
        }

        int tried = 0;

        foreach (var first in position[1].Keys)
        {

            for (int ti = 2; ti < 100000; ti++)
            {
                foreach (var second in position[ti].Keys)
                {
                    HashSet<int> hails = [position[1][first], position[ti][second]];
                    if (hails.Count == 1)
                    {
                        continue;
                    }

                    var totV = Add(second, Neg(first));
                    if (totV.Item1 % (ti - 1) != 0 || totV.Item2 % (ti - 1) != 0 || totV.Item3 % (ti - 1) != 0)
                    {
                        continue;
                    }
                    var v = (totV.Item1 / (ti - 1), totV.Item2 / (ti - 1), totV.Item3 / (ti - 1));
                    if (Math.Abs(v.Item1) > 200000000000000 / 300 || Math.Abs(v.Item2) > 200000000000000 / 300 || Math.Abs(v.Item3) > 200000000000000 / 300)
                    {
                        continue;
                    }
                    var curPos = second;
                    tried++;
                    L("tried " + tried);
                    for (int t = ti + 1; t < position.Count; t++)
                    {
                        curPos = Add(curPos, v);
                        if (position[t].ContainsKey(curPos))
                        {
                            hails.Add(position[t][curPos]);
                        }
                    }

                    if (hails.Count > 2)
                    {
                        var start = Add(first, Neg(v));
                        result = start.Item1 + start.Item2 + start.Item3;
                        L(result);
                    }
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

    private (long, long, long) Add((long, long, long) a, (long, long, long) b)
    {
        return (a.Item1 + b.Item1, a.Item2 + b.Item2, a.Item3 + b.Item3);
    }

    private (long, long, long) Mul((long, long, long) a, long x)
    {
        return (a.Item1 * x, a.Item2 * x, a.Item3 * x);
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

    private (long, long, long) Neg((long, long, long) p)
    {
        return (-p.Item1, -p.Item2, -p.Item3);
    }
}
