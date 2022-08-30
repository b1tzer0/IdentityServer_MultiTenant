namespace idpMultiTenant1.Services
{
    public class AuthMessageSenderOptions
    {
        public string? SendGridKey { get; set; }

        public string? DisplayName { get; set; }

        public string? EmailFrom { get; set; }

        public string EmailConfirmationTemplateId { get; set; }
    }
}
