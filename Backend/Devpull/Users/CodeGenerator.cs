namespace Devpull.Users;

public interface ICodeGenerator
{
    string Generate();
}

public class CodeGenerator : ICodeGenerator
{
    public string Generate()
    {
        var random = new Random();
        var code = random.Next(100000, 999999).ToString();
        return code;
    }
}

public class FakeCodeGenerator : ICodeGenerator
{
    public string Generate()
    {
        return "111111";
    }
}
