namespace Skeleton.Web.Models {
    using Properties;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class ProjectsViewModel : IViewModel {
        public ProjectsViewModel() {
            Title = Resources.ChooseProjectPageTitle;
        }

        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public string Title { get; set; }
        public IEnumerable<string> Projects { get; set; }
        [DisplayName("Project Name")]
        [Required]
        public string SelectedProject { get; set; }
    }
}