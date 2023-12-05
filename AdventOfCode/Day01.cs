using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day01 : BaseDay
{
    private readonly string _input;

    public Day01()
    {
        _input = File.ReadAllText(string.Concat(InputFilePath.AsSpan(0, InputFilePath.Length - 4)
//, "test"
, ".txt")
            );
    }

    public override ValueTask<string> Solve_1()
    {
        int suma = 0;
        var temp = _input.Split("\r\n")
            .ToList();
        foreach (var t in temp)
        {
            string output = "";
            for (var i = 0; i < t.Length; i++)
            {
                var re = new Regex(@"\d");
                var match = re.Match("" + t[i]);
                if (match.Success)
                {
                    output += t[i];
                    break;
                }
            }
            for (var i = t.Length - 1; i >= 0; i--)
            {
                var re = new Regex(@"\d");
                var match = re.Match("" + t[i]);
                if (match.Success)
                {
                    output += t[i];
                    break;
                }
            }
            suma += int.Parse(output);
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {suma}, part 1");
    }

    public override ValueTask<string> Solve_2()
    {
        int suma = 0;
        var temp = _input.Split("\r\n")
            .ToList();
        foreach (var t in temp)
        {
            string output = "";
            for (var i = 0; i < t.Length; i++)
            {
                var nq = IsNumber(t, i);
                if (nq != "")
                {
                    output += nq;
                    break;
                }
            }
            for (var i = t.Length - 1; i >= 0; i--)
            {

                var nq = IsNumber(t, i);
                if (nq != "")
                {
                    output += nq;
                    break;
                }
            }
            suma += int.Parse(output);
        }
        return new ValueTask<string>($"Solution to {ClassPrefix} {suma}, part 2");
    }

    private string IsNumber(string t, int i)
    {
        var re = new Regex(@"\d");
        var match = re.Match("" + t[i]);
        if (match.Success)
        {
            return "" + t[i];
        }
        var sub = t.Substring(i);
        if (sub.StartsWith("one"))
        { return "1"; }
        if (sub.StartsWith("two"))
        { return "2"; }
        if (sub.StartsWith("three"))
        { return "3"; }
        if (sub.StartsWith("four"))
        { return "4"; }
        if (sub.StartsWith("five"))
        { return "5"; }
        if (sub.StartsWith("six"))
        { return "6"; }
        if (sub.StartsWith("seven"))
        { return "7"; }
        if (sub.StartsWith("eight"))
        { return "8"; }
        if (sub.StartsWith("nine"))
        { return "9"; }
        
        return "";
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
