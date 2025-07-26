namespace CafeLokaal.Api.Models;

public class CafeModel
{
    public Guid CafeId { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public ContactInfo PrimaryContact { get; set; } = new();
}

public class ContactInfo
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string TelephoneNumber { get; set; } = string.Empty;
}
