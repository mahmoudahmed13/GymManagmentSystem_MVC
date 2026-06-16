using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Classes;
using GymManagement.DAL.Repositorities.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagment.Controllers
{
    public class PlansController : Controller
    {
        //Index Action => GET BaseURL/Plans/Index(Listing all Plans)
        //Details Action => GET BaseURL/Plans/Details/Id

        //private readonly GymDbContext dbContext = new GymDbContext();
        private readonly IGenericRepository<Plan> planRepository;
        public PlansController(IGenericRepository<Plan> repository)
        {
            planRepository = repository;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await planRepository.GetAllAsync(ct: ct);
            return View(plans);
        }
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await planRepository.GetByIdAsync(id, ct);
            if (plan is null)
                return RedirectToAction(nameof(Index));
            else
                return View(plan);
        }
    }
}
