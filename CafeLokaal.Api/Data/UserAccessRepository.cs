using CafeLokaal.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeLokaal.Api.Data;

public interface IUserAccessRepository
{
    Task<UserAccess> GetUserAccess(string email, string organizationName);
    
    Task<string> GetUserAccessOrganization(string email);

    Task CreateUserAccess(UserAccess userAccess);

}

public class UserAccessRepository : IUserAccessRepository
{
    private readonly IDBContextResolver _dbContextResolver;
    private readonly ILogger<UserAccessRepository> _logger;

    public UserAccessRepository(IDBContextResolver dbContextResolver, ILogger<UserAccessRepository> logger)
    {
        _logger = logger;
        _dbContextResolver = dbContextResolver;
    }
    
    public async Task<UserAccess> GetUserAccess(string email, string organizationName)
    {
        try
        {
            using var context = _dbContextResolver.GetCafeLokaalDB();
            // Query to find the user access by email and organization name
            var userAccess = await context.UserAccess
                .FirstOrDefaultAsync(ua => ua.Email == email && ua.OrganizationName == organizationName) 
                ?? throw new KeyNotFoundException($"User access not found for email: {email} and organization: {organizationName}");
            return userAccess;
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            throw new Exception("An error occurred while retrieving user access", ex);
        }
    }

    public async Task<string> GetUserAccessOrganization(string email)
    {
        try
        {
            _logger.LogInformation("Retrieving user access for email: {Email}", email);
            using var context = _dbContextResolver.GetCafeLokaalDB();
            // Query to find the user access by email
            var user = await context.UserAccess
                .FirstOrDefaultAsync(ua => ua.Email == email) ?? throw new KeyNotFoundException($"User access not found for email: {email}");
            return user.OrganizationName;
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            _logger.LogError(ex, "Error retrieving user access for email: {Email}", email);
            throw new Exception("An error occurred while retrieving user access", ex);
        }
    }

    
    public async Task CreateUserAccess(UserAccess userAccess)
    {
        using var context = _dbContextResolver.GetCafeLokaalDB();

        // Check if the user access already exists
        var existingUserAccess = await context.UserAccess
            .FirstOrDefaultAsync(ua => ua.Email == userAccess.Email && 
                                        ua.OrganizationName == userAccess.OrganizationName);

        if (existingUserAccess != null)
        {   
            throw new InvalidOperationException("User access already exists for this email and organization");
        }

        context.UserAccess.Add(userAccess);
        await context.SaveChangesAsync();

        _logger.LogInformation("UserAccess created successfully for email: {Email}, organization: {OrganizationName}", 
            userAccess.Email, userAccess.OrganizationName);

    }
}