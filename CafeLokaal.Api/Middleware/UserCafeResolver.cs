namespace CafeLokaal.Api.Middleware;

using System;
using CafeLokaal.Api.Data;

public interface ICafeResolver
{
    string GetCafeNameForUser(); // e.g. "CafeWillem"
}

public class UserCafeResolver : ICafeResolver
{
    private readonly IRegisteredUserRepository _registeredUserRepository;

    public UserCafeResolver(IRegisteredUserRepository registeredUserRepository)
    {
        _registeredUserRepository = registeredUserRepository;
    }

    public string GetCafeNameForUser()
    {
        // var user = _httpContextAccessor.HttpContext?.User;

        // if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
        //     throw new UnauthorizedAccessException("User is not authenticated");

        // var userId = user.FindFirst("uid")?.Value;
        // if (string.IsNullOrWhiteSpace(userId))
        //     throw new Exception("User ID missing");

        return _registeredUserRepository.GetCafeNameByUserId("userId"); // Replace with actual user ID retrieval logic
    }
}
