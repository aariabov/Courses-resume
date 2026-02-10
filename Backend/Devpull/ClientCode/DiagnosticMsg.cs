using Microsoft.CodeAnalysis;

namespace Devpull.ClientCode;

public class DiagnosticMsg
{
    public required string Message { get; init; }
    public required int StartLine { get; init; }
    public required int StartColumn { get; init; }
    public required int EndLine { get; init; }
    public required int EndColumn { get; init; }
    public required DiagnosticSeverity Severity { get; init; }
}
