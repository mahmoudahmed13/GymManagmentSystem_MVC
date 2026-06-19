namespace GymManagement.BLL.ViewModels.TrainerViewModels
{
    public class TrainerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string specialization { get; set; }

        //Trainer Details
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }

    }
}
