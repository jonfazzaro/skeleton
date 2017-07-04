using Skeleton.Web.Properties;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Skeleton.Web.Models
{
    public class ProjectsViewModel : IViewModel
    {
        public ProjectsViewModel()
        {
            Title = Resources.ChooseProjectPageTitle;
        }

        public IEnumerable<string> Projects { get; set; }
        public IEnumerable<string> Areas { get; set; }

        [DisplayName("Project Name")]
        [Required]
        public string SelectedProject { get; set; }

        [DisplayName("Area Name")]
        [Required]
        public string SelectedArea { get; set; }

        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public string Title { get; set; }
    }
}