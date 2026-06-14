using FAQApp_test.Models;
using FAQApp_test.Repositories;
using Markdig;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace FAQApp_test.Pages
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
            .DisableHtml()
            .UseAdvancedExtensions()
            .Build();

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

        public async Task<PartialViewResult> OnGetDetailModalAsync(int id)
        {
            var faq = await _faqRepository.GetByIdAsync(id);
            if (faq is null)
            {
                Response.StatusCode = 404;
                return Partial("_DetailModal", new Faq
                {
                    Title = "Not Found",
                    Content = string.Empty,
                    HtmlContent = "<p>FAQが見つかりませんでした。</p>"
                });
            }

            faq.HtmlContent = Markdown.ToHtml(faq.Content, MarkdownPipeline);
            return Partial("_DetailModal", faq);
        }

        public async Task<PartialViewResult> OnDeleteDeleteFaqAsync(int id, string? searchKeyword)
        {
            IReadOnlyList<Faq> faqs;

            try
            {
                await _faqRepository.DeleteAsync(id);
                faqs = await _faqRepository.GetListAsync(searchKeyword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete FAQ by delete handler.");
                faqs = new List<Faq>();
            }

            ViewData["SearchKeyword"] = searchKeyword;
            return Partial("_FaqList", faqs);
        }
    }
}
