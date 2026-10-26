using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationCar.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    //public class LoginViewModel
    //{
    //    [Required]
    //    [Display(Name = "Email")]
    //    [EmailAddress]
    //    public string Email { get; set; }

    //    [Required]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "Password")]
    //    public string Password { get; set; }

    //    [Display(Name = "Remember me?")]
    //    public bool RememberMe { get; set; }
    //}
    public class LoginViewModel
    {
        [Required(ErrorMessage = "شماره موبایل الزامی است")]
        [Display(Name = "شماره موبایل")]
        [RegularExpression(@"^09\d{9}$", ErrorMessage = "شماره موبایل نامعتبر است")]
        [StringLength(11, MinimumLength = 11)]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "رمز عبور الزامی است")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "رمز باید حداقل ۶ کاراکتر باشد")]
        public string Password { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        public string Code { get; set; }

        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "رمز جدید الزامی است")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز جدید")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "رمز حداقل ۶ کاراکتر")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "تکرار رمز الزامی است")]
        [DataType(DataType.Password)]
        [Display(Name = "تکرار رمز")]
        [Compare("NewPassword", ErrorMessage = "رمزها یکسان نیستند")]
        public string ConfirmPassword { get; set; }
        }



    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "شماره موبایل الزامی است")]
        [Display(Name = "شماره موبایل")]
        [RegularExpression(@"^09\d{9}$", ErrorMessage = "شماره موبایل نامعتبر است")]
        public string Mobile { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

}
