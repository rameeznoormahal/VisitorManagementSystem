namespace VMS.Web.ViewModels.Visits
{
    public class VisitVisitorEntryViewModel
    {
        public string IdType { get; set; } = string.Empty;
        public string IdNumber { get; set; } = string.Empty;
        public DateOnly IdExpiryDate { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Email { get; set; }
        public string? CompanyName { get; set; }
        public string? Designation { get; set; }
        public string? Nationality { get; set; }
    }
}
