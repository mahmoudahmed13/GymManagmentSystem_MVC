using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
   
    public class MemberController : Controller
    {
        //Get BaseURL/Members/Index
        //Index() - Displays all member listing page
        private readonly IMemberServices _memberService;
        public MemberController(IMemberServices memberService)
        {
            _memberService = memberService;
        }

        //Get BaseURL/Members/MemberDetails{id}
        //MemberDetails(int id) - Displays member profile page
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var members = await _memberService.GetAllMemberAsync(ct);
            return View(members);
        }

        //Get BaseURL//Member/HealthRecordDetails/{id}
        //HealthRecordDetails(int id) - Shows health record page

        #region Create Member

        //Get BaseURL//Member/Create
        //Create() - Shows member registration form
        [HttpGet]
        public IActionResult Create() => View();

        //Post BaseURL//Member/CreateMember
        //CreateMember() - Save submitted form
        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);

            var result = await _memberService.CreateMemberAsync(model, ct);
            if (result)
                TempData["SuccessMessage"] = "Member Created Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Create Member";

            return RedirectToAction(nameof(Index));
            
        }
        #endregion

        #region Edit Member

        //Get BaseURL//Member/Edit/{id}
        //Edit(int id) - Displays edit form

        //Post BaseURL//Member/Edit {Member}
        //Edit() - Save update
        #endregion

        #region Delete Member
        //Get BaseUrl/Members/Delete/{id}
        //Delete(int id) - Shows deletion confirmation page

        //Post BaseURL//Member/DeleteConfirm/{Id}
        //DeleteConfirm() - Processes deletion(submit From)

        #endregion
    }
}
