using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

ScriptOptions _options = ScriptOptions.Default
    .AddImports(
        "System",
        "System.Collections.Generic",
        "System.Console",
        "System.Diagnostics",
        "System.Dynamic",
        "System.Linq",
        "System.Text",
        "System.Threading.Tasks"
    )
    .AddReferences("System", "System.Core", "Microsoft.CSharp")
    .WithAllowUnsafe(false);

var scriptPath = args[0];

try
{
    var code = await File.ReadAllTextAsync(scriptPath, Encoding.UTF8);
    await CSharpScript.EvaluateAsync(code, _options);
    return 0;
}
catch (CompilationErrorException e)
{
    foreach (var diagnostic in e.Diagnostics)
    {
        await Console.Error.WriteLineAsync(diagnostic.ToString());
    }
    return 2;
}
catch (Exception ex)
{
    await Console.Error.WriteLineAsync($"{ex.GetType().FullName}: {ex.Message}");
    return 101;
}
