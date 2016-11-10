using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Skeleton.Web.Properties;

namespace Skeleton.Web.Models
{
    public class ProjectsViewModel : IViewModel
    {
        public ProjectsViewModel()
        {
            Title = Resources.ChooseProjectPageTitle;
        }

        public IEnumerable<string> Projects { get; set; }

        [DisplayName("Project Name")]
        [Required]
        public string SelectedProject { get; set; }

        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public string Title { get; set; }
    }
}