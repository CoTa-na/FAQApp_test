using System.ComponentModel.DataAnnotations;

namespace FAQApp_test.Models;

public class Faq
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? PrevId { get; set; }
    public int? NextId { get; set; }
    public string? HtmlContent { get; set; }
}
