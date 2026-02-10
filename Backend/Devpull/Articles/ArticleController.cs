using Devpull.Articles.Models;
using Devpull.Controllers;
using Devpull.DbModels;
using Microsoft.AspNetCore.Mvc;

namespace Devpull.Articles;

[ApiController]
[Route("api/article")]
public class ArticleController : ControllerBase
{
    private readonly ArticleService _articleService;
    private readonly ExecutionService _executionService;

    public ArticleController(ArticleService articleService, ExecutionService executionService)
    {
        _articleService = articleService;
        _executionService = executionService;
    }

    [HttpPost("get-article-by-url")]
    public Task<OperationResult<Article>> GetArticleByUrl([FromBody] string articleUrl)
    {
        return _executionService.TryExecute(
            () => _articleService.GetByUrl(articleUrl, HttpContext.RequestAborted)
        );
    }

    [HttpPost("get-articles")]
    public Task<OperationResult<ArticleRegistryRecord[]>> GetArticles()
    {
        return _executionService.TryExecute(
            () => _articleService.GetAll(HttpContext.RequestAborted)
        );
    }
}
