using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day07 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(int, int), string> _dict = new Dictionary<(int, int), string>();


    public Day07()
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
                if (_input[i][j] != '.')
                {
                    _dict.Add((i, j), "" + _input[i][j]);
                }
            }
        }
    }
    public override ValueTask<string> Solve_1()
    {
        var temp = _input.Select(x => x.Split(" "))
            .Select(x => (x[0].ToCharArray().ToList(), int.Parse(x[1])))
            .OrderBy(x => x, new ComparerC())
            .ToList();
        long sum = 0;
        for (int i = 0; i < temp.Count; i++ )
        {
            sum += (i + 1) * temp[i].Item2;
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {sum}, part 1");
    }

    public class ComparerC : IComparer<(List<char>, int)>
    {
        public ComparerC() { }
        //Implementing the Compare method
        public int Compare((List<char>, int) o1, (List<char>, int) o2)
        {
            var c1 = o1.Item1;
            var c2 = o2.Item1;

            var t1 = handValue(c1);
            var t2 = handValue(c2);
            if (t1 > t2)
            {
                return 1;
            }
            if (t1 < t2)
            {
                return -1;
            }
            var v1 = c1.Select(cardValue).ToList();
            var v2 = c2.Select(cardValue).ToList();
            for(int i  = 0; i < v1.Count; i++)
            {
                if (v1[i] != v2[i])
                {
                    return v1[i] - v2[i];
                }
            }
            return 0;
        }

        private int handValue(List<char> c1)
        {
            var countOfEachElement = c1
                                        .GroupBy(x => x, (x, grp) => (x, grp.Count()))
                                        .Select(x => x.Item2)
                                        .ToList();
            if (countOfEachElement.Contains(5))
            {
                return 6;
            };
            if (countOfEachElement.Contains(4))
            {
                return 5;
            }

            var order = c1.Select(cardValue).Order().ToList();

            if (countOfEachElement.Contains(2) && countOfEachElement.Contains(3) )
            {
                return 4;
            }
            if (countOfEachElement.Contains(3))
            {
                return 3;
            }
            if (countOfEachElement.Contains(2) && countOfEachElement.Count == 3)
            {
                return 2;
            }
            if (countOfEachElement.Contains(2))
            {
                return 1;
            }
            if (!countOfEachElement.Contains(2) && !countOfEachElement.Contains(3) && !countOfEachElement.Contains(4) && !countOfEachElement.Contains(5))
            {
                return 0;
            }
            throw new Exception();
        }

        private int cardValue(char c)
        {
            switch (c)
            {
                case 'A': return 14;
                case 'K': return 13;
                case 'Q': return 12;
                case 'J': return 11;
                case 'T': return 10;
                default: return int.Parse("" + c);
            }
        } 
    }

    public override ValueTask<string> Solve_2()
    {
        var temp = _input.Select(x => x.Split(" "))
            .Select(x => (x[0].ToCharArray().ToList(), int.Parse(x[1])))
            .OrderBy(x => x, new ComparerC2())
            .ToList();
        long sum = 0;
        for (int i = 0; i < temp.Count; i++)
        {
            sum += (i + 1) * temp[i].Item2;
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {sum}, part 2");
    }

    public class ComparerC2 : IComparer<(List<char>, int)>
    {
        public ComparerC2() { }
        //Implementing the Compare method
        public int Compare((List<char>, int) o1, (List<char>, int) o2)
        {
            var c1 = o1.Item1;
            var c2 = o2.Item1;

            var t1 = handValue(c1);
            var t2 = handValue(c2);
            if (t1 > t2)
            {
                return 1;
            }
            if (t1 < t2)
            {
                return -1;
            }
            var v1 = c1.Select(cardValue).ToList();
            var v2 = c2.Select(cardValue).ToList();
            for (int i = 0; i < v1.Count; i++)
            {
                if (v1[i] != v2[i])
                {
                    return v1[i] - v2[i];
                }
            }
            return 0;
        }

        private int handValue(List<char> c1)
        {
            var noJ = c1.Where(x => x != 'J').ToList();
            var Js = c1.Where(x => x == 'J').Count();
            if( Js >= 4)
            {
                return 6;
            }
            var countOfEachElement = noJ
                                        .GroupBy(x => x, (x, grp) => (x, grp.Count()))
                                        .Select(x => x.Item2)
                                        .ToList();

            if (countOfEachElement.Contains(5 - Js))
            {
                return 6;
            };
            if (countOfEachElement.Contains(4 - Js))
            {
                return 5;
            }

            if ((countOfEachElement.Contains(2) && countOfEachElement.Contains(3)) || (Js ==1 && countOfEachElement.Contains(2) && countOfEachElement.Count() == 2))
            {
                return 4;
            }
            if (countOfEachElement.Contains(3 - Js))
            {
                return 3;
            }
            if (countOfEachElement.Contains(2) && countOfEachElement.Count == 3)
            {
                return 2;
            }
            if (countOfEachElement.Contains(2 - Js))
            {
                return 1;
            }
            if (!countOfEachElement.Contains(2) && !countOfEachElement.Contains(3) && !countOfEachElement.Contains(4) && !countOfEachElement.Contains(5))
            {
                return 0;
            }
            throw new Exception();
        }

        private int cardValue(char c)
        {
            return c switch
            {
                'A' => 14,
                'K' => 13,
                'Q' => 12,
                'J' => 1,
                'T' => 10,
                _ => int.Parse("" + c),
            };
        }
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
