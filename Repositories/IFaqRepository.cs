using FAQApp_test.Models;

namespace FAQApp_test.Repositories;

public interface IFaqRepository
{
    Task<IReadOnlyList<Faq>> GetListAsync(string? searchKeyword);
    Task<Faq?> GetByIdAsync(int id);
    Task<int> CreateAsync(Faq faq);
    Task UpdateAsync(Faq faq);
    Task DeleteAsync(int id);
}
