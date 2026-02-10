using Devpull.Articles.Models;
using Devpull.Controllers;
using Devpull.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Devpull.Articles;

public class ArticleRepository
{
    private readonly AppDbContext _db;

    public ArticleRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Article> GetByUrl(string articleUrl, CancellationToken cancellationToken)
    {
        return await _db.Articles.FirstOrDefaultAsync(p => p.Url == articleUrl, cancellationToken)
            ?? throw new NotFoundException(articleUrl);
    }

    public async Task<ArticleRegistryRecord[]> GetAll(CancellationToken cancellationToken)
    {
        return await _db.Articles
            .OrderByDescending(a => a.CreateDate)
            .Select(
                a =>
                    new ArticleRegistryRecord
                    {
                        Id = a.Id,
                        Url = a.Url,
                        Title = a.Title,
                        ShortText = a.ShortText,
                        CreateDate = a.CreateDate,
                        UpdateDate = a.UpdateDate
                    }
            )
            .ToArrayAsync(cancellationToken);
    }
}
