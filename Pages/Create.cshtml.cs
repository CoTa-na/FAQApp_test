using FAQApp_test.Models;
using FAQApp_test.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FAQApp_test.Pages;

public class CreateModel : PageModel
{
    private readonly IFaqRepository _faqRepository;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(IFaqRepository faqRepository, ILogger<CreateModel> logger)
    {
        _faqRepository = faqRepository;
        _logger = logger;
    }

    [BindProperty]
    public Faq Faq { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            await _faqRepository.CreateAsync(Faq);
            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create FAQ.");
            ModelState.AddModelError(string.Empty, "FAQ登録に失敗しました。DB接続設定を確認してください。");
            return Page();
        }
    }
}
