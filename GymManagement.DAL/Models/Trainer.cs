using GymManagement.DAL.Models.Enums;

namespace GymManagement.DAL.Models
{
    public class Trainer : GymUser
    {
        //HireDate => CreatedAt
        public Specialty Specialty { get; set; }
    }
}
