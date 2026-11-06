using CinemaSystem.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CinemaSystem.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<UserOTPS> _userOTP;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IRepository<UserOTPS> userOTP)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _userOTP = userOTP;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if(!ModelState.IsValid)
            {
                return View(registerVM);
            }
            ApplicationUser user = new ApplicationUser()
            {
                Name=registerVM.Name,
                Email = registerVM.Email,
                UserName=registerVM.UserName
            };
            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if(!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(registerVM);
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action("EmailConfirmation", "Account", new { area = "Identity" ,id = user.Id ,token }, Request.Scheme);
            await _emailSender.SendEmailAsync(registerVM.Email, "Email Vertification", $"<h1>Click <a href='{link}'>here</a> to Confirm Your Mail<h1>");
            TempData["Success"] = "Email is Sent Successfully";
            return RedirectToAction(nameof(EmailSending) , new {email = registerVM.Email});
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if(!ModelState.IsValid)
            {
                return View(loginVM);
            }
            var user = await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail) ??
                       await _userManager.FindByNameAsync(loginVM.UserNameOrEmail);
            if(user == null)
            {
                ModelState.AddModelError("", "Invaild Email or UserName or Password");
                return View(loginVM);
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);
            if(!result.Succeeded)    
            {
                if(result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "This Email Not Confirmed yet Confirm it First");
                }
                else if(result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Too Many Attempts try again later");
                }
                else
                {
                    ModelState.AddModelError("", "Invaild Email or UserName or Password");
                }
                
                return View(loginVM);
            }
            return RedirectToAction("Index" , "Home" , new {area = "Customer"});
        }
        public IActionResult EmailSending(string email)
        {
            ViewBag.Email = email;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> EmailConfirmation(string id , string token)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user==null)
            {
                return NotFound();
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "invalid user");

                TempData["Error"] = "Error in Email Confirmation";
                return RedirectToAction(nameof(Login));
            }
            TempData["Success"] = "Email is Confirmed Successfully";
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult ResendEmail()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmail(ResendEmailVM resendEmailVM)
        {
            var user = await _userManager.FindByEmailAsync(resendEmailVM.UserNameOrEmail) ??
                       await _userManager.FindByNameAsync(resendEmailVM.UserNameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError("", "Invaild Email or UserName or Password");
                return View(resendEmailVM);
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action("EmailConfirmation", "Account", new { area = "Identity", id = user.Id, token }, Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Email Vertification", $"<h1>Click <a href='{link}'>here</a> to Confirm Your Mail<h1>");
            return RedirectToAction(nameof(EmailSending), new { email = user.Email });
        }
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordVM.UserNameOrEmail) ??
                       await _userManager.FindByNameAsync(forgetPasswordVM.UserNameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError("", "Invaild Email or UserName or Password");
                return View(forgetPasswordVM);
            }
            
            string otp = new Random().Next(1000, 9999).ToString();
            
            UserOTPS userotp = new UserOTPS(user.Id, otp);
            
            await _userOTP.AddAsync(userotp);
            await _userOTP.SaveChangesAsync();

            await _emailSender.SendEmailAsync(user.Email, "Reset Password Mail", $"<h1>Enter This OTP {otp} to reset your password<h1>");
            TempData["Success"] = "OTP is Sent Successfully";
            return RedirectToAction(nameof(EnterOTP), new { UserId = user.Id , OTP = otp });
        }
        [HttpGet]
        public IActionResult EnterOTP(string UserId , string OTP)
        {
            return View(new EnterOTPVM()
            {
                UserId = UserId,
                OTP = OTP
            });
        }
        [HttpPost]
        public async Task<IActionResult> EnterOTP(EnterOTPVM enterOTPVM)
        {
            var user = await _userManager.FindByIdAsync(enterOTPVM.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "Invaild OTP");
                return View(enterOTPVM);
            }
            var userotps = await _userOTP.GetAllAsync(o => o.UserId == enterOTPVM.UserId && o.isValid == true && o.VaildTo >= DateTime.UtcNow);

            var userotp = userotps.OrderBy(o => o.CreatedAt).LastOrDefault();
            if(userotp == null || userotp.OTP != enterOTPVM.OTP)
            {
                ModelState.AddModelError("" , "Invalid / Expired OTP");
                return View(enterOTPVM);
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return RedirectToAction(nameof(ResetPassword), new { UserId = user.Id , Token = token });
        }
        [HttpGet]
        public IActionResult ResetPassword(string UserId , string Token)
        {
            return View(new ResetPasswordVM()
            {
                UserId = UserId,
                Token = Token
            });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if(!ModelState.IsValid)
            {
                return View(resetPasswordVM);
            }
            var user = await _userManager.FindByIdAsync(resetPasswordVM.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "Invaild Email or UserName or Password");
                return View(resetPasswordVM);
            }
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordVM.Token, resetPasswordVM.Password);
            if(!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(resetPasswordVM);
            }
            TempData["Success"] = "Your Password is Reset Successfully";
            return RedirectToAction(nameof(Login));
        }
    }
}
