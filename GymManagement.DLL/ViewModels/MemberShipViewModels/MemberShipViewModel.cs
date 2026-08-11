namespace GymManagement.BLL.ViewModels.MemberShipViewModels
{
    public class MemberShipViewModel
    {
        public int Id { get; set; }
        public string MemberName { get; set; } = default!;
        public string PlanName { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
