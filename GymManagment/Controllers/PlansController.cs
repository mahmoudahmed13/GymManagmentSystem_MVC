using GymManagment.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagment.Controllers
{
    public class PlansController : Controller
    {
        //Index Action => GET BaseURL/Plans/Index(Listing all Plans)
        //Details Action => GET BaseURL/Plans/Details/Id
        private readonly GymDbContext dbContext;
        public PlansController()
        {
            dbContext = new GymDbContext();
        }
        public async Task<IActionResult> Index()
        {
            var plans = await dbContext.Plans.ToListAsync();
            return View(plans);
        }
        public async Task<IActionResult> Details(int id)
        {
            var plan = await dbContext.Plans.FindAsync(id);
            if (plan is null)
                return RedirectToAction(nameof(Index));
            else
                return View(plan);
        }
    }
}
  