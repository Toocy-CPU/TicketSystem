using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Database;
using TicketSystem.Models;
using TicketSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace TicketSystem.Controllers
{
    public class MailController : Controller
    {
        private UserManager<IdentityUser> userManager;
        private readonly TicketSystemDbContext _ctx;

        public MailController(UserManager<IdentityUser> userManager, TicketSystemDbContext ctx)
        {
            this.userManager = userManager;
            _ctx = ctx;
        }
        [Authorize]
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var senders = _ctx.Mails
                .Include(m => m.Sender)
                .Where(m => m.EmpfangerId == userId)
                .Where(m => m.EmpfangerAnzeigen == true)
                .Select(m => m.Sender)
                .Where(u => u != null && u.Id != userId);

            var empfanger = _ctx.Mails
                .Include(m => m.Empfanger)
                .Where(m => m.SenderId == userId)
                .Where(m => m.AbsenderAnzeigen == true)
                .Select(m => m.Empfanger)
                .Where(u => u != null && u.Id != userId);

            var users = senders
                .Union(empfanger) 
                .Distinct()        
                .ToList();

            var preview = new List<ChatPreview>();
            foreach(var ele in users)
            {
                var lastmail = _ctx.Mails
                    .Where(m => m.EmpfangerId == ele!.Id || m.SenderId == ele.Id)
                    .OrderByDescending(m => m.SendDate)
                    .FirstOrDefault();
                preview.Add(new ChatPreview { User = ele, LastMail = lastmail });
            }
            preview = preview.OrderByDescending(m => m.LastMail?.SendDate).ToList();

            var sendUsers = _ctx.Users.Where(u => u.Id != userId).ToList();
            var selectUsers = sendUsers.Except(users).ToList();
            ViewBag.SelectUsers = new SelectList(selectUsers, "Id", "UserName");

            return View(preview);
        }
        [Authorize]
        [HttpGet]
        public IActionResult Chat(string GEtId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var chatMails = _ctx.Mails.Include(s => s.Sender).Include(e => e.Empfanger)
                .Where(m => m.SenderId == userId && m.EmpfangerId == GEtId && m.AbsenderAnzeigen == true ||
                 m.SenderId == GEtId && m.EmpfangerId == userId && m.EmpfangerAnzeigen == true)
                .OrderBy(m => m.SendDate).ToList();
            ViewBag.ChatMails = chatMails;

            var chatName = _ctx.Users.FirstOrDefault(u => u.Id == GEtId)?.UserName;
            ViewBag.ChatName = chatName;

            var mail = new Mail()
            {
                SenderId = userId,
                EmpfangerId = GEtId,
                Text = string.Empty
            };

            return View(mail);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Chat(Mail mail)
        {
            if (ModelState.IsValid)
            {
                mail.SendDate = DateTime.Now;
                _ctx.Mails.Add(mail);
                _ctx.SaveChanges();
            }
            return RedirectToAction("Chat",new { GEtId = mail.EmpfangerId});
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddMail()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            MailViewModel model = new MailViewModel();
            model.Users = userManager.Users.Where(u => u.Id != userId).ToList();
            model.SendMails = _ctx.Mails.Where(m => m.SenderId == userId).Include(m => m.Sender).ToList();
            model.GotMails = _ctx.Mails.Where(m => m.EmpfangerId == userId).Include(m => m.Empfanger).ToList();
            ViewBag.ViewModel = model;

            var mail = new Mail()
            {
                SenderId = userId,
                EmpfangerId = string.Empty,
                Text = string.Empty
            };

            return View(mail);
        }
        [Authorize]
        [HttpPost]
        public IActionResult AddMail(Mail mail)
        {
            
            if(ModelState.IsValid)
            {
                mail.SendDate = DateTime.Now;
                _ctx.Mails.Add(mail);
                _ctx.SaveChanges();
            }
            else 
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                MailViewModel model = new MailViewModel();
                model.Users = userManager.Users.Where(u => u.Id != userId).ToList();
                model.SendMails = _ctx.Mails.Where(m => m.SenderId == userId).Include(m => m.Sender).ToList();
                model.GotMails = _ctx.Mails.Where(m => m.EmpfangerId == userId).Include(m => m.Empfanger).ToList();
                ViewBag.ViewModel = model;
                return View(mail); 
            }
                return RedirectToAction("AddMail");
        }
        public IActionResult CloseMail(int id, bool empfangen)
        {
            var mail = _ctx.Mails.FirstOrDefault(m => m.Id == id);
            if(mail == null)
            {
                return NotFound();
            }
            if (!empfangen)
            {
                mail.EmpfangerAnzeigen = false;
            }
            else
            {
                mail.AbsenderAnzeigen = false;
            }
            if(mail.AbsenderAnzeigen == false && mail.EmpfangerAnzeigen == false)
            {
                _ctx.Mails.Remove(mail);             
            }
            _ctx.SaveChanges();

            return RedirectToAction("Chat", new { GEtId = empfangen ? mail.EmpfangerId : mail.SenderId });
        }
        public async Task<IActionResult> DeleteChat(string getId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _ctx.Mails.Where(m => m.EmpfangerId == getId && m.SenderId == userId)?
            .ForEachAsync(m => m.AbsenderAnzeigen = false);

            await _ctx.Mails.Where(m => m.EmpfangerId == userId && m.SenderId == getId)?
            .ForEachAsync(m => m.EmpfangerAnzeigen = false);

            var removeMails = _ctx.Mails.Where(m => m.EmpfangerAnzeigen == false && m.AbsenderAnzeigen == false).ToList();
            _ctx.Mails.RemoveRange(removeMails);
            await _ctx.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}