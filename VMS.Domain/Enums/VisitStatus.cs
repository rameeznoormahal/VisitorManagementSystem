namespace VMS.Domain.Enums;

public enum VisitStatus
{
    Draft = 1,
    PendingApproval = 2,
    Approved = 3,
    Rejected = 4,
    ReadyForVisit = 5,
    CheckedIn = 6,
    CheckedOut = 7,
    Cancelled = 8,
    Expired = 9,
    NoShow = 10
}