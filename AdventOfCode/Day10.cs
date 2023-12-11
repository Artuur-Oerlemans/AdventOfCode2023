using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text.RegularExpressions;
using static System.Reflection.Metadata.BlobBuilder;

namespace AdventOfCode;

public class Day10 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(int, int), string> _dict = new Dictionary<(int, int), string>();
    private readonly int maxI;
    private readonly int maxJ;

    public Day10()
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
        var start = _dict.First(x => x.Value == "S").Key;
        int steps = 0;
        (int, int) current = start;
        (int, int) dir = (0, 1);
        do
        {
            switch (_dict[current].ToCharArray()[0])
            {
                case 'S':
                case '7':
                    if( dir == (0, 1))
                    {
                        dir = (1, 0);
                    } else
                    {
                        dir = (0, -1);
                    }
                    break;
                case '|':
                    if (dir == (1, 0))
                    {
                        dir = (1, 0);
                    }
                    else
                    {
                        dir = (-1, 0);
                    }
                    break;
                case '-':
                    if (dir == (0,1))
                    {
                        dir = (0,1);
                    }
                    else
                    {
                        dir = (0, -1);
                    }
                    break;
                case 'L':
                    if (dir == (1,0))
                    {
                        dir = (0, 1);
                    }
                    else
                    {
                        dir = (-1, 0);
                    }
                    break;
                case 'J':
                    if (dir == (1,0))
                    {
                        dir = (0, -1);
                    }
                    else
                    {
                        dir = (-1, 0);
                    }
                    break;
                case 'F':
                    if (dir == (-1, 0))
                    {
                        dir = (0, 1);
                    }
                    else
                    {
                        dir = (1,0);
                    }
                    break;

                default: throw new Exception();
            }
            current = Add(current, dir);
            steps++;
        } while (current != start);
        return new ValueTask<string>($"Solution to {ClassPrefix} {steps/2}, part 1");
    }

    public override ValueTask<string> Solve_2()
    {
        var start = _dict.First(x => x.Value == "S").Key;
        int steps = 0;
        (int, int) current = start;
        (int, int) dir = (0, 1);
        Dictionary<(int, int), (int, int)> dirAt = new Dictionary<(int, int), (int, int)>();
        Dictionary<(int, int), (int, int)> dirFrom = new Dictionary<(int, int), (int, int)>();
        do
        {
            var oldDir = dir;
            switch (_dict[current].ToCharArray()[0])
            {
                case 'S':
                case '7':
                    if (dir == (0, 1))
                    {
                        dir = (1, 0);
                    }
                    else
                    {
                        dir = (0, -1);
                    }
                    break;
                case '|':
                    if (dir == (1, 0))
                    {
                        dir = (1, 0);
                    }
                    else
                    {
                        dir = (-1, 0);
                    }
                    break;
                case '-':
                    if (dir == (0, 1))
                    {
                        dir = (0, 1);
                    }
                    else
                    {
                        dir = (0, -1);
                    }
                    break;
                case 'L':
                    if (dir == (1, 0))
                    {
                        dir = (0, 1);
                    }
                    else
                    {
                        dir = (-1, 0);
                    }
                    break;
                case 'J':
                    if (dir == (1, 0))
                    {
                        dir = (0, -1);
                    }
                    else
                    {
                        dir = (-1, 0);
                    }
                    break;
                case 'F':
                    if (dir == (-1, 0))
                    {
                        dir = (0, 1);
                    }
                    else
                    {
                        dir = (1, 0);
                    }
                    break;

                default: throw new Exception();
            }
            if (!dirAt.ContainsKey(current))
            {
                dirAt.Add(current, dir);
            }
            if (!dirFrom.ContainsKey(current))
            {
                dirFrom.Add(current, oldDir);
            }
            current = Add(current, dir);
            steps++;
        } while (current != start);

        int enclosed = 0;
        for( int i = 0; i < maxI; i++)
        {
            int hits = 0;
            bool hitting = false;
            var verts = new List<(int, int)>();
            for ( int j = 0; j < maxJ; j++)
            {
                if (!hitting && dirAt.ContainsKey((i, j)))
                {
                    verts = new List<(int, int)>();
                    if (dirFrom[(i, j)].Item1 != 0)
                    {
                        verts.Add(dirFrom[(i, j)]);
                    }
                    if (dirAt[(i, j)].Item1 != 0)
                    {
                        verts.Add(dirAt[(i, j)]);
                    }
                } else if (hitting && dirAt.ContainsKey((i, j)))
                {
                    if (dirAt[(i, j)].Item2 != 0 && dirFrom[(i, j -1)].Item2 != 0 && dirAt[(i, j)] == dirFrom[(i, j - 1)])
                    {

                    } else if (dirFrom[(i, j)].Item2 != 0 && dirAt[(i, j - 1)].Item2 != 0 && dirFrom[(i, j)] == dirAt[(i, j - 1)]) {

                    } else
                    {
                        if (verts.Count == 2)
                        {
                            hits++;
                        }
                        else if (dirFrom[(i, j - 1)].Item1 != 0)
                        {
                            if (!verts.Contains((-dirFrom[(i, j - 1)].Item1, 0)))
                            {
                                hits++;
                            }
                        }
                        else if (dirAt[(i, j - 1)].Item1 != 0)
                        {
                            if (!verts.Contains((-dirAt[(i, j - 1)].Item1, 0)))
                            {
                                hits++;
                            }
                        }
                        verts = new List<(int, int)>();
                        if (dirFrom[(i, j)].Item1 != 0)
                        {
                            verts.Add(dirFrom[(i, j)]);
                        }
                        if (dirAt[(i, j)].Item1 != 0)
                        {
                            verts.Add(dirAt[(i, j)]);
                        }
                    }
                } else if (hitting && !dirAt.ContainsKey((i, j)))
                {
                    if(verts.Count == 2)
                    {
                        hits++;
                    } else if (dirFrom[(i, j-1)].Item1 != 0)
                    {
                        if(!verts.Contains((-dirFrom[(i, j - 1)].Item1, 0)))
                        {
                            hits++;
                        }
                    }
                    else if (dirAt[(i, j-1)].Item1 != 0)
                    {
                        if (!verts.Contains((-dirAt[(i, j - 1)].Item1, 0)))
                        {
                            hits++;
                        }
                    }
                } 
                hitting = dirAt.ContainsKey((i, j));
                if( hits % 2 == 1 && !hitting)
                {
                    enclosed++;
                }
            }
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {enclosed}, part 2");
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
