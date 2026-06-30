using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Attachment;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberService : IMemberServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork unitOfWork, IMapper mapper, IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        public async Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            //checkEmail
            var emailExist = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email, ct);
            //checkPhone
            var phoneExist = await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone, ct);

            //Email or Phone Exist Return False
            if (emailExist || phoneExist) return Result.Fail("Email or Phone Already Exist");

            //upload Photo
            var storedPhotoName = await _attachmentService.UploadAsync(model.PhotoFile.OpenReadStream(), model.PhotoFile.FileName, "MembersPhoto");
            if (string.IsNullOrWhiteSpace(storedPhotoName)) return Result.Fail("Failed To Upload Photo");

            //Else return True Add Member
            var member = _mapper.Map<Member>(model);
            member.Photo = storedPhotoName; 
            //var result = await _memberRepository.AddAsync(member);
            _unitOfWork.GetRepository<Member>().Add(member); //Add Local
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
                return Result.Ok();
            else
            {
                //Delete Photo If Failed To Add Member
                _attachmentService.Delete(storedPhotoName, "MembersPhoto");
                return Result.Fail("Failed To Created");
            }
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMemberAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            if (!members.Any()) return [];

            var membersViewModel = _mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>>(members);
            return membersViewModel;
        }

        public async Task<Result<MemberViewModel>> GetMemberDetailsByIdAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);

            if (member == null) return Result<MemberViewModel>.NotFound("Member Not Found");

            var model = _mapper.Map<Member, MemberViewModel>(member);

            var activeMembership = await _unitOfWork.GetRepository<MemberShip>().FirstOrDefaultAsync(x => x.MemberId == memberId && x.EndData > DateTime.Now);
            if (activeMembership is not null)
            {
                var activePlan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMembership.PlanId, ct);
                model.PlanName = activePlan?.Name;
                model.MembershipStartDate = activeMembership.CreatedAt.ToString();
                model.MembershipEndDate = activeMembership.EndData.ToString();
            }

            return Result<MemberViewModel>.Ok(model);
        }

        public async Task<Result<HealthRecordViewModel>> GetMemberHealthRecoedAsync(int memberId, CancellationToken ct = default)
        {
            var record = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(x => x.MemberId == memberId, ct: ct);
            if (record == null) return Result<HealthRecordViewModel>.NotFound("Health Record Not Found");

            else
            {
                var mapperMember = _mapper.Map<HealthRecord, HealthRecordViewModel>(record);
                return Result<HealthRecordViewModel>.Ok(mapperMember);
            }
        }

        public async Task<Result<MemberToUpdateViewModel>> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if (member == null) return Result<MemberToUpdateViewModel>.NotFound("Member Not Found");

            else
            {
                var mapperMember = _mapper.Map<MemberToUpdateViewModel>(member);
                return Result<MemberToUpdateViewModel>.Ok(mapperMember);
            }
        }

        public async Task<Result> RemoveMemberAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);
            if (member == null) return Result.NotFound("Member Not Found");
            var hasFutureBookings = await _unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId == memberId && b.Session.StartDate > DateTime.Now, ct);
            if (hasFutureBookings) return Result.Fail("Can Not Remove Member Has Bookins");

            _attachmentService.Delete(member.Photo ?? " ", "MembersPhoto"); //delete photo

            _unitOfWork.GetRepository<Member>().Delete(member); //delete local
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Delete This Member");
        }

        public async Task<Result> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return Result.NotFound("Member Not Found");

            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == member.Email && m.Id != id);
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == member.Phone && m.Id != id);

            if (emailExists || phoneExists) return Result.Fail("Email or Phone Already Exist");
            _mapper.Map(model, member);
            member.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Member>().Update(member);//update local
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Edit This Member");
        }
    }
}
