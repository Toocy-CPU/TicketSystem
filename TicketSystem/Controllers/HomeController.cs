using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Database;
using TicketSystem.Models;
using TicketSystem.ViewModels;

namespace TicketSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly TicketSystemDbContext _ctx;
    private UserManager<IdentityUser> _userManager;

    public HomeController(ILogger<HomeController> logger, TicketSystemDbContext ctx, UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _ctx = ctx;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        Statistic s1 = new Statistic();
        _ctx.Tickets.Where(t => t.TicketClosed == false).ToList().ForEach(t => s1.OpenTickets++);
        _ctx.Tickets.Where(t => t.TicketClosed == true).ToList().ForEach(t => s1.ClosedTickets++);
        _ctx.Projects.Where(p => p.ProjectClosed == false).ToList().ForEach(p => s1.OpenProjects++);
        _ctx.Projects.Where(p => p.ProjectClosed == true).ToList().ForEach(p => s1.ClosedProjects++);


        return View(s1);
    }
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Admin()
    {
        Statistic s1 = new Statistic();
        var users = _userManager.Users.ToList();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                s1.Admins++;
            }
            if (roles.Contains("Developer"))
            {
                s1.Devs++;
            }
            if (roles.Contains("Tester"))
            {
                s1.Tester++;
            }
        }
        return View(s1);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
