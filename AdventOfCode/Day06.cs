using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day06 : BaseDay
{
    private readonly List<string> _input;
    private readonly Dictionary<(int, int), string> _dict = new Dictionary<(int, int), string>();

    public Day06()
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
        var times = _input[0].Split(": ")[1].Split(" ").Select(x => long.Parse(x)).ToList();
        var distances = _input[1].Split(": ")[1].Split(" ").Select(x => long.Parse(x)).ToList();

        long result = 1;
        for(int i = 0; i < times.Count; i++)
        {
            long fasterTimes = 0;
            for(long j = 0; j < times[i]; j++)
            {
                if( j * (times[i] - j) > distances[i])
                {
                    fasterTimes++;
                }
            }
            result *= fasterTimes;
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 1");
    }

    public override ValueTask<string> Solve_2()
    {
        var times = _input[0].Split(": ")[1].Replace(" ", "").Split(" ").Select(x => long.Parse(x)).ToList();
        var distances = _input[1].Split(": ")[1].Replace(" ", "").Split(" ").Select(x => long.Parse(x)).ToList();

        long result = 1;
        for (int i = 0; i < times.Count; i++)
        {
            long fasterTimes = 0;
            for (long j = 0; j < times[i]; j++)
            {
                if (j * (times[i] - j) > distances[i])
                {
                    fasterTimes++;
                }
            }
            result *= fasterTimes;
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {result}, part 2");
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
