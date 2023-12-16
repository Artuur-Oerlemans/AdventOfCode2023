using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day09 : BaseDay
{
    private readonly List<string> _input;


    public Day09()
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
        var lines = _input.Select(x => x.Split(" ").Select(x => long.Parse(x)).ToList())
            .ToList();
        long sum = 0;
        foreach (var line in lines)
        {
            var reLine = CreateRecursionLine(line);
            sum += line[line.Count - 1] + reLine[reLine.Count - 1];
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {sum}, part 1");
    }

    private List<long> CreateRecursionLine(List<long> line)
    {
        List<long> reLine = new List<long>();
        for (int i = 1; i < line.Count; i++)
        {
            reLine.Add(line[i] - line[i - 1]);
        }
        if (reLine.All(x => x == reLine[0]))
        {
            reLine.Add(reLine[0]);
            return reLine;
        }
        var rereLine = CreateRecursionLine(reLine);
        reLine.Add(reLine[reLine.Count - 1] + rereLine[rereLine.Count - 1]);
        return reLine;
    }

    public override ValueTask<string> Solve_2()
    {
        var lines = _input.Select(
                                 x => x.Split(" ")
                                .Select(x => long.Parse(x))
                                .ToList())
            .ToList();
        long sum = 0;
        foreach (var line in lines)
        {
            var reLine = CreateReverse(line);
            sum += line[0] - reLine[0];
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {sum}, part 2");
    }

    private List<long> CreateReverse(List<long> line)
    {
        List<long> reLine = new List<long>();
        for (int i = 1; i < line.Count; i++)
        {
            reLine.Add(line[i] - line[i - 1]);
        }
        if (reLine.All(x => x == reLine[0]))
        {
            reLine.Insert(0, reLine[0]);
            return reLine;
        }
        var rereLine = CreateReverse(reLine);
        reLine.Insert(0, reLine[0] - rereLine[0]);
        return reLine;
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
