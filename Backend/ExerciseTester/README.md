## Промпты

### Для генерации упражнений

Можно попросить сгенерировать упражнения на определенную тему, например
```cmd
сгенерируй упражнения по логическому типу данных на c#, в виде недописанной функции, которая может принимать параметры и всегда возвращает значения, например, int Add(int a, int b) { // Ваш код }
```

Потом для каждого упражнения можно попросить сгенерировать тест кейсы
```cmd
сгенерируй юнит тесты для 
public static int StandardFunction(int a)
    {
        return a * a;
    }
в правильном формате. Например для public static int Add(int a, int b) { return a + b; } правильный формат тестов будет 
public static IEnumerable<object[]> TestData()
{
	yield return [3, 2, 1, true];
	yield return [5, 2, 3, false];
	yield return [6, 2, 4, true];
	yield return [3, 3, 0, true];
	yield return [0, 0, 0, true];
	yield return [1, 1, 0, true];
	yield return [1, 0, 1, true];
	yield return [10, 7, 3, true];
	yield return [15, 8, 7, true];
	yield return [100, 50, 50, true];
	yield return [-5, -2, -3, true];
	yield return [-10, -7, -3, true];
	yield return [0, 5, -5, true];
	yield return [2, 5, -3, true];
	yield return [-2, -5, 3, true];
	yield return [1000, 600, 400, true];
	yield return [-100, -50, -50, true];
	yield return [int.MaxValue, int.MaxValue, 0, true];
	yield return [int.MinValue + 1, int.MinValue, 1, true];
	yield return [-1, int.MaxValue, int.MinValue, true];
	yield return [0, int.MaxValue, int.MinValue + 1, true];
}
```

Но можно пойти еще дальше, и попросить сгенерировать код сразу для нескольких функций, возьмя за основу существующий код
```cmd
@C:\Projects\Courses\Backend\Common\Const.cs
@C:\Projects\Courses\Backend\Devpull\Migrations\20251002105008_InsertIsPositiveExercise.cs
@C:\Projects\Courses\Backend\ExerciseTester\Testers\IsPositiveTester.cs
@C:\Projects\Courses\Backend\ExerciseTester\Program.cs
C:\Projects\Courses\Backend\UnitTests\IsPositiveTests.cs
есть функции
int MaxOfTwo(int a, int b)
{
    if (a > b)
        return a;
    else
        return b;
}
bool IsOdd(int number)
{
    if (number % 2 == 1)
        return true;
    else
        return false;
}
string Sign(int number)
{
    if (number > 0)
        return "положительное";
    else if (number < 0)
        return "отрицательное";
    else
        return "ноль";
}
string AgeCategory(int age)
{
    if (age < 12)
        return "ребенок";
    else if (age < 18)
        return "подросток";
    else if (age < 65)
        return "взрослый";
    else
        return "пенсионер";
}
сделай для них все, по аналогии с IsPositive, миграцию создай новую, используя команду dotnet ef migrations add IfElseExercises --project Devpull
```
