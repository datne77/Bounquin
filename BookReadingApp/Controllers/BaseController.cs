using BookReadingApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace BookReadingApp.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IAccountRepository _accountRepo;
        protected readonly ICategoryRepository _categoryRepo;
        protected readonly IBookRepository _bookRepo;

        public BaseController(IAccountRepository accountRepo, ICategoryRepository categoryRepo, IBookRepository bookRepo)
        {
            _accountRepo = accountRepo;
            _categoryRepo = categoryRepo;
            _bookRepo = bookRepo;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _accountRepo.GetUserByIdAsync(userId).Result;

                ViewBag.AvatarPath = string.IsNullOrEmpty(user?.ProfilePictureUrl)
                    ? "/uploads/default-avatar.png"
                    : user.ProfilePictureUrl;
            }

            base.OnActionExecuting(context);
        }

    }
}
