using FAQApp_test.Models;
using FAQApp_test.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace FAQApp_test.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IFaqRepository _faqRepository;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IFaqRepository faqRepository, ILogger<IndexModel> logger)
        {
            _faqRepository = faqRepository;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchKeyword { get; set; }
        public string? ErrorMessage { get; private set; }

        public IReadOnlyList<Faq> Faqs { get; private set; } = new List<Faq>();

        public async Task OnGetAsync()
        {
            try
            {
                Faqs = await _faqRepository.GetListAsync(SearchKeyword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load FAQ list.");
                ErrorMessage = "FAQ一覧の取得に失敗しました。DB接続設定を確認してください。";
                Faqs = new List<Faq>();
            }
        }

        public async Task<PartialViewResult> OnGetFaqListAsync(string? searchKeyword)
        {
            IReadOnlyList<Faq> faqs;

            try
            {
                faqs = await _faqRepository.GetListAsync(searchKeyword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search FAQ list.");
                faqs = new List<Faq>();
            }

            ViewData["SearchKeyword"] = searchKeyword;
            return Partial("_FaqList", faqs);
        }

        public async Task<PartialViewResult> OnPostDeleteAsync(int id, string? searchKeyword)
        {
            IReadOnlyList<Faq> faqs;

            try
            {
                await _faqRepository.DeleteAsync(id);
                faqs = await _faqRepository.GetListAsync(searchKeyword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete FAQ.");
                faqs = new List<Faq>();
            }

            ViewData["SearchKeyword"] = searchKeyword;
            return Partial("_FaqList", faqs);

        }
    }
}
