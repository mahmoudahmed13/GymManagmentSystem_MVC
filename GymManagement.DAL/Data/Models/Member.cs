namespace GymManagement.DAL.Data.Models
{
    public class Member : GymUser
    {
        public string? Photo { get; set; }
        //JoinDate => CreatedAt

        public HealthRecord HealthRecord { get; set; } = default!;

        public ICollection<MemberShip> MemberShips { get; set; }
        public ICollection<Booking> Bookings { get; set; }

    }
}
