namespace SecurityTokenService.ViewModels
{
    using System.ComponentModel;

    public class SignInViewModel
    {
        [DisplayName("User Name")]
        public string UserName { get; set; }
        [DisplayName("Password")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}