namespace CafeLokaal.Api.Data;

public interface IRegisteredUserRepository
{
    string GetCafeNameByUserId(string userId);
}

public class RegisteredUserRepository : IRegisteredUserRepository
{
    private readonly CafeLokaalContext _context;

    public RegisteredUserRepository(CafeLokaalContext context)
    {
        _context = context;
    }

    public string GetCafeNameByUserId(string userId)
    {
        // Implement your logic to retrieve the cafe name by user ID
        return "CafeWillem"; // Placeholder
    }
}
