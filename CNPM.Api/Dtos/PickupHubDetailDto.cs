using Microsoft.Identity.Client;

namespace CNPM.Api.Dtos
{
    public class PickupHubDetailDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? OpeningHours { get; set; }
        public string? ClosingHours { get; set; }
        public string? Status { get; set; }
    }

    public class HubOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
