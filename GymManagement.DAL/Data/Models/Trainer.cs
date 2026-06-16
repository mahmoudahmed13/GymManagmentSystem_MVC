using GymManagement.DAL.Data.Models.Enums;

namespace GymManagement.DAL.Data.Models
{
    public class Trainer : GymUser
    {
        //HireDate => CreatedAt
        public Specialty Specialty { get; set; }

        public ICollection<Session> Sessions { get; set; } = default!;
    }
}
