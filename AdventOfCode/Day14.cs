using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day14 : BaseDay
{
    private readonly List<string> _input;
    private Dictionary<(int, int), string> _dict = new Dictionary<(int, int), string>();
    private readonly int maxI;
    private readonly int maxJ;

    public Day14()
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
        for(int i = 0; i < maxI; i++)
        {
            for(int j = 0; j < maxJ; j++)
            {
                if (_dict.ContainsKey((i, j)) && _dict[(i, j)] == "O")
                {
                    int pos = i;
                    while (pos -1 >= 0 && !_dict.ContainsKey((pos - 1, j)))
                    {
                        _dict.Remove((pos, j));
                        _dict[(pos - 1, j)] = "O";
                        pos -= 1;
                    }

                    bool leans = true;
                    for(int sup = pos - 1; sup >= 0; sup--)
                    {
                        if(!_dict.ContainsKey((sup, j)))
                        {
                            L("bug");
                        }
                        if (_dict[(sup, j)] == "#")
                        {
                            leans = false;
                            break;
                        }
                    }
                    if(true)
                    {
                        result += maxI - pos;
                    }
                }
            }
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 1");
    }

    public override ValueTask<string> Solve_2()
    {
        long result = 0;
        _dict = new Dictionary<(int, int), string>();
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
        Dictionary<string, int> states = new Dictionary<string, int>();
        for(int cycles = 1000000000 -1; cycles >= 0; cycles--)
        {
            RollNorth();
            RollWest();
            RollSouth();
            RollEast();
            string state = DictToString();
            if (states.ContainsKey(state))
            {
                cycles = cycles % (states[state] - cycles);
            } else
            {
                states[state] = cycles;
            }
        }

        for (int i = 0; i < maxI; i++)
        {
            for (int j = 0; j < maxJ; j++)
            {
                if (_dict.ContainsKey((i, j)) && _dict[(i, j)] == "O")
                {
                    int pos = i; 
                    result += maxI - pos;
                }
            }
        }
        L(DictToString());
        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 2");
    }

    string DictToString()
    {
        string r = "";
        for(int i = 0; i < maxI; i++)
        {
            for(int j =0; j < maxJ; j++)
            {
                if(_dict.ContainsKey((i,j)))
                {
                    r += _dict[(i, j)];
                } else
                {
                    r += '.';
                }
            }
            r += "\r\n";
        }
        return r;
    }

    void RollNorth()
    {
        for (int i = 0; i < maxI; i++)
        {
            for (int j = 0; j < maxJ; j++)
            {
                if (_dict.ContainsKey((i, j)) && _dict[(i, j)] == "O")
                {
                    int pos = i;
                    while (pos - 1 >= 0 && !_dict.ContainsKey((pos - 1, j)))
                    {
                        _dict.Remove((pos, j));
                        _dict[(pos - 1, j)] = "O";
                        pos -= 1;
                    }
                }
            }
        }
    }

    void RollSouth()
    {
        for (int i = maxI -1; i >= 0; i--)
        {
            for (int j = 0; j < maxJ; j++)
            {
                if (_dict.ContainsKey((i, j)) && _dict[(i, j)] == "O")
                {
                    int pos = i;
                    while (pos + 1 < maxI && !_dict.ContainsKey((pos + 1, j)))
                    {
                        _dict.Remove((pos, j));
                        _dict[(pos + 1, j)] = "O";
                        pos += 1;
                    }
                }
            }
        }
    }

    void RollWest()
    {
        for (int j = 0; j < maxJ; j++)
        {
            for (int i = 0; i < maxI; i++)
            {
                if (_dict.ContainsKey((i, j)) && _dict[(i, j)] == "O")
                {
                    int pos = j;
                    while (pos - 1 >= 0 && !_dict.ContainsKey((i, pos - 1)))
                    {
                        _dict.Remove((i, pos));
                        _dict[(i, pos - 1)] = "O";
                        pos -= 1;
                    }
                }
            }
        }
    }

    void RollEast()
    {
        for (int j = maxJ -1; j >= 0; j--)
        {
            for (int i = 0; i < maxI; i++)
            {
                if (_dict.ContainsKey((i, j)) && _dict[(i, j)] == "O")
                {
                    int pos = j;
                    while (pos + 1 < maxJ && !_dict.ContainsKey((i, pos + 1)))
                    {
                        _dict.Remove((i, pos));
                        _dict[(i, pos + 1)] = "O";
                        pos += 1;
                    }
                }
            }
        }
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
