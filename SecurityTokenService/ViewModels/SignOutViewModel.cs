namespace SecurityTokenService.ViewModels
{
    using System.Collections.Generic;

    public class SignOutViewModel
    {
        public string ReturnUrl { get; set; }

        public IEnumerable<string> RealmsToSignOut { get; set; }
    }
}