using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day13 : BaseDay
{
    private readonly List<string> _input;

    public Day13()
    {
        _input =
        [
            .. File.ReadAllText(string.Concat(InputFilePath.AsSpan(0, InputFilePath.Length - 4)
//, "test"
, ".txt")
                        ).Split("\r\n\r\n")
,
        ];

    }
    public override ValueTask<string> Solve_1()
    {
        long result = 0;
        foreach( var f in _input)
        {
            List<string> input = f.Split("\r\n").ToList();
            Dictionary<(int, int), string> _dict = new Dictionary<(int, int), string>();
            int maxI = input.Count;
            int maxJ = input[0].Length;
            for (int i = 0; i < input.Count; i++)
            {
                for (int j = 0; j < input[i].Length; j++)
                {
                    _dict.Add((i, j), "" + input[i][j]);
                }
            }

            for(int i = 0; i < maxI -1; i++)
            {
                if (input[i] == input[i + 1])
                {
                    long temp = i + 1;
                    bool isMirror = true;
                    for(int d = 2; i + d < maxI && i +1 -d >= 0; d++ )
                    {
                        isMirror = isMirror && input[i + 1 - d] == input[i + d];
                    }
                    if ( isMirror)
                    {
                        result += 100 * temp;
                        break;
                    }
                }
            }

            for (int j = 0; j < maxJ - 1; j++)
            {
                bool isSomeMirror = true;
                for(int i = 0; i < maxI; i++)
                {
                    isSomeMirror = isSomeMirror && _dict[(i, j)] == _dict[(i, j + 1)];
                }
                if( isSomeMirror )
                {
                    long temp = j + 1;
                    bool isMirror = true;
                    for (int d = 2; j + d < maxJ && j + 1 - d >= 0; d++)
                    {

                        for (int i = 0; i < maxI; i++)
                        {
                            isMirror = isMirror && _dict[(i, j + 1 - d)] == _dict[(i, j + d)];
                        }
                    }
                    if (isMirror)
                    {
                        result += temp;
                        break;
                    }
                }
            }

        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 1");
    }

    public override ValueTask<string> Solve_2()
    {
        long result = 0;
        foreach (var f in _input)
        {
            List<string> input = f.Split("\r\n").ToList();
            Dictionary<(int, int), string> _dict = new Dictionary<(int, int), string>();
            int maxI = input.Count;
            int maxJ = input[0].Length;
            for (int i = 0; i < input.Count; i++)
            {
                for (int j = 0; j < input[i].Length; j++)
                {
                    _dict.Add((i, j), "" + input[i][j]);
                }
            }

            for (int i = 0; i < maxI - 1; i++)
            {
                int hasSmudge = 0;
                for (int j = 0; j < maxJ; j++)
                {
                    var equa = _dict[(i, j)] == _dict[(i + 1, j)];
                    if (!equa)
                    {
                        hasSmudge++;
                    }
                }
                if (hasSmudge <= 1)
                {
                    long temp = i + 1;
                    for (int d = 2; i + d < maxI && i + 1 - d >= 0; d++)
                    {
                        for (int j = 0; j < maxJ; j++)
                        {
                            var equa = _dict[(i + 1 - d, j)] == _dict[(i + d, j)];
                            if (!equa)
                            {
                                hasSmudge++;
                            }
                        }
                    }
                    if (hasSmudge == 1)
                    {
                        result += 100 *  temp;
                        break;
                    }
                }
            }

            for (int j = 0; j < maxJ - 1; j++)
            {
                int hasSmudge = 0;
                for (int i = 0; i < maxI; i++)
                {
                    var equa = _dict[(i, j)] == _dict[(i, j + 1)];
                    if ( !equa)
                    {
                        hasSmudge++;
                    }
                }
                if (hasSmudge <= 1)
                {
                    long temp = j + 1;
                    for (int d = 2; j + d < maxJ && j + 1 - d >= 0; d++)
                    {
                        for (int i = 0; i < maxI; i++)
                        {

                            var equa = _dict[(i, j + 1 - d)] == _dict[(i, j + d)];
                            if (!equa)
                            {
                                hasSmudge++;
                            }
                        }
                    }
                    if (hasSmudge == 1)
                    {
                        result += temp;
                        break;
                    }
                }
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
