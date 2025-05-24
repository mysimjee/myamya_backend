using myamyafansite_back_end.Database;
using myamyafansite_back_end.Database.Model;

namespace myamyafansite_back_end.Services;


public interface IAccountService
{
    Task<Account?> RegisterAccountAsync(Account newAccount);
    Task<Account?> LoginAsync(Account account);
    Task<bool> LogoutAsync();
    Task<bool> VerifyEmailAsync(Guid userId, string verificationCode); 
    Task<bool> ForgotPasswordAsync(string identifier);
    Task<Account?> UpdateAccountAsync(Guid userId, Account updatedAccount);
    Task<List<Account>?> ViewAccountListAsync(int skip, int limit);
    Task<int> GetTotalAccountCountBySearchTermAsync(string searchTerm); 
    Task<List<Account>?> FindAccountsAsync(string searchTerm, int skip, int limit);
    Task<Account?> FindAccountByIdAsync(Guid userId);
    Task<List<LoginHistory>?> ViewLoginHistoryAsync(Guid userId, int skip, int limit); // Pagination added
    Task<int> GetTotalAccountCountAsync();

}

public class AccountService
{
    public MyamyaFanSiteDbContext DbContext;
    
    
    
    
}