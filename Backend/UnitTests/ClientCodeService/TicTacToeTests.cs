using Devpull.ClientCode;
using Devpull.Common;
using Microsoft.Extensions.Configuration;

namespace UnitTests.ClientCodeService;

[TestClass]
public class TicTacToeTests
{
    [DataTestMethod]
    [DataRow(
        new string[] { },
        true,
        "|0| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: "
    )]
    [DataRow(
        new[] { "0" },
        true,
        "|0| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 0\n|Ю| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: "
    )]
    [DataRow(
        new[] { "0", "3" },
        true,
        "|0| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 0\n|Ю| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 3\n|Ю| |1| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: "
    )]
    [DataRow(
        new[] { "0", "3", "1" },
        true,
        "|0| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 0\n|Ю| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 3\n|Ю| |1| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 1\n|Ю| |Ю| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: "
    )]
    [DataRow(
        new[] { "0", "3", "1", "4" },
        true,
        "|0| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 0\n|Ю| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 3\n|Ю| |1| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 1\n|Ю| |Ю| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 4\n|Ю| |Ю| |2|\n|Д| |Д| |5|\n|6| |7| |8|\nИгрок Юля делает ход: "
    )]
    [DataRow(
        new[] { "0", "3", "1", "4", "2" },
        false,
        "|0| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 0\n|Ю| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 3\n|Ю| |1| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 1\n|Ю| |Ю| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 4\n|Ю| |Ю| |2|\n|Д| |Д| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 2\n|Ю| |Ю| |Ю|\n|Д| |Д| |5|\n|6| |7| |8|\nИгрок Юля победил!!!\nИгра закончена!"
    )]
    // [DataRow(new[] { "0", "3", "1" }, true, "")]
    // [DataRow(new[] { "0", "3", "1" }, true, "")]
    public async Task fast_game(string[] input, bool inProgress, string expectedOutput)
    {
        var sut = CreateSut();
        var result = await sut.Execute(
            new ClientCodeModel { Code = code, Inputs = input },
            CancellationToken.None
        );

        Assert.IsNull(result.Error, result.Error);
        Assert.IsNull(result.RuntimeError);
        Assert.IsNull(result.CompileError);
        Assert.AreEqual(expectedOutput, result.Result);
        Assert.AreEqual(result.InProgress, inProgress);
    }

    [DataTestMethod]
    [DataRow(
        new string[] { },
        true,
        "|0| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: "
    )]
    [DataRow(
        new[] { "0" },
        true,
        "|0| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 0\n|Ю| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: "
    )]
    [DataRow(
        new[] { "0", "3" },
        true,
        "|0| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 0\n|Ю| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 3\n|Ю| |1| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: "
    )]
    [DataRow(
        new[] { "0", "3", "1" },
        true,
        "|0| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 0\n|Ю| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 3\n|Ю| |1| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 1\n|Ю| |Ю| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: "
    )]
    [DataRow(
        new[] { "0", "3", "1", "4" },
        true,
        "|0| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 0\n|Ю| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 3\n|Ю| |1| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 1\n|Ю| |Ю| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 4\n|Ю| |Ю| |2|\n|Д| |Д| |5|\n|6| |7| |8|\nИгрок Юля делает ход: "
    )]
    [DataRow(
        new[] { "0", "3", "1", "4", "5" },
        true,
        "|0| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 0\n|Ю| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 3\n|Ю| |1| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 1\n|Ю| |Ю| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 4\n|Ю| |Ю| |2|\n|Д| |Д| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 5\n|Ю| |Ю| |2|\n|Д| |Д| |Ю|\n|6| |7| |8|\nИгрок Дима делает ход: "
    )]
    [DataRow(
        new[] { "0", "3", "1", "4", "5", "2" },
        true,
        "|0| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 0\n|Ю| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 3\n|Ю| |1| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 1\n|Ю| |Ю| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 4\n|Ю| |Ю| |2|\n|Д| |Д| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 5\n|Ю| |Ю| |2|\n|Д| |Д| |Ю|\n|6| |7| |8|\nИгрок Дима делает ход: 2\n|Ю| |Ю| |Д|\n|Д| |Д| |Ю|\n|6| |7| |8|\nИгрок Юля делает ход: "
    )]
    [DataRow(
        new[] { "0", "3", "1", "4", "5", "2", "6" },
        true,
        "|0| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 0\n|Ю| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 3\n|Ю| |1| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 1\n|Ю| |Ю| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 4\n|Ю| |Ю| |2|\n|Д| |Д| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 5\n|Ю| |Ю| |2|\n|Д| |Д| |Ю|\n|6| |7| |8|\nИгрок Дима делает ход: 2\n|Ю| |Ю| |Д|\n|Д| |Д| |Ю|\n|6| |7| |8|\nИгрок Юля делает ход: 6\n|Ю| |Ю| |Д|\n|Д| |Д| |Ю|\n|Ю| |7| |8|\nИгрок Дима делает ход: "
    )]
    [DataRow(
        new[] { "0", "3", "1", "4", "5", "2", "6", "7" },
        true,
        "|0| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 0\n|Ю| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 3\n|Ю| |1| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 1\n|Ю| |Ю| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 4\n|Ю| |Ю| |2|\n|Д| |Д| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 5\n|Ю| |Ю| |2|\n|Д| |Д| |Ю|\n|6| |7| |8|\nИгрок Дима делает ход: 2\n|Ю| |Ю| |Д|\n|Д| |Д| |Ю|\n|6| |7| |8|\nИгрок Юля делает ход: 6\n|Ю| |Ю| |Д|\n|Д| |Д| |Ю|\n|Ю| |7| |8|\nИгрок Дима делает ход: 7\n|Ю| |Ю| |Д|\n|Д| |Д| |Ю|\n|Ю| |Д| |8|\nИгрок Юля делает ход: "
    )]
    [DataRow(
        new[] { "0", "3", "1", "4", "5", "2", "6", "7", "8" },
        false,
        "|0| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 0\n|Ю| |1| |2|\n|3| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 3\n|Ю| |1| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 1\n|Ю| |Ю| |2|\n|Д| |4| |5|\n|6| |7| |8|\nИгрок Дима делает ход: 4\n|Ю| |Ю| |2|\n|Д| |Д| |5|\n|6| |7| |8|\nИгрок Юля делает ход: 5\n|Ю| |Ю| |2|\n|Д| |Д| |Ю|\n|6| |7| |8|\nИгрок Дима делает ход: 2\n|Ю| |Ю| |Д|\n|Д| |Д| |Ю|\n|6| |7| |8|\nИгрок Юля делает ход: 6\n|Ю| |Ю| |Д|\n|Д| |Д| |Ю|\n|Ю| |7| |8|\nИгрок Дима делает ход: 7\n|Ю| |Ю| |Д|\n|Д| |Д| |Ю|\n|Ю| |Д| |8|\nИгрок Юля делает ход: 8\n|Ю| |Ю| |Д|\n|Д| |Д| |Ю|\n|Ю| |Д| |Ю|\nИгра закончена!"
    )]
    public async Task no_winner_game(string[] input, bool inProgress, string expectedOutput)
    {
        var sut = CreateSut();
        var result = await sut.Execute(
            new ClientCodeModel { Code = code, Inputs = input },
            CancellationToken.None
        );

        Assert.AreEqual(expectedOutput, result.Result);
        Assert.IsNull(result.RuntimeError);
        Assert.IsNull(result.CompileError);
        Assert.IsNull(result.Error);
        Assert.AreEqual(result.InProgress, inProgress);
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

    const string code =
        @"string[] board = new string[] { ""0"", ""1"", ""2"", ""3"", ""4"", ""5"", ""6"", ""7"", ""8"" };

PrintBoard(board);

for (int i = 0; i < board.Length; i++)
{
    if (i % 2 == 0)
    {
        PlayerMakesMove(""Юля"");
        if (CheckIsWin(""Юля"", board))
        {
            Console.WriteLine(""Игрок Юля победил!!!"");
            break;
        }
    }
    else
    {
        PlayerMakesMove(""Дима"");
        if (CheckIsWin(""Дима"", board))
        {
            Console.WriteLine(""Игрок Дима победил!!!"");
            break;
        }
    }
}

Console.Write(""Игра закончена!"");

void PlayerMakesMove(string name)
{
    Console.Write($""Игрок {name} делает ход: "");
    string input = Console.ReadLine();
    int idx = Convert.ToInt32(input);
    board[idx] = name.Substring(0, 1);
    PrintBoard(board);
}

void PrintBoard(string[] board)
{
    Console.WriteLine($""|{board[0]}| |{board[1]}| |{board[2]}|"");
    Console.WriteLine($""|{board[3]}| |{board[4]}| |{board[5]}|"");
    Console.WriteLine($""|{board[6]}| |{board[7]}| |{board[8]}|"");
}

bool CheckIsWin(string name, string[] board)
{
    string chr = name.Substring(0, 1); // первая буква имени игрока
    if ((board[0] == chr && board[1] == chr && board[2] == chr) || // первая строка
        (board[3] == chr && board[4] == chr && board[5] == chr) || // вторая строка
        (board[6] == chr && board[7] == chr && board[8] == chr) || // третья строка
        (board[0] == chr && board[3] == chr && board[6] == chr) || // первый столбец
        (board[1] == chr && board[4] == chr && board[7] == chr) || // второй столбец
        (board[2] == chr && board[5] == chr && board[8] == chr) || // третий столбец
        (board[0] == chr && board[4] == chr && board[8] == chr) || // первая диагональ
        (board[2] == chr && board[4] == chr && board[6] == chr))   // вторая диагональ
    {
        return true;
    }

    return false;
}";
}
