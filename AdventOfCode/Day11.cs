using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day11 : BaseDay
{
    private readonly List<string> _input;


    public Day11()
    {
        _input =
        [
            .. File.ReadAllText(string.Concat(InputFilePath.AsSpan(0, InputFilePath.Length - 4)
//, "test"
, ".txt")
                        ).Split("\r\n")
,
        ];

    }
    public override ValueTask<string> Solve_1()
    {
        var list = _input.Select(x => x.ToList()).ToList();
        for(int i = 0; i < list.Count; i++)
        {
            if (list[i].All(x => x == '.'))
            {
                list.Insert(i, list[i]);
                i++;
            }
        }
        for(int j = 0; j < list[0].Count; j++)
        {
            bool allDot = true;
            for(int i = 0;i < list.Count; i++)
            {
                allDot = list[i][j] == '.' && allDot;
            }
            if(allDot)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Insert(j, '.');
                }
                j++;
            }
        }
        Dictionary<(int, int), string> dict = new Dictionary<(int, int), string>();
        int maxI = list.Count;
        int maxJ = list[0].Count;
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list[i].Count; j++)
            {
                if (list[i][j] != '.')
                {
                    dict.Add((i, j), "" + list[i][j]);
                }
            }
        }
        var gals = dict.Keys.ToList();
        int sum = 0;
        for(int i = 0; i < gals.Count; i++) { 
            for( int j = i + 1; j < gals.Count; j++)
            {
                sum += ManDis(gals[i], gals[j]);
            }
        }

        return new ValueTask<string>($"Solution to {ClassPrefix} {sum}, part 1");
    }

    public override ValueTask<string> Solve_2()
    {
        int expand = 999999;
        var cols = new HashSet<int>();
        var rows = new HashSet<int>();
        var list = _input.Select(x => x.ToList()).ToList();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].All(x => x == '.'))
            {
                rows.Add(i);
            }
        }
        for (int j = 0; j < list[0].Count; j++)
        {
            bool allDot = true;
            for (int i = 0; i < list.Count; i++)
            {
                allDot = list[i][j] == '.' && allDot;
            }
            if (allDot)
            {
                cols.Add(j);
            }
        }
        Dictionary<(int, int), string> dict = new Dictionary<(int, int), string>();
        int maxI = list.Count;
        int maxJ = list[0].Count;
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list[i].Count; j++)
            {
                if (list[i][j] == '#')
                {
                    dict.Add((i, j), "" + list[i][j]);
                }
            }
        }
        var gals = dict.Keys.ToList();
        long sum = 0;
        for (int i = 0; i < gals.Count; i++)
        {
            for (int j = i + 1; j < gals.Count; j++)
            {
                sum += ManDis(gals[i], gals[j]); ;
                foreach(int col in cols)
                {
                    if ((gals[i].Item2 < col && col < gals[j].Item2) || (gals[j].Item2 < col && col < gals[i].Item2))
                    {
                        sum += expand;
                    }
                }

                foreach (int row in rows)
                {
                    if ((gals[i].Item1 < row && row < gals[j].Item1) || (gals[j].Item1 < row && row < gals[i].Item1))
                    {
                        sum += expand;
                    }
                }
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

    private int ManDis((int, int) a, (int, int) b)
    {
        return Math.Abs(a.Item1 - a.Item1) + Math.Abs(b.Item2 - b.Item2);
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
