namespace GymManagement.BLL.ViewModels.BookingViewModels
{
    public class SessionMembersViewModel
    {
        public int MemberId { get; set; }
        public int SessionId { get; set; }
        public string MemberName { get; set; } = default!;
        public bool IsAttended { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
