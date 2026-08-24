using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using TicketSystem.Database;
using TicketSystem.Models;
using TicketSystem.ViewModels;


namespace TicketSystem.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<IdentityUser> userManager;
        private SignInManager<IdentityUser> signInManager;
        private readonly TicketSystemDbContext _ctx;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,TicketSystemDbContext ctx)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _ctx = ctx;
        }
        [HttpGet]
        public IActionResult Login(string returnURL)
        {
            return View(new LoginModel()
            {
                Username = string.Empty,
                Password = string.Empty,
                ReturnUrl = returnURL
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser? user = await userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    var result = await signInManager.PasswordSignInAsync(
                            user, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        return Redirect(model.ReturnUrl ?? "/");
                    }
                }
                ModelState.AddModelError("", "Username oder Passwort ungültig");
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }
        public IActionResult AccessDenied(string returnUrl)
        {
            return View("AccessDenied", returnUrl);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult UserAdd()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UserAdd(UserCreateModel user)
        {
            if (ModelState.IsValid)
            {
                if (await userManager.FindByNameAsync(user.Name) == null)
                {

                    var userAdd = new IdentityUser(user.Name);
                    var passwordValidator = new PasswordValidator<IdentityUser>();
                    var passwordValidationResult = await passwordValidator.ValidateAsync(userManager, userAdd, user.Password);

                    if (passwordValidationResult.Succeeded)
                    {
                        await userManager.CreateAsync(userAdd, user.Password);
                        await userManager.AddToRoleAsync(userAdd, user.Role);
                        var users = userManager.Users.ToList();
                        var model = new List<UserWithRoles>();
                        foreach (var user1 in users)
                        {
                            var roles = await userManager.GetRolesAsync(user1);
                            model.Add(new UserWithRoles
                            {
                                UserId = user1.Id,
                                UserName = user1.UserName,
                                Roles = roles.ToList()
                            });
                        }
                        TempData["Success"] = user.Name + " wurde erfolgreich hinzugefügt.";
                        return View("UserList", model);
                    }
                    foreach (var error in passwordValidationResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();

                }
                ModelState.AddModelError("", "User bereits vorhanden");
                return View();
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserList()
        {
            var users = userManager.Users.ToList();
            var model = new List<UserWithRoles>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                model.Add(new UserWithRoles
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Roles = roles.ToList()
                });
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await signInManager.SignOutAsync();
                TempData["Success"] = "Passwort erfolgreich geändert. Bitte melden Sie sich mit dem neuen Passwort erneut an.";
                return Redirect("Login");
            }
            if (await userManager.CheckPasswordAsync(user, model.OldPassword))
            {
                TempData["Fail"] = "Das neue Password entspricht nicht der von uns erwarteten Anforderungen.";
                return Redirect("ChangePassword");
            }
            TempData["Fail"] = "Das aktuelle Password war falsch. Nichts wurde geändert.";
            return Redirect("ChangePassword");

        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangeUsername(string id)
        {
            var model = new ChangeUsernameViewModel()
            {
                UserId = id,
                NewUsername = string.Empty
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUsername(ChangeUsernameViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Prüfen, ob der Benutzername bereits existiert
            var existingUser = await userManager.FindByNameAsync(model.NewUsername);
            if (existingUser != null)
            {
                ModelState.AddModelError("NewUsername", "Dieser Benutzername ist bereits vergeben.");
                return View(model);
            }
            var user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
            return RedirectToAction("Login");

            user.UserName = model.NewUsername;
            user.NormalizedUserName = model.NewUsername.ToUpper();

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await signInManager.SignOutAsync();
                TempData["Success"] = "Benutzername erfolgreich geändert. Bitte melden Sie sich mit dem neuen Benutzernamen erneut an.";
                return RedirectToAction("Login");
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Fail"] = "User nicht gefunden.";
                return RedirectToAction("UserList");
            }

            // Prüfe auf offene Tickets
            var offeneTickets = _ctx.Tickets.Any(t => t.IdentityUserId == userId);
            // Prüfe auf Kommentare
            var userKommentare = _ctx.Comments.Any(c => c.IdentityUserId == userId);
            // Prüfe auf Bearbeitung
            var userBearbeiter = _ctx.Tickets.Any(t => t.BearbeiterId == userId);
            // Prüfe auf Dateiupload
            var userUpload = _ctx.UploadFiles.Any(u => u.IdentityUserId == userId);
            // Prüfe auf mail
            var userMail = _ctx.Mails.Any(m => m.EmpfangerId == userId || m.SenderId == userId);
            if(offeneTickets || userKommentare || userBearbeiter || userUpload || userMail)
            {
                if (offeneTickets)
                {
                    TempData["OpenTicketFail"] = "Benutzer kann nicht gelöscht werden, da er Tickets eröffnet hat.";
                }
                if (userKommentare)
                {
                    TempData["OpenCommentFail"] = "Benutzer kann nicht gelöscht werden, solange Kommentare existieren.";
                }
                if (userBearbeiter)
                {
                    TempData["OpenBearbeiterFail"] = "Benutzer kann nicht gelöscht werden, da er als Bearbeiter eingetragen ist.";
                }
                if (userUpload)
                {
                    TempData["OpenUploadFail"] = "Benutzer kann nicht gelöscht werden, da er Dateien hochgeladen hat.";
                }
                if (userMail)
                {
                    TempData["OpenMailFail"] = "Benutzer kann nicht gelöscht werden, da Mails von ihm existieren.";
                }
                return RedirectToAction("UserList");
            }
            

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "User erfolgreich gelöscht.";
            }
            else
            {
                TempData["Fail"] = "Fehler beim Löschen des Users.";
            }
            return RedirectToAction("UserList");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ChangeRole(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var roles = new List<string> { "Admin", "Developer", "Tester" };
            var model = new ChangeRoleViewModel
            {
                UserId = userId,
                Roles = roles
            };

            TempData["User"] = "Benutzer: " + user.UserName;
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(ChangeRoleViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var currentRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, currentRoles);
            await userManager.AddToRoleAsync(user, model.NewRole);

            TempData["User"] = "Benutzer: " + user.UserName;
            TempData["ChangeRoleSuccess"] = "Rolle erfolgreich geändert.";
            return RedirectToAction("UserList");
        }
        [Authorize]
        public async Task<IActionResult> PersonalSite()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = await userManager.FindByIdAsync(userId);

            var workingTickets = _ctx.Tickets
                .Include(p => p.Project)
                .Include(e => e.IdentityUser)
                .Where(t => t.BearbeiterId == userId).ToList();

            ViewBag.WorkingList = workingTickets;

            var createTickets = _ctx.Tickets
                .Include(b => b.Bearbeiter)
                .Include(p => p.Project)
                .Where(t => t.IdentityUserId == userId).ToList();
            ViewBag.MadeList = createTickets;

            return View(user);
        }
        public async Task<IActionResult> ChangePasswordAdmin(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return NotFound();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var model = new ChangePasswordAdminViewModel
            {
                UserId = user.Id,
                Username = user.UserName
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePasswordAdmin(ChangePasswordAdminViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                TempData["Fail"] = "Benutzer nicht gefunden.";
                return RedirectToAction("UserList");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["Success"] = $"Passwort für Benutzer {user.UserName} wurde erfolgreich geändert.";
                return RedirectToAction("UserList");
            }
            else
            {
                TempData["Fail"] = "Ein Fehler ist aufgetreten!";
                model.Username = user.UserName;
                return View(model);
            }
        }
        public IActionResult ChangeUsernameAdmin(string id)
        {
            var model = new ChangeUsernameViewModel()
            {
                UserId = id,
                NewUsername = string.Empty
            };
            TempData["User"] = "Benutzer: " + _ctx.Users.Where(u => u.Id == id).First().UserName;
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUsernameAdmin(ChangeUsernameViewModel model)
        {
            TempData["User"] = "Benutzer: " + _ctx.Users.Where(u => u.Id == model.UserId).First().UserName;
            if (!ModelState.IsValid)
                return View(model);

            // Prüfen, ob der Benutzername bereits existiert
            var existingUser = await userManager.FindByNameAsync(model.NewUsername);
            if (existingUser != null)
            {
                ModelState.AddModelError("NewUsername", "Dieser Benutzername ist bereits vergeben.");
                return View(model);
            }

            var user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
                return RedirectToAction("UserList");

            user.UserName = model.NewUsername;
            user.NormalizedUserName = model.NewUsername.ToUpper();

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Benutzername erfolgreich geändert.";
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            var users = userManager.Users.ToList();
            var model2 = new List<UserWithRoles>();

            foreach (var user2 in users)
            {
                var roles = await userManager.GetRolesAsync(user2);
                model2.Add(new UserWithRoles
                {
                    UserId = user2.Id,
                    UserName = user2.UserName,
                    Roles = roles.ToList()
                });
            }
            return View("UserList", model2);
        }

    }
}
