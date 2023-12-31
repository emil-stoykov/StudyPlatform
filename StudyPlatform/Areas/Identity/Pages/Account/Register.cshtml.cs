﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using StudyPlatform.Data.Models;
using static StudyPlatform.Common.GeneralConstants;
using static StudyPlatform.Common.ModelValidationConstants.Users;
using static StudyPlatform.Common.ErrorMessages.Users;
using static StudyPlatform.Common.ViewModelConstants.Account;
using StudyPlatform.Infrastructure;

namespace StudyPlatform.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration config,
            RoleManager<IdentityRole<Guid>> roleManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _config = config;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "First Name")]
            [StringLength(FirstNameMaxLength, ErrorMessage = FirstNameNotInRange, MinimumLength = FirstNameMinLength)]
            public string FirstName { get; set; }

            [Display(Name = "Middle Name")]
            [StringLength(MiddleNameMaxLength, ErrorMessage = MiddleNameNotInRange, MinimumLength = MiddleNameMinLength)]
            public string? MiddleName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            [StringLength(LastNameMaxLength, ErrorMessage = LastNameNotInRange, MinimumLength = LastNameMinLength)]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Age")]
            [Range(AgeMin, AgeMax)]
            public int Age { get; set; }

            public string Role { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                ApplicationUser user = CreateUser();
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.MiddleName = Input.MiddleName;
                user.Age = Input.Age.ToString();
                await _userStore.SetUserNameAsync(user, Input.Username, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    string roleName = this._config["RoleNames:StudentRoleName"];
                    bool roleExists = await this._roleManager.RoleExistsAsync(roleName);

                    if (roleExists)
                    {
                        IdentityResult roleResult = await this._userManager.AddToRoleAsync(user, roleName);

                        if (!roleResult.Succeeded)
                        {
                            throw new InvalidOperationException($"Could not add role for {nameof(user)}");
                        }

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }      
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

    }
}
