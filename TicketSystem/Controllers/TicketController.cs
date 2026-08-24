using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Database;
using TicketSystem.Models;
using TicketSystem.ViewModels;

namespace TicketSystem.Controllers
{
    public class TicketController : Controller
    {
        private readonly TicketSystemDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public TicketController(TicketSystemDbContext context, UserManager<IdentityUser> usermanager)
        {
            _context = context;
            _userManager = usermanager;
        }
        [Authorize]
        public IActionResult Index()
        {

            var projects = _context.Projects.Include(t => t.Tickets).ToList();
            if(projects != null)
            {
                foreach (var ele in projects)
                {
                    ele.ClosedTickets = ele.Tickets.Where(t => t.TicketClosed == true).Count();
                    ele.OpenTickets = ele.Tickets.Where(t => t.TicketClosed == false).Count();
                }
            }
            
            return View(projects);
        }
        [HttpGet]
        [Authorize]
        public IActionResult AddTicket(int projectId)
        {
            var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);
            if(project == null)
            {
                return NotFound();
            }
            if(project.ProjectClosed == true)
            {
                TempData["ProjectCloedError"] = "Projekt ist bereits abgeschlossen.";
                return RedirectToAction("Index");
            }

            var users = _userManager.Users.ToList();
            var bearbeiter = new SelectList(users, nameof(IdentityUser.Id), nameof(IdentityUser.UserName));
            ViewBag.Bearbeiter = bearbeiter;

            var ticket = new Ticket()
            {
                Description = string.Empty,
                Title = string.Empty,
                ProjectId = projectId,
                IdentityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
            };            

            return View(ticket);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddTicket(Ticket ticket, IFormFile[] files)
        {
            ticket.CreatedAt = DateTime.Now;

            var users = _userManager.Users.ToList();
            var bearbeiter = new SelectList(users, nameof(IdentityUser.Id), nameof(IdentityUser.UserName));
            ViewBag.Bearbeiter = bearbeiter;

            if (ticket.BearbeiterId != null)
            {
                ticket.HandeledAt = DateTime.Now;
            }
            // Upload limit prüfung           
            if (files.Count() > 10)
            {
                TempData["CountError"] = "Es können maximal 10 Dateien zu einem Ticket hochgeladen werden.";
            }
            else
            {
                string? uploadSizeError = null;
                foreach (var file in files)
                {
                    if (file.Length > 1048577)
                    {
                        uploadSizeError += file.FileName + " ist größer als 1 MB. /*/";
                    }
                }
                TempData["UploadSizeError"] = uploadSizeError;
            }
            if(TempData["CountError"] != null || TempData["UploadSizeError"] != null)
            {                
                return View("AddTicket", ticket);
            }
            //
            if (ModelState.IsValid)
            {
                _context.Tickets.Add(ticket);
                _context.SaveChanges();
                var ticketFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files"
                    , _context.Projects.FirstOrDefault(p => p.Id == ticket.ProjectId)!.Title, ticket.Title);
                Directory.CreateDirectory(ticketFolder);

                if (files != null && files.Length > 0)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            var filePath = Path.Combine(ticketFolder, file.FileName);

                            using (var stream = System.IO.File.Create(filePath))
                            {
                                await file.CopyToAsync(stream);
                            }
                            _context.UploadFiles.Add(new UploadFile()
                            {
                                Filename = file.FileName,
                                Filesize = new FileInfo(filePath).Length / 1024,
                                Type = Path.GetExtension(file.FileName),
                                IdentityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                                TicketId = ticket.Id,
                                UploadedAt = DateTime.Now
                            });
                            _context.SaveChanges();
                        }
                    }
                }
                return RedirectToAction("TicketDetails", ticket);
            }
            return View();
        }
        [Authorize]
        public IActionResult ProjectDetails(int id)
        {
            var project = _context.Projects.Include(t => t.Tickets).ThenInclude(u => u.IdentityUser).OrderByDescending(t => t.CreatedAt).FirstOrDefault(p => p.Id == id); // Changed !!
            return View(project);

        }
        [Authorize]
        public IActionResult TicketDetails(int id)
        {
            var ticket = _context.Tickets
                .Include(u => u.IdentityUser)
                .Include(b => b.Bearbeiter)
                .Include(c => c.Closer)
                .Include(p => p.Project)
                .Include(up => up.UploadFiles)
                .Include(c => c.Comments)
                .ThenInclude(u => u.IdentityUser)
                .Include(b => b.BlockedTickets)
                    .ThenInclude(t => t.BlockedTicket)
                        .FirstOrDefault(t => t.Id == id);

            var blockingTickets = _context.BlockTickets.Include(t => t.Ticket)
                .Where(b => b.BlockedTicketId == id).Select(t => t.Ticket).ToList();
            ViewBag.BlockingList = blockingTickets;

            var ticketList = new SelectList(_context.Tickets
                .Where(t => t.Id != id)
                .Where(t => t.ProjectId == ticket.ProjectId).ToList(), "Id", "Title");
            ViewBag.TicketList = ticketList;
            return View(ticket);
        }
        [Authorize]
        public IActionResult TicketList()
        {
            var tickets = _context.Tickets.Include(u => u.IdentityUser).Include(p => p.Project)
               .OrderBy(t => t.Project.Title).ThenByDescending(t => t.CreatedAt);  // selig fragen

            ViewBag.Projects = new SelectList(_context.Projects.ToList(), "Id", "Title", -1);
            ViewBag.UserCreater = new SelectList(_userManager.Users.ToList(), "Id", "UserName", "-1");
            ViewBag.UserBearbeiter = new SelectList(_userManager.Users.ToList(), "Id", "UserName", "-1");

            FilterViewModel m1 = new FilterViewModel();
            m1.Liste1 = tickets;
            foreach (var item in _context.Users)
            {
                m1.CategoryOptions.Add(new SelectListItem { Value = item.Id, Text = item.UserName });
            }

            return View(m1);
        }
        [HttpGet]
        [Authorize]
        public IActionResult TicketListFiltered(string CreaterId, int ProjectId, string BearbeiterId, string reset)
        {
            if (reset == "true")
            {
                ProjectId = -1;
                CreaterId = "-1";
                BearbeiterId = "-1";
            }

            var tickets = _context.Tickets.Include(u => u.IdentityUser).Include(p => p.Project)
                              .OrderBy(t => t.Project.Title).ThenByDescending(t => t.CreatedAt).ToList();

            if (ProjectId != -1)
            {
                tickets = tickets.Where(t => t.ProjectId == ProjectId).ToList();
            }
            if (CreaterId != "-1")
            {
                tickets = tickets.Where(t => t.IdentityUserId == CreaterId).ToList();
            }
            if (BearbeiterId == "-2")
            {
                tickets = tickets.Where(t => t.BearbeiterId == null).ToList();
            }
            else if (BearbeiterId != "-1")
            {
                tickets = tickets.Where(t => t.BearbeiterId == BearbeiterId).ToList();
            }

            ViewBag.SelectedProjectId = ProjectId;
            ViewBag.SelectedCreaterId = CreaterId;
            ViewBag.SelectedBearbeiterId = BearbeiterId;

            ViewBag.Projects = new SelectList(_context.Projects.ToList(), "Id", "Title", ProjectId);
            ViewBag.UserCreater = new SelectList(_userManager.Users.ToList(), "Id", "UserName", CreaterId);
            ViewBag.UserBearbeiter = new SelectList(_userManager.Users.ToList(), "Id", "UserName", BearbeiterId);

            FilterViewModel m1 = new FilterViewModel();

            m1.Liste1 = tickets;
            foreach (var item in _context.Users)
            {
                m1.CategoryOptions.Add(new SelectListItem { Value = item.Id, Text = item.UserName });
            }

            return View("TicketList", m1);
        }
        [HttpGet]
        [Authorize]
        public IActionResult AddComment(int ticketId)
        {
            var ticket = _context.Tickets.FirstOrDefault(t => t.Id == ticketId);
            if (ticket == null) { return NotFound(); }
            if (ticket.TicketClosed)
            {
                TempData["ClosedError"] = "Das Ticket ist bereits geschlossen";
                return RedirectToAction("TicketDetails", ticket);
            }
            var comment = new Comment()
            {
                Title =  string.Empty,
                Content = string.Empty,
                IdentityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                TicketId = ticketId
            };
            return View(comment);
        }
        [HttpPost]
        [Authorize]
        public IActionResult AddComment(Comment comment)
        {
            comment.CreatedAt = DateTime.Now;
            var ticket = _context.Tickets.FirstOrDefault(t => t.Id == comment.TicketId);

            if (ModelState.IsValid)
            {
                _context.Comments.Add(comment);
                _context.SaveChanges();
                return RedirectToAction("TicketDetails", new { id = comment.TicketId });
            }
            return View();                      
        }
        [HttpGet]
        [Authorize]
        public  IActionResult EditTicket(int? id)
        {
            var ticket = _context.Tickets.Include(p => p.Project).FirstOrDefault(t => t.Id == id);
            if (ticket == null) { return NotFound(); }
            if (ticket.TicketClosed)
            {
                TempData["ClosedError"] = "Das Ticket ist bereits geschlossen";
                return RedirectToAction("TicketDetails", ticket);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var isOwner = ticket.IdentityUserId == userId;
            var isBearbeiter = ticket.BearbeiterId == userId;

            if (!isAdmin && !isOwner && !isBearbeiter)
            {
                TempData["UserError"] = "Sie sind nicht berechtigt, dieses Ticket zu bearbeiten.";
                return RedirectToAction("TicketDetails", ticket);
            }


            var users = _userManager.Users.ToList();
            var bearbeiter = new SelectList(users, nameof(IdentityUser.Id), nameof(IdentityUser.UserName));
            ViewBag.Bearbeiter = bearbeiter;

            return View(ticket);
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditTicket( Ticket ticketEdit)
        {

            var ticket = await _context.Tickets.FindAsync(ticketEdit.Id);
            if (ticket != null)
            {
                ticket.Description = ticketEdit.Description != null ? ticketEdit.Description : ticket.Description;

                if(ticketEdit.BearbeiterId == "-1")
                {
                    ticket.BearbeiterId = null;
                    ticket.HandeledAt = default;
                }
                else
                {
                    ticket.BearbeiterId = ticketEdit.BearbeiterId;
                    ticket.HandeledAt = DateTime.Now;
                }
                   

                _context.Update(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction("TicketDetails",ticket);
            }           
                var users = _userManager.Users.ToList();
            var bearbeiter = new SelectList(users, nameof(IdentityUser.Id), nameof(IdentityUser.UserName));
            ViewBag.Bearbeiter = bearbeiter;
            return View(ticket);
        }
        [Authorize]
        public async Task<IActionResult> CloseTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) { return NotFound(); }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var isOwner = ticket.IdentityUserId == userId;
            var isBearbeiter = ticket.BearbeiterId == userId;

            if (!isAdmin && !isOwner && !isBearbeiter)
            {
                TempData["UserError"] = "Sie sind nicht berechtigt, dieses Ticket zu schließen.";
                return RedirectToAction("TicketDetails", ticket);
            }
            // blocking tickets
            var blockingList = _context.BlockTickets.Include(t => t.Ticket).Where( b => b.BlockedTicketId == id).ToList();
            string blockString = string.Empty;
            if(blockingList.Count() >0)
            {
                foreach ( var blocking in blockingList)
                {
                    if(blocking.Ticket != null)
                    {
                        blockString += "'" + blocking.Ticket.Title + "'" + " muss vorher erledigt werden. /*/";
                    }
                }
                TempData["BlockClosing"] = blockString;
                return RedirectToAction(nameof(TicketDetails), ticket);
            }
            //

            ticket.TicketClosed = true;
            ticket.CloserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            ticket.ClosedAt = DateTime.Now;

            // alle blockierungen lösen
            var blockedList = _context.BlockTickets.Where(b => b.TicketId == id);
            _context.BlockTickets.RemoveRange(blockedList);
            //

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TicketDetails),ticket);
        }
        public bool BlockTicketEvaluate(int blockId, int getBlockedId)
        {
            var ticketBlocked = _context.Tickets.Include(t => t.BlockedTickets).FirstOrDefault(t => t.Id == getBlockedId);

            if (ticketBlocked?.BlockedTickets != null)
            {
                foreach (var ele in ticketBlocked.BlockedTickets)
                {
                    //if (ele.BlockedTicketId == ele.TicketId) { return false; }
                    if (ele.BlockedTicketId == blockId) { return false; }
                    else if(!BlockTicketEvaluate(blockId, ele.BlockedTicketId))
                    {
                        return false;
                    }
                }
            }
            return true;
     
        }
        public IActionResult BlockTicket(int blockId, int getBlockedId)
        {
            var ticket = _context.Tickets.Include(t => t.BlockedTickets).FirstOrDefault(t => t.Id == blockId);
            var targetBlockTicket = _context.Tickets.Include(t => t.BlockedTickets).FirstOrDefault(t => t.Id == getBlockedId);

            if (ticket == null || targetBlockTicket == null) { return NotFound(); }

            if (targetBlockTicket.TicketClosed)
            {
                TempData["BlockError"] = $"Das Ticket '{targetBlockTicket.Title}' ist bereits geschlossen.";
                return RedirectToAction("TicketDetails", ticket);
            }
            if(ticket.BlockedTickets!.Select(t => t.BlockedTicketId).Contains(getBlockedId))
            {
                TempData["BlockError"] = $"Das Ticket '{targetBlockTicket.Title}' wird bereits geblockt.";
                return RedirectToAction("TicketDetails", ticket);
            }
            if (BlockTicketEvaluate(blockId, getBlockedId))
            {
                var blockAdd = new BlockTicket()
                {
                    TicketId = blockId,
                    BlockedTicketId = getBlockedId,
                    BlocketAt = DateTime.Now
                };
                _context.BlockTickets.Add(blockAdd);
                _context.SaveChanges();
                TempData["BlockSuccess"] = $"Das Ticket '{targetBlockTicket.Title}' wurde erfolgreich geblockt.";
            }
            else
            {
                TempData["BlockError"] = $"Blocken des Tickets '{targetBlockTicket.Title}' würde eine Schleife auslösen.";
            }
            return RedirectToAction("TicketDetails",ticket);
        }
        public IActionResult UnblockTicket(int id)
        {           
            var blockEntry = _context.BlockTickets.FirstOrDefault(b => b.Id == id);            
            if (blockEntry == null) { return NotFound(); }
            var ticket = _context.Tickets.FirstOrDefault(t => t.Id == blockEntry.TicketId);

            _context.BlockTickets.Remove(blockEntry);
            _context.SaveChanges();
            return RedirectToAction("TicketDetails", ticket);
        }
        // was darf nicht passieren wenn tickets geblockt sind? wer darf blocken und entblocken?
    }
}
