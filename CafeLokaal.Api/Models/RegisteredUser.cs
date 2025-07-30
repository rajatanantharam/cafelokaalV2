namespace CafeLokaal.Api.Models
{
    public class RegisteredUser
    {
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; } = Guid.Empty;
        public Guid SubscriptionId { get; set; } = Guid.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}