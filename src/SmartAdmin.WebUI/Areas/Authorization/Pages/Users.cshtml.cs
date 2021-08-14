using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Razor.Application.Common.Extensions;
using CleanArchitecture.Razor.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using System.Linq.Dynamic.Core;
using CleanArchitecture.Razor.Application.Common.Mappings;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using CleanArchitecture.Razor.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Razor.Application.Common.Interfaces;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Data;

namespace SmartAdmin.WebUI.Areas.Authorization.Pages
{
    [Authorize]
    public class UserModel : PageModel
    {
        [BindProperty]
        public RegisterModel RegisterFormModel { get; set; } = new();
        [BindProperty]
        public EditUserModel EditFormModel { get; set; } = new();
        [BindProperty]
        public ResetPasswordModel ResetFormModel { get; set; } = new();
        [BindProperty]
        public IFormFile UploadedFile { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IExcelService _excelService;
        private readonly IStringLocalizer<UserModel> _localizer;

        public UserModel(
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IExcelService excelService,
            IStringLocalizer<UserModel> localizer
            )
        {
            _userManager = userManager;
            _mapper = mapper;
            _excelService = excelService;
            _localizer = localizer;
        }

        public async Task OnGetAsync()
        {

        }
        public async Task<IActionResult> OnGetDataAsync(int page=1,int rows=15,string sort="UserName",string order="asc",string filterRules="") {
            var filters = PredicateBuilder.FromFilter<ApplicationUser>(filterRules);
            var data=await _userManager.Users.Where(filters)
                   .OrderBy($"{sort} {order}")
                   .PaginatedDataAsync(page, rows);
            return new JsonResult(data);
        }

        public async Task<IActionResult> OnPostRegisterAsync()
        {
            var user = new ApplicationUser {
                EmailConfirmed = true,
                IsActive = false,
                Site = RegisterFormModel.Site,
                DisplayName = RegisterFormModel.DisplayName,
                UserName = RegisterFormModel.UserName,
                Email = RegisterFormModel.Email,
                PhoneNumber = RegisterFormModel.PhoneNumber
                 };
            var result = await _userManager.CreateAsync(user, RegisterFormModel.Password);
            return new JsonResult(result.ToApplicationResult());
        }
        public async Task<IActionResult> OnPostEditAsync()
        {
            var user = await _userManager.FindByIdAsync(EditFormModel.Id);
            user.DisplayName=EditFormModel.DisplayName;
            user.PhoneNumber=EditFormModel.PhoneNumber;
            user.Email=EditFormModel.Email;
            user.Site=EditFormModel.Site;
            var result = await _userManager.UpdateAsync(user);
            return new JsonResult(result.ToApplicationResult());
        }
        public async Task<IActionResult> OnPostResetPasswordAsync()
        {
           
            var user = await _userManager.FindByIdAsync(ResetFormModel.Id);
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, code, ResetFormModel.Password);
            
            return new JsonResult(result.ToApplicationResult());
        }
        public async Task<IActionResult> OnGetUnLockAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.SetLockoutEndDateAsync(user, System.DateTimeOffset.MaxValue);
            return new JsonResult(result.ToApplicationResult());
        }
        public async Task<IActionResult> OnGetLockAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.SetLockoutEndDateAsync(user, System.DateTimeOffset.Now.AddMinutes(-30));
            return new JsonResult(result.ToApplicationResult());
        }
        public async Task<IActionResult> OnGetDeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.DeleteAsync(user);
            return new JsonResult(result.ToApplicationResult());
        }
        public async Task<IActionResult> OnGetActiveCheckedAsync([FromQuery] string[] id)
        {
            foreach (var key in id)
            {
                var user = await _userManager.FindByIdAsync(key);
                user.IsActive = true;
                var result = await _userManager.UpdateAsync(user);
            }
            return new JsonResult(Result.Success());
        }
        public async Task<IActionResult> OnGetDeleteCheckedAsync([FromQuery] string[] id)
        {
            foreach(var key in id)
            {
                var user = await _userManager.FindByIdAsync(key);
                var result = await _userManager.DeleteAsync(user);
            }
            return new JsonResult(Result.Success());
        }
        public async Task<FileResult> OnPostExportAsync(string sort = "UserName", string order = "asc", string filterRules = "")
        {
            var filters = PredicateBuilder.FromFilter<ApplicationUser>(filterRules);
            var data = await _userManager.Users.Where(filters)
                   .OrderBy($"{sort} {order}")
                   .ToListAsync();
            var result = await _excelService.ExportAsync(data, new Dictionary<string, System.Func<ApplicationUser, object>>()
            {
                { _localizer["Site"], item => item.Site },
                { _localizer["User Name"], item => item.UserName },
                { _localizer["Display Name"], item => item.DisplayName },
                { _localizer["Phone Number"], item => item.PhoneNumber },
                { _localizer["Email"], item => item.Email }
            }, _localizer["ApplicationUsers"]);
            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", _localizer["ApplicationUsers"] + ".xlsx");
        }
        public async  Task<FileResult> OnGetCreateTemplate()
        {
            var fields = new string[] {
                _localizer["Site"],
                _localizer["User Name"],
                _localizer["Display Name"],
                _localizer["Phone Number"],
                _localizer["Email"],
                _localizer["Password"],
              };
            var result = await _excelService.CreateTemplateAsync(fields, _localizer["ApplicationUsers"]);
            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", _localizer["ApplicationUsers"] + ".xlsx");
        }
        public async Task<IActionResult> OnPostImportAsync()
        {
            try
            {
                var stream = new MemoryStream();
                await UploadedFile.CopyToAsync(stream);
                var data = stream.ToArray();
                var result = await _excelService.ImportAsync(data, mappers: new Dictionary<string, Func<DataRow, RegisterModel, object>>
            {
                { _localizer["Site"], (row,item) => item.Site = row[_localizer["Site"]]?.ToString() },
                { _localizer["User Name"], (row,item) => item.UserName = row[_localizer["User Name"]]?.ToString() },
                { _localizer["Display Name"], (row,item) => item.DisplayName =  row[_localizer["Display Name"]]?.ToString() },
                { _localizer["Phone Number"], (row,item) => item.PhoneNumber =  row[_localizer["Phone Number"]]?.ToString() },
                { _localizer["Email"], (row,item) => item.Email =  row[_localizer["Email"]]?.ToString() },
                { _localizer["Password"], (row,item) => item.Password =  row[_localizer["Password"]]?.ToString() }
            }, _localizer["ApplicationUsers"]);
                if (result.Succeeded)
                {
                    var importItems = result.Data;
                    foreach(var item in importItems)
                    {
                        var user = new ApplicationUser
                        {
                            EmailConfirmed = true,
                            IsActive = false,
                            Site = item.Site,
                            DisplayName = item.DisplayName,
                            UserName = item.UserName,
                            Email = item.Email,
                            PhoneNumber = item.PhoneNumber
                        };
                        var iresult = await _userManager.CreateAsync(user, item.Password);
                        if (iresult.Succeeded == false)
                        {
                            return new JsonResult(Result.FailureAsync(iresult.Errors.Select(x=>x.Description)));
                        }
                    }
                }
                return new JsonResult(Result.Success());
            }catch(Exception e)
            {
                return new JsonResult(Result.Failure(new string[]{ e.Message}));
            }
        }
        public class RegisterModel
        {
            [Display(Name = "Site")]
            [Required]
            public string Site { get; set; }
            [Display(Name = "User Name")]
            [Required]
            public string UserName { get; set; }
            [Required]
            [Display(Name = "Display Name")]
            public string DisplayName { get; set; }
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

         
        }
        public class EditUserModel
        {
            public string Id { get; set; }
            [Display(Name = "Site")]
            [Required]
            public string Site { get; set; }
            [Display(Name = "User Name")]
            [Required]
            public string UserName { get; set; }
            [Required]
            [Display(Name = "Display Name")]
            public string DisplayName { get; set; }
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }





        }
        public class ResetPasswordModel
        {
            [Required]
            public string Id { get; set; }
            [Required]
            public string UserName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }
    }


}
