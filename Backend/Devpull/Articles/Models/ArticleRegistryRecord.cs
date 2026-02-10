namespace Devpull.Articles.Models;

public class ArticleRegistryRecord
{
    public required Guid Id { get; set; }
    public required string Url { get; set; }
    public required string Title { get; set; }
    public required string ShortText { get; set; }
    public required DateTime CreateDate { get; set; }
    public required DateTime UpdateDate { get; set; }
}
