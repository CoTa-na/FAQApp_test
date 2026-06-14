using Dapper;
using FAQApp_test.Models;
using Microsoft.Data.SqlClient;

namespace FAQApp_test.Repositories;

public class FaqRepository : IFaqRepository
{
    private readonly string _connectionString;

    public FaqRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
    }

    public async Task<IReadOnlyList<Faq>> GetListAsync(string? searchKeyword)
    {
        using var connection = new SqlConnection(_connectionString);

        var faqs = await connection.QueryAsync<Faq>(
            "usp_GetFaqList",
            new { SearchKeyword = searchKeyword },
            commandType: System.Data.CommandType.StoredProcedure);

        return faqs.ToList();
    }

    public async Task<Faq?> GetByIdAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);

        return await connection.QueryFirstOrDefaultAsync<Faq>(
            "usp_GetFaqDetail",
            new { Id = id },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<int> CreateAsync(Faq faq)
    {
        using var connection = new SqlConnection(_connectionString);

        var parameters = new DynamicParameters();
        parameters.Add("@Title", faq.Title);
        parameters.Add("@Content", faq.Content);
        parameters.Add("@CreatedBy", faq.CreatedBy);
        parameters.Add("@NewId", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

        await connection.ExecuteAsync(
            "usp_CreateFaq",
            parameters,
            commandType: System.Data.CommandType.StoredProcedure);

        return parameters.Get<int>("@NewId");
    }

    public async Task UpdateAsync(Faq faq)
    {
        using var connection = new SqlConnection(_connectionString);

        await connection.ExecuteAsync(
            "usp_UpdateFaq",
            new
            {
                faq.Id,
                faq.Title,
                faq.Content
            },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);

        await connection.ExecuteAsync(
            "usp_DeleteFaq",
            new { Id = id },
            commandType: System.Data.CommandType.StoredProcedure);
    }
}
