using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Devpull.DbModels;

public class Article
{
    public required Guid Id { get; set; }
    public required string Url { get; set; }
    public required string Title { get; set; }
    public required string Text { get; set; }
    public required string ShortText { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
}
