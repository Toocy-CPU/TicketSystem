using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketSystem.Database;
using TicketSystem.Models;

namespace TicketSystem.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly TicketSystemDbContext _context;

        public ProjectsController(TicketSystemDbContext context)
        {
            _context = context;
        }


        // GET: Projects
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Projects.ToListAsync());
        }

        // GET: Projects/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,DeadLine")] Project project)
        {
            if (project.DeadLine < DateTime.Now)
            {
                ModelState.AddModelError("DeadLine", "Die Deadline muss nach dem aktuellen Datum liegen.");
            }
            if (ModelState.IsValid)
            {
                project.CreatedAt = DateTime.Now;
                _context.Add(project);
                var projectFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files",project.Title);
                Directory.CreateDirectory(projectFolder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,CreatedAt,DeadLine")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }
            if (project.DeadLine < DateTime.Now)
            {
                ModelState.AddModelError("DeadLine", "Die Deadline muss nach dem aktuellen Datum liegen.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> CloseProject(int id)
        {
            var isAdmin = User.IsInRole("Admin");

            if (User.IsInRole("Admin"))
            {
                var project = await _context.Projects.FindAsync(id);
                if(project == null)
                {
                    return NotFound();
                }
                else
                {
                    project.ProjectClosed = true;
                    var tickets = _context.Tickets.Where(t => t.ProjectId == id);
                    if(tickets != null)
                    {
                        foreach(var ticket in tickets)
                        {
                            ticket.TicketClosed = true;
                        }
                        _context.SaveChanges();
                    }
                }
            }
            else
            {
                TempData["UserError"] = "Nur Admins können Projekte schließen.";
            }
            return RedirectToAction("Index", "Ticket");
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
