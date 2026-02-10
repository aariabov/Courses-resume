using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using Devpull.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;

namespace Devpull.ClientCode;

public interface IClientCodeService
{
    Task<ClientCodeExecResult> Execute(
        ClientCodeModel model,
        CancellationToken cancellationToken,
        string? containerName = null
    );

    Task<DiagnosticMsg[]> Analyze(AnalyzeCodeModel model, CancellationToken cancellationToken);
}

public class ClientCodeService : IClientCodeService
{
    private static readonly ScriptOptions _options = ScriptOptions.Default.AddImports(
        "System",
        "System.Console"
    // "System.Collections.Generic",
    // "System.Linq",
    // "System.Text",
    // "System.Threading.Tasks"
    );

    // пока работает без этого
    // .AddReferences("System", "System.Core", "Microsoft.CSharp");

    private readonly ClientCodeModelValidator _clientCodeModelValidator;
    private readonly AnalyzeCodeModelValidator _analyzeCodeModelValidator;
    private readonly Devpull.Common.AppConfig _config;
    private static readonly string[] _usings = new[] { "System" };

    public ClientCodeService(
        ClientCodeModelValidator clientCodeModelValidator,
        AnalyzeCodeModelValidator analyzeCodeModelValidator,
        Devpull.Common.AppConfig config
    )
    {
        _clientCodeModelValidator = clientCodeModelValidator;
        _analyzeCodeModelValidator = analyzeCodeModelValidator;
        _config = config;
    }

    public async Task<ClientCodeExecResult> Execute(
        ClientCodeModel model,
        CancellationToken cancellationToken,
        string? containerName = null
    )
    {
        await _clientCodeModelValidator.Validate(model);

        if (string.IsNullOrWhiteSpace(containerName))
        {
            containerName = $"csharp-runner_{Guid.NewGuid()}";
        }

        // добавляем замену ввода в код
        var inputStr = string.Join(", ", model.Inputs.Select(i => $"\"{i}\""));
        var code =
            model.Code
            + @$"

public partial class Program
{{
    static int inputIndex = 0;
    static string[] inputs = new string[] {{ {inputStr} }};
    public static string GetInput_5DA3462468B540DAAD91B22858A68B6B(bool? newLine)
    {{
        if (inputIndex >= inputs.Length)
        {{
            Environment.Exit(1);
        }}

        if(newLine.HasValue)
        {{
            if (newLine == true)
            {{
                Console.WriteLine(inputs[inputIndex]);
            }}
            else
            {{
                Console.Write(inputs[inputIndex]);
            }}
        }}

        return inputs[inputIndex++];
    }}
}}";
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = await tree.GetRootAsync(cancellationToken);
        var rewriter = new DynamicReadLineRewriter();
        var newRoot = rewriter.Visit(root);
        var resultCode = newRoot.ToFullString();

        var timeout = _config.ClientCode.TimeoutInSeconds;
        var ramLimit = _config.ClientCode.RamLimitInMb;
        var cpuLimit = (double)_config.ClientCode.CpuInPercent / 100;

        var tempFile = Helpers.CreateTempFile(resultCode);
        var process = Helpers.CreateProcess(
            "csharp-runner",
            containerName,
            cpuLimit,
            ramLimit,
            tempFile
        );

        process.Start();

        // при большом выводе процесс блокируется, поэтому надо запускать так
        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
            await process.WaitForExitAsync(cts.Token);

            var output = await outputTask;
            var error = await errorTask;

            var exitCode = process.ExitCode;
            var result = ClientCodeExecResult.Create(exitCode, output, error, ramLimit);

            return result;
        }
        catch (OperationCanceledException)
        {
            process.Kill();
            var output = await outputTask;
            return ClientCodeExecResult.Create(
                102,
                output,
                $"Выполнение программы было отменено по таймауту {timeout} сек.",
                ramLimit
            );
        }
        finally
        {
            // явно удаляем контейнер, ибо флаг --rm удаляет контейнер, только если он сам успешно завершился
            await Helpers.DeleteContainer(containerName);
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
            process.Dispose();
        }
    }

    public async Task<DiagnosticMsg[]> Analyze(
        AnalyzeCodeModel model,
        CancellationToken cancellationToken
    )
    {
        await _analyzeCodeModelValidator.Validate(model);

        var script = CSharpScript.Create(model.Code, options: _options);
        var compilation = script.GetCompilation();
        var diagnostics = compilation.GetDiagnostics(cancellationToken);

        var errors = diagnostics
            .Select(
                d =>
                    new DiagnosticMsg
                    {
                        Message = d.GetMessage(),
                        Severity = d.Severity,
                        StartLine = d.Location.GetLineSpan().StartLinePosition.Line,
                        StartColumn = d.Location.GetLineSpan().StartLinePosition.Character,
                        EndLine = d.Location.GetLineSpan().EndLinePosition.Line,
                        EndColumn = d.Location.GetLineSpan().EndLinePosition.Character
                    }
            )
            .ToArray();

        return errors;
    }

    public async Task<CompletionItem[]> GetCompletion(
        CompletionRequest model,
        CancellationToken cancellationToken
    )
    {
        var code = model.Code;
        var position = model.Position;

        var host = MefHostServices.Create(MefHostServices.DefaultAssemblies);
        using var workspace = new AdhocWorkspace(host);

        var compilationOptions = new CSharpCompilationOptions(
            OutputKind.DynamicallyLinkedLibrary,
            usings: _usings
        );

        var scriptProjectInfo = ProjectInfo
            .Create(
                ProjectId.CreateNewId(),
                VersionStamp.Create(),
                "Script",
                "Script",
                LanguageNames.CSharp,
                isSubmission: true
            )
            .WithMetadataReferences(
                new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(Assembly.Load("System.Console").Location)
                }
            )
            .WithCompilationOptions(compilationOptions);

        var scriptProject = workspace.AddProject(scriptProjectInfo);
        var scriptDocumentInfo = DocumentInfo.Create(
            DocumentId.CreateNewId(scriptProject.Id),
            "Script",
            sourceCodeKind: SourceCodeKind.Script,
            loader: TextLoader.From(
                TextAndVersion.Create(SourceText.From(code), VersionStamp.Create())
            )
        );
        var scriptDocument = workspace.AddDocument(scriptDocumentInfo);

        var completionService = CompletionService.GetService(scriptDocument);
        var results = await completionService!.GetCompletionsAsync(
            scriptDocument,
            position,
            cancellationToken: cancellationToken
        );

        var typedSpan = results.Span;

        var sourceText = await scriptDocument.GetTextAsync(cancellationToken);
        var typedText = sourceText.GetSubText(typedSpan).ToString();

        return results.ItemsList
            .Where(
                item => item.DisplayText.StartsWith(typedText, StringComparison.OrdinalIgnoreCase)
            )
            .ToArray();
    }
}

sealed class DynamicReadLineRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode? VisitInvocationExpression(InvocationExpressionSyntax node)
    {
        if (node.ToString() == "Console.ReadLine()")
        {
            var expr = SyntaxFactory.ParseExpression(
                "Program.GetInput_5DA3462468B540DAAD91B22858A68B6B(true)"
            );
            return expr;
        }

        // if (node.ToString() == "Console.Read()" || node.ToString() == "Console.Read(false)")
        // {
        //     var expr = SyntaxFactory.ParseExpression("Program.GetInput_5DA3462468B540DAAD91B22858A68B6B(false)");
        //     return expr;
        // }

        if (node.ToString() == "Console.Read(true)")
        {
            var expr = SyntaxFactory.ParseExpression(
                "Program.GetInput_5DA3462468B540DAAD91B22858A68B6B(null)"
            );
            return expr;
        }

        return base.VisitInvocationExpression(node);
    }
}
