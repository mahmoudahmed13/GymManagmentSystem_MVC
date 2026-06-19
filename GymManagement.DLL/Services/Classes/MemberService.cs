using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberService : IMemberServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public MemberService(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            //checkEmail
            var emailExist = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email, ct);
            //checkPhone
            var phoneExist = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone, ct);
            
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

            //var result = await _memberRepository.AddAsync(member);
            _unitOfWork.GetRepository<Member>().Add(member); //Add Local
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMemberAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
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

        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId , ct);

            if (member == null) return null;

            var model = new MemberViewModel()
            {
                Name = member.Name,
                Phone = member.Phone,
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Gender = member.Gender.ToString(),
                Address = $"{member.Address.BuildingNumber} - {member.Address.Street} - {member.Address.City}",
                Email = member.Email
            };

            var activeMembership = await _unitOfWork.GetRepository<MemberShip>().FirstOrDefaultAsync(x => x.MemberId == memberId && x.EndData > DateTime.Now);
            if (activeMembership is not null)
            {
                var activePlan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMembership.PlanId, ct);
                model.PlanName = activePlan?.Name;
                model.MembershipStartDate = activeMembership.CreatedAt.ToString();
                model.MembershipEndDate = activeMembership.EndData.ToString();
            }

            return model;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecoedAsync(int memberId, CancellationToken ct = default)
        {
            var record = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(x =>x.MemberId == memberId , ct:ct);
            if (record == null) return null;

            else
                return new HealthRecordViewModel()
                {
                    Weight = record.Weight,
                    BloodType = record.BloodType,
                    Height = record.Height,
                    Note = record.Note,
                };
        }

        public async Task<MemberToUpdateViewModel> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if (member == null) return null;

            else
                return new MemberToUpdateViewModel()
                {
                    Name = member.Name,
                    Phone = member.Phone,
                    Email = member.Email,
                    BuildingNumber = member.Address.BuildingNumber,
                    City = member.Address.City,
                    Street = member.Address.Street,
                    Photo = member.Photo
                };
        }

        public async Task<bool> RemoveMemberAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if (member == null) return false;
            var hasFutureBookings = await _unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId == memberId && b.Session.StartDate > DateTime.Now, ct);
            if(hasFutureBookings) return false;

             _unitOfWork.GetRepository<Member>().Delete(member); //delete local
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }

        public async Task<bool> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return false;

            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == member.Email && m.Id != id);
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == member.Phone && m.Id != id);

            if (emailExists || phoneExists) return false;

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.City = model.City;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.Street = model.Street;
            member.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Member>().Update(member);//update local
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }
    }
}
