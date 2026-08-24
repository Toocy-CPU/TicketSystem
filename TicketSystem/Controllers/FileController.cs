using System.IO;
using System.Net.Sockets;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using TicketSystem.Database;
using TicketSystem.Models;

namespace TicketSystem.Controllers
{  
    public class FileController : Controller
    {
        private readonly TicketSystemDbContext _ctx;
        private readonly UserManager<IdentityUser> _userManager;
        public FileController(TicketSystemDbContext content, UserManager<IdentityUser> usermanager)
        {
            _userManager = usermanager;
            _ctx = content;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index(int id)
        {
            var ticket = _ctx.Tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null) { return NotFound(); }

            return View(ticket);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Index(int id,IFormFile[] files)
        {
            var ticket = _ctx.Tickets.Include(p => p.Project).Include(up => up.UploadFiles).FirstOrDefault(t => t.Id == id);
            if (ticket == null) { return NotFound(); }
            string? uploadSuccess = null;
            string? uploadFailed = null;
            string? uploadSizeError = null;

            if(files == null || files.Length <= 0)
            {
                TempData["CountError"] = "Keine Datei ausgewählt.";
            }
            else if((ticket.UploadFiles?.Count() + files.Count()) > 11)
            {
                TempData["CountError"] = "Es können maximal 10 Dateien zu einem Ticket hochgeladen werden.";
            }
            else 
            {             
                foreach (var file in files)
                {
                    if(file.Length > 1048577) { uploadSizeError += file.FileName + " ist größer als 1 MB. /*/"; }
                    else if(ticket.UploadFiles.Select(u => u.Filename).Contains(file.FileName)) { uploadFailed += file.FileName + " bereits vorhanden /*/"; }
                    else if (file.Length > 0)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files"
                            ,ticket.Project.Title,ticket.Title,file.FileName); // nochmal gucken

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await file.CopyToAsync(stream);
                        }
                        _ctx.UploadFiles.Add(new UploadFile()
                        {
                            Filename = file.FileName,
                            Filesize = new FileInfo(Path.Combine(filePath)).Length / 1024,
                            Type = Path.GetExtension(file.FileName),
                            IdentityUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                            TicketId = ticket.Id,
                            UploadedAt = DateTime.Now
                        });
                        _ctx.SaveChanges();
                        uploadSuccess += file.FileName + " erfolgreich hochgeladen /*/";
                    }
                    
                }               
            }           
            TempData["UploadSizeError"] = uploadSizeError;
            TempData["UploadFailed"] = uploadFailed;
            TempData["UploadSuccess"] = uploadSuccess;
            return RedirectToAction("TicketDetails", "Ticket", ticket);
        }
        [Authorize]
        public IActionResult Download(int fileId) // parameter muss den selben namen haben wie in asp-route-xxx ..wird so als query string angehängt sonst muss es in der route in cs.programm gemacht werden
        {
            var file = _ctx.UploadFiles.FirstOrDefault(t => t.Id == fileId);
            if (file!= null)
            {
                var ticket = _ctx.Tickets.Include(p => p.Project).FirstOrDefault(t => t.Id == file.TicketId);

                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files"
                    ,ticket.Project.Title,ticket.Title , file.Filename);
                if (System.IO.File.Exists(filepath))
                {
                    var contType = "application/octet-stream";
                    return PhysicalFile(filepath, contType, file.Filename);
                }
            }
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public IActionResult RemoveFile(int fileId)
        {            
            var file = _ctx.UploadFiles.FirstOrDefault(t => t.Id == fileId);
            if (file != null)
            {
                var ticket = _ctx.Tickets.Include(p => p.Project).FirstOrDefault(t => t.Id == file.TicketId);
               
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files"
                    , ticket.Project.Title, ticket.Title, file.Filename);
                // wer darf löschen?
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");
                var isOwner = ticket.IdentityUserId == userId;
                var isBearbeiter = ticket.BearbeiterId == userId;

                if (!isAdmin && !isOwner && !isBearbeiter)
                {
                    TempData["UserError"] = "Sie sind nicht berechtigt, diese Datei zu löschen.";
                    return RedirectToAction("TicketDetails","Ticket", ticket);
                }


                if (System.IO.File.Exists(filepath))
                {
                    TempData["FileDeleted"] = $"Die Datei {file.Filename} wurde gelöscht";
                    _ctx.UploadFiles.Remove(file);
                    _ctx.SaveChanges();                  

                    System.IO.File.Delete(filepath);

                    return RedirectToAction("TicketDetails", "Ticket", ticket);
                }              
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
