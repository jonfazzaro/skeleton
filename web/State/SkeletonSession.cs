using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Skeleton.Web.State
{
    public class SkeletonSession
    {
        public const string Key = "_skeleton";

        [Required]
        public string Password { get; set; }

        public Dictionary<string, string> ProjectPriorityFieldNames { get; set; }
        public IEnumerable<string> Projects { get; set; }
        public bool RememberMe { get; set; }

        [DisplayName("URL")]
        [Required]
        public string Url { get; set; }

        [Required]
        public string Username { get; set; }
    }
}