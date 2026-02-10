using Devpull.Articles.Models;
using Devpull.DbModels;

namespace Devpull.Articles;

public class ArticleService
{
    private readonly ArticleRepository _articleRepository;

    public ArticleService(ArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public Task<Article> GetByUrl(string articleUrl, CancellationToken cancellationToken)
    {
        return _articleRepository.GetByUrl(articleUrl, cancellationToken);
    }

    public Task<ArticleRegistryRecord[]> GetAll(CancellationToken cancellationToken)
    {
        return _articleRepository.GetAll(cancellationToken);
    }
}
