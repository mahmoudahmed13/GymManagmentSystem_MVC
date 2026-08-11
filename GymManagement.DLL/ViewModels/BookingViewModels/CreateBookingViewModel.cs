using GymManagement.DAL.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace GymManagement.BLL.ViewModels.BookingViewModels
{
    public class CreateBookingViewModel
    {
        [Required]
        public int MemberId { get; set; }
        [Required]
        public int SessionId { get; set; }
        [Required]
        public bool IsAttended { get; set; }
        public string AttendanceStatus => IsAttended ? "Attended" : "Mark As Attended";

    }
}
