namespace Devpull.ClientCode;

public class ClientCodeCacheService : IClientCodeService
{
    private readonly ICacheService _cacheService;
    private readonly IClientCodeService _clientCodeService;
    private readonly ILogger<ClientCodeCacheService> _logger;

    public ClientCodeCacheService(
        ICacheService cacheService,
        IClientCodeService clientCodeService,
        ILogger<ClientCodeCacheService> logger
    )
    {
        _cacheService = cacheService;
        _clientCodeService = clientCodeService;
        _logger = logger;
    }

    public async Task<DiagnosticMsg[]> Analyze(
        AnalyzeCodeModel model,
        CancellationToken cancellationToken
    )
    {
        const string cacheKeyPrefix = "Analyze";

        if (!string.IsNullOrWhiteSpace(model.Code) && ShouldCache(model.Code))
        {
            var resultFromCache = _cacheService.GetItem<DiagnosticMsg[]>(
                $"{cacheKeyPrefix}_{model.Code}"
            );
            if (resultFromCache != null)
            {
                return resultFromCache;
            }
        }

        var result = await _clientCodeService.Analyze(model, cancellationToken);

        if (ShouldCache(model.Code))
        {
            _cacheService.SetItem($"{cacheKeyPrefix}_{model.Code}", result);
        }

        return result;
    }

    public async Task<ClientCodeExecResult> Execute(
        ClientCodeModel model,
        CancellationToken cancellationToken,
        string? containerName = null
    )
    {
        const string cacheKeyPrefix = "Execute";

        var shouldCache = model.Inputs.Length == 0 && ShouldCache(model.Code);
        if (shouldCache)
        {
            var resultFromCache = _cacheService.GetItem<ClientCodeExecResult>(
                $"{cacheKeyPrefix}_{model.Code}"
            );
            if (resultFromCache != null)
            {
                LogResult(model.Code, isFromCache: true, resultFromCache);
                return resultFromCache;
            }
        }

        var result = await _clientCodeService.Execute(model, cancellationToken);

        if (result.IsNormalCode && shouldCache)
        {
            _cacheService.SetItem($"{cacheKeyPrefix}_{model.Code}", result);
        }

        LogResult(model.Code, isFromCache: false, result);

        return result;
    }

    private static readonly Action<
        ILogger,
        bool,
        string,
        ClientCodeExecResult,
        Exception?
    > _logInfo = LoggerMessage.Define<bool, string, ClientCodeExecResult>(
        LogLevel.Information,
        new EventId(1, nameof(LogResult)),
        "FromCache: {FromCache} Client code: {ClientCode}, Result: {@Result}"
    );

    private static readonly Action<
        ILogger,
        bool,
        string,
        ClientCodeExecResult,
        Exception?
    > _logWarning = LoggerMessage.Define<bool, string, ClientCodeExecResult>(
        LogLevel.Warning,
        new EventId(2, nameof(LogResult)),
        "FromCache: {FromCache} Client code: {ClientCode}, Result: {@Result}"
    );

    private static readonly Action<
        ILogger,
        bool,
        string,
        ClientCodeExecResult,
        Exception?
    > _logError = LoggerMessage.Define<bool, string, ClientCodeExecResult>(
        LogLevel.Error,
        new EventId(3, nameof(LogResult)),
        "FromCache: {FromCache} Client code: {ClientCode}, Result: {@Result}"
    );

    private void LogResult(string code, bool isFromCache, ClientCodeExecResult result)
    {
        if (result.IsNormalCode)
        {
            _logInfo(_logger, isFromCache, code, result, null);
        }
        else if (result.IsExpectedError)
        {
            _logWarning(_logger, isFromCache, code, result, null);
        }
        else
        {
            _logError(_logger, isFromCache, code, result, null);
        }
    }

    private static bool ShouldCache(string code)
    {
        var cachedCode = new HashSet<string>()
        {
            "Console.WriteLine(\"Hello, World!\");",
            "int number = \"123\"; // Ошибка: Невозможно присвоить строку переменной типа int",
            "Console.WriteLine(\"Быстрее\");\r\nConsole.WriteLine(\"Выше\");\r\nConsole.WriteLine(\"Сильнее\");",
            "int number = \"123\"; // Ошибка: Невозможно присвоить строку переменной типа int",
            "int x = 10;\r\nint y = 0;\r\nint result = x / y; // Ошибка: деление на ноль",
            "int x = 5;\r\nif (x > 0)\r\n{\r\n    Console.WriteLine(\"x меньше 0\"); // Ошибка, должно быть: x больше 0\r\n}",
            "int number = \"123\";\r\nConsole.WriteLine(number);",
            "int x = 10;\r\nint y = 0;\r\nint result = x / y;\r\nConsole.WriteLine(result);",
            "int x = 5;\r\nif (x > 0)\r\n{\r\n    Console.WriteLine(\"x меньше 0\");\r\n}",
            "Console.Write(\"Привет \");\r\nConsole.Write(\"Мир!\");",
            "Console.WriteLine(\"Привет\");\r\nConsole.WriteLine(\"Мир!\");",
            "string name = \"Иванов Иван\";\r\nConsole.WriteLine(name);",
            "string name = \"Иванов Иван\";\r\nConsole.WriteLine(name);\r\n\r\nname = \"Ваня Сидоров\";\r\nConsole.WriteLine(name);",
            "string name = \"Иванов Иван\";\r\nint age = 20;\r\nbool isStudent = true;\r\n\r\nConsole.WriteLine(name);\r\nConsole.WriteLine(age);\r\nConsole.WriteLine(isStudent);",
            "string name = \"Иванов Иван\";\r\nint age = 20;\r\nbool isStudent = true;\r\n\r\nConsole.WriteLine($\"Имя: {name}\");\r\nConsole.WriteLine($\"Возраст: {age}\");\r\nConsole.WriteLine($\"Является студентом: {isStudent}\");\r\nConsole.WriteLine($\"Имя: {name}, Возраст: {age}, Является студентом: {isStudent}\");",
            "int[] numbers = { 1, 2, 3, 4, 5 };\r\n\r\n// получение первого элемента\r\nint firstNumber = numbers[0];\r\nConsole.WriteLine(firstNumber);\r\n\r\n// получение второго элемента\r\nint secondNumber = numbers[1];\r\nConsole.WriteLine(secondNumber);",
            "int[] numbers = { 1, 2, 3, 4, 5 };\r\n// получение и вывод первого элемента\r\nConsole.WriteLine(numbers[0]);\r\n\r\n// изменение первого элемента\r\nnumbers[0] = 11;\r\nConsole.WriteLine(numbers[0]);",
            "string[] board = new string[9];\r\n\r\nConsole.WriteLine($\"|{board[0]}| |{board[1]}| |{board[2]}|\");\r\nConsole.WriteLine($\"|{board[3]}| |{board[4]}| |{board[5]}|\");\r\nConsole.WriteLine($\"|{board[6]}| |{board[7]}| |{board[8]}|\");",
            "string[] board = new string[] { \"0\", \"1\", \"2\", \"3\", \"4\", \"5\", \"6\", \"7\", \"8\" };\r\n\r\nConsole.WriteLine($\"|{board[0]}| |{board[1]}| |{board[2]}|\");\r\nConsole.WriteLine($\"|{board[3]}| |{board[4]}| |{board[5]}|\");\r\nConsole.WriteLine($\"|{board[6]}| |{board[7]}| |{board[8]}|\");",
            "void Hello()\r\n{\r\n    Console.WriteLine(\"Привет!\");\r\n}\r\n\r\nHello(); // вызов функции\r\nHello(); // повторный вызов функции",
            "void Hello(string name)\r\n{\r\n    Console.WriteLine($\"Привет, {name}!\");\r\n}\r\n\r\nHello(\"Иван\");\r\nHello(\"Мария\");",
            "string Hello()\r\n{\r\n    return \"Привет!\";\r\n}\r\n\r\n// объявление переменной и присваивание ей результата функции\r\nstring helloString = Hello();\r\nConsole.WriteLine(helloString);",
            "string Hello(string name)\r\n{\r\n    return $\"Привет, {name}!\";\r\n}\r\n\r\n// можно сразу использовать результат функции, не объявляя переменную\r\nConsole.WriteLine(Hello(\"Иван\"));\r\nConsole.WriteLine(Hello(\"Мария\"));",
            "int Add(int a, int b)\r\n{\r\n    int sum = a + b;\r\n    return sum;\r\n}\r\n\r\nvar result = Add(1, 2);\r\nConsole.WriteLine(result);\r\n\r\n// или сразу использовать результат функции, не объявляя переменную\r\nConsole.WriteLine(Add(2, 2));",
            "bool isMan = false;\r\nbool isWoman = true;\r\nbool hasChildren = true;\r\n\r\n// мама - это женщина И у нее есть дети\r\nbool isMom = isWoman && hasChildren;\r\nConsole.WriteLine(isMom); // True\r\n\r\n// папа - это мужчина И у него есть дети\r\nbool isDad = isMan && hasChildren;\r\nConsole.WriteLine(isMan); // False",
            "bool hasSon = false;\r\nbool hasDaughter = true;\r\n\r\n// родитель - есть сын ИЛИ есть дочь\r\nbool isParent = hasSon || hasDaughter;\r\nConsole.WriteLine(isParent); // True",
            "bool isWeekend = true;\r\n\r\n// будни - если НЕ выходной\r\nbool isWorkday = !isWeekend;\r\nConsole.WriteLine(isWorkday); // False",
            "bool isStudent = true;\r\n\r\nif (isStudent)\r\n{\r\n    Console.WriteLine(\"Надо сдавать экзамены.\");\r\n}",
            "bool isStudent = true;\r\n\r\nif (isStudent)\r\n{\r\n    Console.WriteLine(\"Надо сдавать экзамены.\");\r\n}\r\nelse\r\n{\r\n    Console.WriteLine(\"Надо работать.\");\r\n}",
            "bool isSunny = true; // солнечно ли сегодня\r\nbool isWeekend = false; // выходной ли сегодня\r\n\r\n// солнечно И выходной\r\nif (isSunny && isWeekend)\r\n{\r\n    Console.WriteLine(\"Отличное время для пикника!\");\r\n}\r\n\r\n// солнечно ИЛИ выходной\r\nif (isSunny || isWeekend)\r\n{\r\n    Console.WriteLine(\"Можно выйти на прогулку.\");\r\n}\r\n\r\n// сегодня НЕ выходной - будний день\r\nif (!isWeekend)\r\n{\r\n    Console.WriteLine(\"Сегодня будний день.\");\r\n}",
            "for (int i = 0; i < 5; i++)\r\n{\r\n    Console.WriteLine(\"Значение i: \" + i);\r\n}",
            "string[] fruits = new string[] { \"яблоко\", \"банан\", \"вишня\" };\r\nfor (int i = 0; i < fruits.Length; i++) // fruits.Length - количество элементов массива\r\n{\r\n    var fruit = fruits[i];\r\n    Console.WriteLine(fruit);\r\n}",
            "for (int i = 0; i < 10; i++)\r\n{\r\n    if (i == 5)\r\n        break;\r\n    Console.WriteLine(\"i: \" + i);\r\n}",
            "for (int i = 0; i < 5; i++)\r\n{\r\n    if (i == 2)\r\n        continue;\r\n    Console.WriteLine(\"i: \" + i);\r\n}"
        };
        return cachedCode.Contains(code);
    }
}
