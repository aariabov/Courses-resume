using Common;
using Devpull.Controllers;
using Devpull.DbModels;
using Devpull.Exercises.Models;
using Microsoft.EntityFrameworkCore;

namespace Devpull.Exercises;

public class ExerciseRepository
{
    private readonly AppDbContext _db;

    public ExerciseRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ExerciseRegistryRecord[]> GetAll(
        string? userId,
        CancellationToken cancellationToken
    )
    {
        var query = _db.Exercises
            .OrderBy(e => e.Number)
            .Select(
                e =>
                    new ExerciseRegistryRecord
                    {
                        Id = e.Id,
                        LevelId = e.ExerciseLevelId,
                        Number = e.Number,
                        Level = e.ExerciseLevel.Name,
                        ShortName = e.ShortName,
                        Url = e.Url,
                        IsAccepted = e.RunExercises
                            .Where(r => r.UserId == userId)
                            .Any(
                                r =>
                                    r.Result.RootElement
                                        .GetProperty(nameof(FunctionTestingResult.Status))
                                        .GetInt32() == (int)TestStatus.Success
                            )
                    }
            );

        return await query.ToArrayAsync(cancellationToken);
    }

    public async Task<int> InsertRunExecute(
        RunExercise runExercise,
        CancellationToken cancellationToken
    )
    {
        _db.RunExercises.Add(runExercise);
        return await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<Exercise> GetByUrl(string url, CancellationToken cancellationToken)
    {
        return await _db.Exercises
                .Include(e => e.ExerciseExamples)
                .Include(e => e.ExerciseLevel)
                .FirstOrDefaultAsync(p => p.Url == url, cancellationToken)
            ?? throw new NotFoundException(url);
    }

    public Task<RunExercise[]> GetRunExerciseHistory(
        Guid exerciseId,
        string userId,
        CancellationToken cancellationToken
    )
    {
        return _db.RunExercises
            .Where(r => r.ExerciseId == exerciseId && r.UserId == userId)
            .OrderByDescending(r => r.Date)
            .Take(10)
            .ToArrayAsync(cancellationToken);
    }
}
