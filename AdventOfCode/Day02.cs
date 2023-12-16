using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day02 : BaseDay
{
    private readonly string _input;

    public Day02()
    {
        _input = File.ReadAllText(string.Concat(InputFilePath.AsSpan(0, InputFilePath.Length - 4)
//, "test"
, ".txt")
            );
    }

    public override ValueTask<string> Solve_1()
    {
        Regex re = new(@".* (\d+): (.+)$");
        var sumIds = _input.Split("\r\n")
            .ToList()
            .Select(s => re.Match(s).Groups)
            .Select(g => (int.Parse(g[1].Value), g[2].Value))
            .Where(x => Filter(x.Item2))
            .Select(x => x.Item1)
            .ToList()
            .Sum();
        return new ValueTask<string>($"Solution to {ClassPrefix} {sumIds}, part 1");
    }

    public bool Filter(string input)
    {
        return input.Split("; ")
            .All(FilterSub);
    }

    public bool FilterSub(string input)
    {
        Regex re = new(@"^(\d+) (\w+)$");
        return input.Split(", ")
            .Select(s => re.Match(s).Groups)
            .Select(g => (g[2].Value, int.Parse(g[1].Value)))
            .All(x => Individual(x.Item1, x.Item2));
    }

    public bool Individual(string color, int amount)
    {
        switch (color)
        {
            case "red":
                return amount <= 12;
            case "green":
                return amount <= 13;
            case "blue":
                return amount <= 14;
            default: throw new ArgumentException("what color ?" + color);
        }
    }

    public override ValueTask<string> Solve_2()
    {
        Regex re = new(@".* (\d+): (.+)$");
        var sumPowers = _input.Split("\r\n")
            .ToList()
            .Select(s => re.Match(s).Groups)
            .Select(g => (int.Parse(g[1].Value), g[2].Value))
            .Select(x => Power(x.Item2))
            .Sum();
        return new ValueTask<string>($"Solution to {ClassPrefix} {sumPowers}, part 2");
    }

    public int Power(string input)
    {
        var temp = input.Split("; ")
            .Select(FilterSub2)
            .ToList();
        for(int i = 1;  i < temp.Count; i++)
        {
            foreach (var key in temp[i].Keys)
            {
                if (!temp[0].ContainsKey(key))
                {
                    temp[0][key] = 0;
                }
                if (temp[i][key] > temp[0][key])
                {
                    temp[0][key] = temp[i][key];
                }
            }
        }
        return temp[0]["red"] * temp[0]["blue"] * temp[0]["green"];
    }

    public Dictionary<string, int> FilterSub2(string input)
    {
        Regex re = new(@"^(\d+) (\w+)$");
        return input.Split(", ")
            .Select(s => re.Match(s).Groups)
            .ToDictionary(g => g[2].Value, g => int.Parse(g[1].Value));
    }

    private void L(Object o)
    {
        Console.WriteLine(o.ToString());
    }

    private List<(int, String)> ReadListTuple(string input)
    {
        string temps =
@"1 A
2 B
3 C";
        return temps.Split("\r\n").Select(s => (int.Parse("" + s[0]), "" + s[2])).ToList();
    }

    private Dictionary<int, (int, String)> ReadDicTuple(string input)
    {
        string temps =
@"1 A
2 B
3 C";
        return temps.Split("\r\n").Select(s => (int.Parse("" + s[0]), "" + s[2])).ToDictionary(t => t.Item1, t => t);
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
