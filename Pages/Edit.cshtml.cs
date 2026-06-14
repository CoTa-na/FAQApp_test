using FAQApp_test.Models;
using FAQApp_test.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FAQApp_test.Pages;

public class EditModel : PageModel
{
    private readonly IFaqRepository _faqRepository;
    private readonly ILogger<EditModel> _logger;

    public EditModel(IFaqRepository faqRepository, ILogger<EditModel> logger)
    {
        _faqRepository = faqRepository;
        _logger = logger;
    }

    [BindProperty]
    public Faq Faq { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var faq = await _faqRepository.GetByIdAsync(id);
            if (faq is null)
            {
                return NotFound();
            }

            Faq = faq;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load FAQ for edit.");
            return RedirectToPage("/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var current = await _faqRepository.GetByIdAsync(Faq.Id);
            if (current is null)
            {
                return NotFound();
            }

            current.Title = Faq.Title;
            current.Content = Faq.Content;

            await _faqRepository.UpdateAsync(current);
            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update FAQ.");
            ModelState.AddModelError(string.Empty, "FAQ更新に失敗しました。DB接続設定を確認してください。");
            return Page();
        }
    }
}
