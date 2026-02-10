using Common;
using Devpull.Common;
using Devpull.Course;
using Devpull.DbModels;
using Devpull.Exercises.Models;
using Microsoft.Playwright;
using ExerciseDto = Devpull.Exercises.Models.ExerciseDto;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Devpull.Exercises;

public class ExerciseService
{
    private readonly FunctionTestModelValidator _functionTestModelValidator;
    private readonly ExerciseRepository _exerciseRepository;
    private readonly AuthService _authService;
    private readonly Devpull.Common.AppConfig _config;

    public ExerciseService(
        FunctionTestModelValidator functionTestModelValidator,
        ExerciseRepository exerciseRepository,
        AuthService authService,
        Devpull.Common.AppConfig config
    )
    {
        _functionTestModelValidator = functionTestModelValidator;
        _exerciseRepository = exerciseRepository;
        _authService = authService;
        _config = config;
    }

    public Task<ExerciseRegistryRecord[]> GetAll(CancellationToken cancellationToken)
    {
        var userId = _authService.GetUserId();
        return _exerciseRepository.GetAll(userId, cancellationToken);
    }

    public async Task<FunctionTestingResultView> Test(
        FunctionTestModel model,
        CancellationToken cancellationToken
    )
    {
        await _functionTestModelValidator.Validate(model);

        var timeout = _config.Exercise.TimeoutInSeconds;
        var ramLimit = _config.Exercise.RamLimitInMb;
        var cpuLimit = (double)_config.Exercise.CpuInPercent / 100;

        var result = await RunDockerContainer(
            model.ExerciseId.ToString(),
            model.Code,
            cpuLimit,
            ramLimit,
            timeout
        );

        var userId = _authService.GetUserIdOrThrow();
        var resultDoc = JsonSerializer.SerializeToDocument(result);
        var runExercise = new RunExercise
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ExerciseId = model.ExerciseId,
            Code = model.Code,
            Result = resultDoc,
            Date = DateTime.UtcNow
        };
        await _exerciseRepository.InsertRunExecute(runExercise, cancellationToken);

        return new FunctionTestingResultView(result);
    }

    public static async Task<FunctionTestingResult> RunDockerContainer(
        string exerciseId,
        string code,
        double cpuLimit,
        int ramLimit,
        int timeout
    )
    {
        var tempFile = Helpers.CreateTempFile(code);
        var containerName = $"csharp-tester_{Guid.NewGuid()}";
        var process = Helpers.CreateProcess(
            "csharp-tester",
            containerName,
            cpuLimit,
            ramLimit,
            tempFile,
            exerciseId
        );

        FunctionTestingResult result;

        try
        {
            process.Start();

            // при большом выводе процесс блокируется, поэтому надо запускать так
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
            await process.WaitForExitAsync(cts.Token);

            var output = await outputTask;
            var error = await errorTask;

            var exitCode = process.ExitCode;
            if (exitCode == 0)
            {
                var lastLine = output.Split("\n", StringSplitOptions.RemoveEmptyEntries).Last();
                result = JsonSerializer.Deserialize<FunctionTestingResult>(lastLine)!;
            }
            else
            {
                result = FunctionTestingResult.CreateError(exitCode, ramLimit);
            }
        }
        catch (OperationCanceledException)
        {
            process.Kill();
            result = FunctionTestingResult.CreateTimeoutError(
                $"Выполнение программы было отменено по таймауту {timeout} сек."
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

        return result;
    }

    public async Task<bool> Test1()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions { Headless = false }
        );
        var page = await browser.NewPageAsync(); // Открываем страницу задачи
        await page.GotoAsync("https://leetcode.com/problems/add-two-integers/description/");

        // Ждем появления заголовка
        await page.WaitForSelectorAsync("div.text-title-large a");
        var title = await page.InnerTextAsync("div.text-title-large a");

        // Ждем появления описания
        await page.WaitForSelectorAsync("div.elfjS[data-track-load='description_content']");
        var description = await page.InnerHTMLAsync(
            "div.elfjS[data-track-load='description_content']"
        );

        Console.WriteLine($"Заголовок: {title}");
        Console.WriteLine($"Описание: {description}");
        return true;
    }

    public async Task<ExerciseDto> GetByUrl(string url, CancellationToken cancellationToken)
    {
        var e = await _exerciseRepository.GetByUrl(url, cancellationToken);
        return new ExerciseDto
        {
            Id = e.Id,
            Description = e.Description,
            LevelId = e.ExerciseLevelId,
            Number = e.Number,
            Level = e.ExerciseLevel.Name,
            ShortName = e.ShortName,
            Url = e.Url,
            Template = e.Template,
            IsAccepted = e.IsAccepted,
            Examples = e.ExerciseExamples
                .Select(
                    x =>
                        new ExerciseExampleDto
                        {
                            Id = x.Id,
                            Input = x.Input,
                            Output = x.Output,
                            Explanation = x.Explanation,
                        }
                )
                .ToArray()
        };
    }

    public async Task<RunExerciseHistoryDto[]> GetRunExerciseHistory(
        Guid exerciseId,
        CancellationToken cancellationToken
    )
    {
        var userId = _authService.GetUserIdOrThrow();
        var runExercises = await _exerciseRepository.GetRunExerciseHistory(
            exerciseId,
            userId,
            cancellationToken
        );

        var result = new List<RunExerciseHistoryDto>();
        foreach (var runExercise in runExercises)
        {
            var testingResult = JsonSerializer.Deserialize<FunctionTestingResult>(
                runExercise.Result
            );
            var viewResult = new FunctionTestingResultView(testingResult!);
            var item = new RunExerciseHistoryDto
            {
                Id = runExercise.Id,
                Code = runExercise.Code,
                Date = runExercise.Date,
                Result = viewResult
            };
            result.Add(item);
        }

        return result.ToArray();
    }
}
