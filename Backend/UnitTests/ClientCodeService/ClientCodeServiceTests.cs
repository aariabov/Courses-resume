using Devpull.ClientCode;
using Devpull.Common;
using Microsoft.Extensions.Configuration;

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]

namespace UnitTests.ClientCodeService;

[TestClass]
public class ClientCodeServiceTests
{
    // интерполяция объявление и вывод массивов
    // функции
    // цикл for
    // break
    // continue
    // игра в крестики-нолики
    // ограничить размер файла

    [TestMethod]
    public async Task comments()
    {
        var expected = "Привет!";
        var code = $@"
/*
Это многострочный комментарий.
Он может занимать несколько строк.
*/
Console.Write(""{expected}""); // Комментарий после кода";

        var sut = CreateSut();
        var result = await sut.Execute(
            new ClientCodeModel { Code =  code},
            CancellationToken.None
        );
        Assert.AreEqual(expected, result.Result);
        Assert.IsNull(result.RuntimeError);
        Assert.IsNull(result.CompileError);
        Assert.IsNull(result.Error);
        Assert.IsFalse(result.InProgress);
    }

    [TestMethod]
    public async Task compile_error()
    {
        var code = "int number = \"123\";";
        var expectedCompileError = "(1,14): error CS0029: Cannot implicitly convert type 'string' to 'int'";

        var sut = CreateSut();
        var result = await sut.Execute(
            new ClientCodeModel { Code =  code},
            CancellationToken.None
        );
        Assert.IsNull(result.Result);
        Assert.IsNull(result.RuntimeError);
        Assert.IsNull(result.Error);
        Assert.AreEqual(expectedCompileError, result.CompileError!.Trim());
        Assert.IsFalse(result.InProgress);
    }

    [TestMethod]
    public async Task runtime_error()
    {
        var code =
@"int x = 10;
int y = 0;
int result = x / y;";
        var expectedRuntimeError = "System.DivideByZeroException: Attempted to divide by zero.";

        var sut = CreateSut();
        var result = await sut.Execute(
            new ClientCodeModel { Code =  code},
            CancellationToken.None
        );
        Assert.AreEqual(string.Empty, result.Result);
        Assert.AreEqual(expectedRuntimeError, result.RuntimeError!.Trim());
        Assert.IsNull(result.CompileError);
        Assert.IsNull(result.Error);
        Assert.IsFalse(result.InProgress);
    }

    [TestMethod]
    public async Task runtime_error_with_output()
    {
        var expectedOutput = "Привет!";
        var code =
            $@"
Console.Write(""{expectedOutput}"");
int x = 10;
int y = 0;
int result = x / y;";
        var expectedRuntimeError = "System.DivideByZeroException: Attempted to divide by zero.";

        var sut = CreateSut();
        var result = await sut.Execute(
            new ClientCodeModel { Code =  code},
            CancellationToken.None
        );
        Assert.AreEqual(expectedOutput, result.Result);
        Assert.AreEqual(expectedRuntimeError, result.RuntimeError!.Trim());
        Assert.IsNull(result.CompileError);
        Assert.IsNull(result.Error);
        Assert.IsFalse(result.InProgress);
    }

    [TestMethod]
    public async Task print_msg()
    {
        var expected = "Привет!";

        var sut = CreateSut();
        var result = await sut.Execute(
            new ClientCodeModel { Code = $"Console.Write(\"{expected}\");" },
            CancellationToken.None
        );
        Assert.IsNull(result.Error, result.Error);
        Assert.AreEqual(expected, result.Result);
        Assert.IsNull(result.RuntimeError);
        Assert.IsNull(result.CompileError);
        Assert.IsFalse(result.InProgress);
    }

    [TestMethod]
    public async Task print_multiline_msg()
    {
        var hi = "Привет!";
        var world = "Мир!";
        var expected = $"{hi}\n{world}\n";

        var sut = CreateSut();
        var result = await sut.Execute(
            new ClientCodeModel { Code = $"Console.WriteLine(\"{hi}\");Console.WriteLine(\"{world}\");" },
            CancellationToken.None
        );
        Assert.AreEqual(expected, result.Result);
        Assert.IsNull(result.RuntimeError);
        Assert.IsNull(result.CompileError);
        Assert.IsNull(result.Error);
        Assert.IsFalse(result.InProgress);
    }

    [TestMethod]
    public async Task print_msg_and_read()
    {
        var expected = "Введите имя: ";

        var sut = CreateSut();
        var result = await sut.Execute(
            new ClientCodeModel
            {
                Code = $"Console.Write(\"{expected}\");string input = Console.ReadLine();"
            },
            CancellationToken.None
        );
        Assert.AreEqual(expected, result.Result);
        Assert.IsNull(result.RuntimeError);
        Assert.IsNull(result.CompileError);
        Assert.IsNull(result.Error);
        Assert.IsTrue(result.InProgress);
    }

    [TestMethod]
    public async Task run_to_user_input()
    {
        var expected = "Введите имя: ";
        var userInput = "Дима";

        var sut = CreateSut();
        var result = await sut.Execute(
            new ClientCodeModel
            {
                Code =
                    $"Console.Write(\"{expected}\");string input = Console.ReadLine();Console.Write(\"Привет, {userInput}\");"
            },
            CancellationToken.None
        );
        Assert.AreEqual(expected, result.Result);
        Assert.IsNull(result.RuntimeError);
        Assert.IsNull(result.CompileError);
        Assert.IsNull(result.Error);
        Assert.IsTrue(result.InProgress);
    }

    [TestMethod]
    public async Task print_user_input()
    {
        var prompt = "Введите имя: ";
        var userInput = "Дима";
        var hi = "Привет";
        var expected = $"{prompt}{userInput}\n{hi}, {userInput}";

        var sut = CreateSut();
        var result = await sut.Execute(
            new ClientCodeModel
            {
                Code =
                    $"Console.Write(\"{prompt}\");string input = Console.ReadLine();Console.Write(\"{hi}, {userInput}\");",
                Inputs = [userInput]
            },
            CancellationToken.None
        );
        Assert.AreEqual(expected, result.Result);
        Assert.IsNull(result.RuntimeError);
        Assert.IsNull(result.CompileError);
        Assert.IsNull(result.Error);
        Assert.IsFalse(result.InProgress);
    }

    [TestMethod]
    public async Task print_vars()
    {
        var name = "Иванов Иван";
        var age = 20;
        var isStudent = true;
        var expected = $"{name}\n{age}\n{isStudent}\n";
        var code =
            $@"
string name = ""{name}"";
int age = {age};
bool isStudent = {isStudent.ToString().ToLower()};

Console.WriteLine(name);
Console.WriteLine(age);
Console.WriteLine(isStudent);";

        var sut = CreateSut();
        var result = await sut.Execute(
            new ClientCodeModel
            {
                Code = code
            },
            CancellationToken.None
        );
        Assert.AreEqual(expected, result.Result);
        Assert.IsNull(result.RuntimeError);
        Assert.IsNull(result.CompileError);
        Assert.IsNull(result.Error);
        Assert.IsFalse(result.InProgress);
    }

    private static Devpull.ClientCode.ClientCodeService CreateSut()
    {
        var appConfig = new AppConfig
        {
            ClientCode =
            {
                TimeoutInSeconds = 14,
                RamLimitInMb = 256,
                CpuInPercent = 50
            }
        };

        return new Devpull.ClientCode.ClientCodeService(
            new ClientCodeModelValidator(),
            new AnalyzeCodeModelValidator(),
            appConfig
        );
    }

    //     [TestMethod]
    //     public async Task client_can_delete_file()
    //     {
    //         var file = "DeleteFile.txt";
    //         if (!File.Exists(file))
    //         {
    //             File.Create(file).Dispose();
    //         }
    //
    //         var sut = new Devpull.ClientCode.ClientCodeService(
    //             new ClientCodeModelValidator(),
    //             new AnalyzeCodeModelValidator()
    //         );
    //         var result = await sut.Execute(
    //             new ClientCodeModel { Code = $"Console.WriteLine(\"Готово!\");" },
    //             CancellationToken.None
    //         );
    //         Assert.IsFalse(File.Exists(file));
    //     }
    //
    //     [TestMethod]
    //     public async Task generate_big_files()
    //     {
    //         var file = "BigFile.txt";
    //         if (File.Exists(file))
    //         {
    //             File.Delete(file);
    //         }
    //
    //         var sut = new Devpull.ClientCode.ClientCodeService(
    //             new ClientCodeModelValidator(),
    //             new AnalyzeCodeModelValidator()
    //         );
    //         var result = await sut.Execute(
    //             new ClientCodeModel
    //             {
    //                 Code = $"System.IO.File.WriteAllBytes(\"{file}\", new byte[1024]);"
    //             },
    //             CancellationToken.None
    //         );
    //         Assert.IsTrue(File.Exists(file));
    //     }
    //
    //     [TestMethod]
    //     public async Task run_cmd()
    //     {
    //         var file = "BigFile.txt";
    //         if (File.Exists(file))
    //         {
    //             File.Delete(file);
    //         }
    //
    //         var sut = new Devpull.ClientCode.ClientCodeService(
    //             new ClientCodeModelValidator(),
    //             new AnalyzeCodeModelValidator()
    //         );
    //         var result = await sut.Execute(
    //             new ClientCodeModel
    //             {
    //                 Code =
    //                     @"
    //     using System;
    //     using System.Diagnostics;
    //
    //     var process = new Process();
    //     process.StartInfo.FileName = ""cmd.exe"";
    //     process.StartInfo.Arguments = ""/c dir"";
    //     process.StartInfo.RedirectStandardOutput = true;
    //     process.StartInfo.UseShellExecute = false;
    //     process.StartInfo.CreateNoWindow = true;
    //
    //     process.Start();
    //     var output = process.StandardOutput.ReadToEnd();
    //     process.WaitForExit();
    //
    //     Console.WriteLine(output);
    // "
    //             },
    //             CancellationToken.None
    //         );
    //         Assert.IsNotNull(result.Result);
    //     }
    //
    //     [TestMethod]
    //     public async Task infinite_loop()
    //     {
    //         var sut = new Devpull.ClientCode.ClientCodeService(
    //             new ClientCodeModelValidator(),
    //             new AnalyzeCodeModelValidator()
    //         );
    //
    //         var result = await sut.Execute(
    //             new ClientCodeModel { Code = "await Task.Delay(5000);Console.WriteLine(\"Готово!\");" },
    //             CancellationToken.None
    //         );
    //         Assert.IsNotNull(result);
    //     }

    // while (true) {}

    // for (int i = 0; i < 10000; i++) {
    //     new System.Threading.Thread(() => { while(true){} }).Start();
    // }

    // var list = new List<byte[]>(); while(true) list.Add(new byte[1024*1024]);
}
