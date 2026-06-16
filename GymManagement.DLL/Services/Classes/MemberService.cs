using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberService : IMemberServices
    {
        private readonly IGenericRepository<Member> _memberRepository;
        public MemberService(IGenericRepository<Member> memberRepository) 
        {
            _memberRepository = memberRepository;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            //checkEmail
            var emailExist = await _memberRepository.AnyAsync(x => x.Email == model.Email, ct);
            //checkPhone
            var phoneExist = await _memberRepository.AnyAsync(x => x.Phone == model.Phone, ct);
            
            //Email or Phone Exist Return False
            if (emailExist || phoneExist) return false;

            //Else return True Add Member
            var member = new Member()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    City = model.City,
                    Street = model.Street,
                },
                HealthRecord = new HealthRecord()
                {
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Weight = model.HealthRecordViewModel.Weight,
                    Height = model.HealthRecordViewModel.Height,
                    Note = model.HealthRecordViewModel.Note,
                }
                
            };

            var result = await _memberRepository.AddAsync(member);
            return result > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMemberAsync(CancellationToken ct = default)
        {
            var members = await _memberRepository.GetAllAsync(ct: ct);
            if (!members.Any()) return [];

            var membersViewModel = members.Select(m => new MemberViewModel()
            {
                Email = m.Email,
                Gender = m.Gender.ToString(),
                Id = m.Id,
                Name = m.Name,
                Phone = m.Phone,
                Photo = m.Photo,
            });
            return membersViewModel;
        }
    }
}
