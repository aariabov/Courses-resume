namespace Devpull.ClientCode;

public class CompletionRequest
{
    public string Code { get; set; } = string.Empty;
    public int Position { get; set; }
}
