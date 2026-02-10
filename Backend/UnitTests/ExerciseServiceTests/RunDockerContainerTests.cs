using Common;
using Devpull.Course;
using Devpull.Exercises;
using FluentAssertions;

namespace UnitTests.ExerciseServiceTests;

[TestClass]
public class RunDockerContainerTests
{
    [TestMethod]
    public async Task compile_error()
    {
        var expected = new FunctionTestingResult
        {
            Status = TestStatus.CompileError,
            CompileError = "(1,17): error CS1002: ; expected"
        };
        var res = await ExerciseService.RunDockerContainer(
            ExerciseConst.AddExerciseId,
            "code",
            0.5,
            256,
            10
        );
        res.Status.Should().Be(TestStatus.CompileError);
        res.CompileError.Should().Be(expected.CompileError);
        res.Should().BeEquivalentTo(expected);
        // res.Should()
        //     .BeEquivalentTo(
        //         expected,
        //         options =>
        //             options
        //                 .Using<string>(ctx =>
        //                 {
        //                     ctx.Subject.Should().NotBeNull("because the property must not be null");
        //                     // no further check -> any non-null string passes
        //                 })
        //                 .When(info => info.Path == "CompileErrord")
        //     );
    }

    [TestMethod]
    public async Task timeout_error()
    {
        var timeout = 4;
        var expected = new FunctionTestingResult
        {
            Status = TestStatus.TimeoutError,
            Error = $"Выполнение программы было отменено по таймауту {timeout} сек."
        };
        var code =
            @"int Add(int a, int b)
{
    System.Threading.Thread.Sleep(5000);
    return 0;
}";

        var res = await ExerciseService.RunDockerContainer(
            ExerciseConst.AddExerciseId,
            code,
            0.5,
            256,
            timeout
        );
        res.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public async Task too_small_memory()
    {
        var ramLimit = 32;
        var expected = new FunctionTestingResult
        {
            Status = TestStatus.CommonError,
            Error = $"Превышен лимит памяти {ramLimit}Mb"
        };
        var code =
            @"int Add(int a, int b)
{
    System.Threading.Thread.Sleep(5000);
    return 0;
}";

        var res = await ExerciseService.RunDockerContainer(
            ExerciseConst.AddExerciseId,
            code,
            0.5,
            ramLimit,
            10
        );
        res.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public async Task memory_limit_exceed()
    {
        var ramLimit = 110;
        var arrSize = 200 * 1024 * 1024; // 200Mb
        var expectedRuntimeError =
            "Произошла ошибка при выполнении: Exception of type 'System.OutOfMemoryException' was thrown.";
        var code =
            @$"int Add(int a, int b)
{{
    var buffer = new byte[{arrSize}];
    return 0;
}}";

        var res = await ExerciseService.RunDockerContainer(
            ExerciseConst.AddExerciseId,
            code,
            0.5,
            ramLimit,
            10
        );
        res.Status.Should().Be(TestStatus.FailPublicTests);
        res.Results.Should().OnlyContain(r => r.Error == expectedRuntimeError);
    }

    [TestMethod]
    public async Task fail_public_tests()
    {
        var code =
            @"int Add(int a, int b)
{
    return 23213910;
}";

        var res = await ExerciseService.RunDockerContainer(
            ExerciseConst.AddExerciseId,
            code,
            0.5,
            256,
            10
        );
        res.Status.Should().Be(TestStatus.FailPublicTests);
        res.Results
            .Should()
            .AllSatisfy(r =>
            {
                r.IsSuccess.Should().BeFalse();
                r.FailMsg.Should().NotBeNull();
            });
    }

    [TestMethod]
    public async Task not_found_exercise()
    {
        var exerciseId = "not_exists_exercise_id";
        var expected = new FunctionTestingResult
        {
            Status = TestStatus.CommonError,
            Error = $"Не найдено упражнение с Ид {exerciseId}"
        };
        var res = await ExerciseService.RunDockerContainer(exerciseId, "code", 0.5, 256, 10);
        res.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public async Task success()
    {
        var code =
            @"int Add(int a, int b)
{
    return a + b;
}";
        var res = await ExerciseService.RunDockerContainer(
            ExerciseConst.AddExerciseId,
            code,
            0.5,
            256,
            10
        );
        res.Status.Should().Be(TestStatus.Success);
        res.Results.Should().OnlyContain(r => r.IsSuccess == true);
    }

    // FailPrivateTests - непонятно, как проверять, мы не можем контролировать тесты
}
