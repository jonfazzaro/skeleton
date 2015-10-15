namespace Skeleton.Web.Models {
    using Properties;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class SignInViewModel : IViewModel {
        public SignInViewModel() {
            Title = Resources.SignInPageTitle;
        }

        public string Title { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public string Password { get; set; }
        public IEnumerable<string> Projects { get; set; }
        [DisplayName("Project Collection URL")]
        public string Url { get; set; }
        public string Username { get; set; }
        [DisplayName("Remember me")]
        public bool RememberMe { get; set; }
    }
}