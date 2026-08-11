using System.ComponentModel.DataAnnotations;

namespace GymManagement.BLL.ViewModels.MemberShipViewModels
{
    public class CreateMemberShipViewModel
    {
        [Required(ErrorMessage = "Member Is Required")]
        public int MemberId { get; set; }
        [Required(ErrorMessage ="Plan Is Required")]
        public int PlanId { get; set; }
    }
}
