namespace Skeleton.Web.Models {
    using System.Collections.Generic;

    public class SignInViewModel : IViewModel {
        public SignInViewModel() {
            Title = "Sign in";
        }

        public string Title { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public string Password { get; set; }
        public IEnumerable<string> Projects { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
    }
}