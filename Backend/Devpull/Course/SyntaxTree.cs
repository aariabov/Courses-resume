using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Devpull.Controllers;

public static class SyntaxTree
{
    /// <summary>
    /// Просто как пример
    /// </summary>
    /// <returns></returns>
    private static async Task<bool> Run()
    {
        try
        {
            var options = ScriptOptions.Default
                .AddImports(
                    "System",
                    "System.IO",
                    "System.Collections.Generic",
                    "System.Console",
                    "System.Diagnostics",
                    "System.Dynamic",
                    "System.Linq",
                    "System.Text",
                    "System.Threading.Tasks"
                )
                .AddReferences("System", "System.Core", "Microsoft.CSharp");
            string code1 =
                @"
            int Add(int a, int b)
    {
        var res = a + b;
        Console.WriteLine(res);
        return a + b;
    }
        ";
            var deleg = await CSharpScript.EvaluateAsync<Func<int, int, int>>(
                $"(a, b) => {{{code1} return Add(a, b);}}",
                options
            );
            Console.WriteLine(deleg(1, 2)); // 10
            Console.WriteLine(deleg(3, 2)); // 10

            var syntaxTree = CSharpSyntaxTree.ParseText(code1);
            string assemblyName = Path.GetRandomFileName();
            string assemblyPath =
                Path.GetDirectoryName(typeof(object).Assembly.Location)
                ?? throw new InvalidOperationException(
                    $"Не найдена директория для {typeof(object).Assembly.Location}"
                );

            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location)
            };
            references.Add(
                MetadataReference.CreateFromFile(
                    Path.Combine(assemblyPath, "System.Private.CoreLib.dll")
                )
            );
            references.Add(
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Console.dll"))
            );
            references.Add(
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll"))
            );

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            using (var stream = new MemoryStream())
            {
                var result = compilation.Emit(stream);

                if (result.Success)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    var assembly = Assembly.Load(stream.ToArray());

                    // Use reflection to call the dynamically created method
                    var type = assembly.GetType("DynamicClass");
                    var method = type!.GetMethod("Add");

                    _ = method!.Invoke(null, new object[] { 1, 2 });
                }
                else
                {
                    foreach (var diagnostic in result.Diagnostics)
                    {
                        Console.WriteLine(diagnostic);
                    }
                }
            }
        }
        catch (CompilationErrorException e)
        {
            Console.WriteLine(string.Join(Environment.NewLine, e.Diagnostics));
        }
        return true;
    }
}
