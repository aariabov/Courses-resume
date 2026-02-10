using Microsoft.CodeAnalysis.Scripting;
using System.Text;
using System.Text.Json;
using Common;
using ExerciseTester.Testers;

var scriptPath = args[0];
var exerciseId = args[1];

FunctionTestingResult result;
try
{
    var code = await File.ReadAllTextAsync(scriptPath, Encoding.UTF8);
    result = await (
        exerciseId.ToUpper() switch
        {
            ExerciseConst.AddExerciseId => new AddTester().TestClientCode(code),
            ExerciseConst.JoinExerciseId => new JoinTester().TestClientCode(code),
            ExerciseConst.SquareExerciseId => new SquareTester().TestClientCode(code),
            ExerciseConst.MinExerciseId => new MinTester().TestClientCode(code),
            ExerciseConst.IsPositiveExerciseId => new IsPositiveTester().TestClientCode(code),
            ExerciseConst.IsEvenExerciseId => new IsEvenTester().TestClientCode(code),
            ExerciseConst.IsInRangeExerciseId => new IsInRangeTester().TestClientCode(code),
            ExerciseConst.SumArrayExerciseId => new SumArrayTester().TestClientCode(code),
            ExerciseConst.MaxArrayElementExerciseId
                => new MaxArrayElementTester().TestClientCode(code),
            ExerciseConst.PowerExerciseId => new PowerTester().TestClientCode(code),
            ExerciseConst.CountEvenExerciseId => new CountEvenTester().TestClientCode(code),
            ExerciseConst.IndexOfExerciseId => new IndexOfTester().TestClientCode(code),
            ExerciseConst.ReverseArrayExerciseId => new ReverseArrayTester().TestClientCode(code),
            ExerciseConst.RotateExerciseId => new RotateTester().TestClientCode(code),
            ExerciseConst.MaxOfTwoExerciseId => new MaxOfTwoTester().TestClientCode(code),
            ExerciseConst.IsOddExerciseId => new IsOddTester().TestClientCode(code),
            ExerciseConst.SignExerciseId => new SignTester().TestClientCode(code),
            ExerciseConst.AgeCategoryExerciseId => new AgeCategoryTester().TestClientCode(code),
            ExerciseConst.FindMaxConsecutiveOnesExerciseId => new FindMaxConsecutiveOnesTester().TestClientCode(code),
            ExerciseConst.FindOnesCountExerciseId => new FindOnesCountTester().TestClientCode(code),
            ExerciseConst.SortedSquaresExerciseId => new SortedSquaresTester().TestClientCode(code),
            ExerciseConst.DuplicateZerosExerciseId => new DuplicateZerosTester().TestClientCode(code),
            ExerciseConst.MergeExerciseId => new MergeTester().TestClientCode(code),
            ExerciseConst.TwoSumExerciseId => new TwoSumTester().TestClientCode(code),
            ExerciseConst.RemoveElementExerciseId => new RemoveElementTester().TestClientCode(code),
            ExerciseConst.RemoveDuplicatesExerciseId => new RemoveDuplicatesTester().TestClientCode(code),
            ExerciseConst.CheckIfExistDoubleExerciseId => new CheckIfExistDoubleTester().TestClientCode(code),
            ExerciseConst.ValidMountainArrayExerciseId => new ValidMountainArrayTester().TestClientCode(code),
            ExerciseConst.ReplaceElementsExerciseId => new ReplaceElementsTester().TestClientCode(code),
            ExerciseConst.MoveZeroesExerciseId => new MoveZeroesTester().TestClientCode(code),
            ExerciseConst.HeightCheckerExerciseId => new HeightCheckerTester().TestClientCode(code),
            ExerciseConst.ThirdMaxExerciseId => new ThirdMaxTester().TestClientCode(code),
            ExerciseConst.PivotIndexExerciseId => new PivotIndexTester().TestClientCode(code),
            ExerciseConst.PlusOneExerciseId => new PlusOneTester().TestClientCode(code),
            ExerciseConst.MinSubArrayLenExerciseId => new MinSubArrayLenTester().TestClientCode(code),
            _ => throw new Exception($"Не найдено упражнение с Ид {exerciseId}"),
        }
    );
}
catch (CompilationErrorException e)
{
    var error = string.Join("\n", e.Diagnostics.Select(d => d.ToString()));
    result = FunctionTestingResult.CreateCompileError(error);
}
catch (Exception ex)
{
    result = FunctionTestingResult.CreateError(ex.Message);
}

var json = JsonSerializer.Serialize(result);
Console.WriteLine(json);
