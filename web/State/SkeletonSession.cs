using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Skeleton.State {
    public class SkeletonSession {
        public const string Key = "_skeleton";

        [DisplayName("URL")]
        [Required]
        public string Url { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public IEnumerable<string> Projects { get; set; }
        public Dictionary<string, string> ProjectPriorityFieldNames { get; set; }
    }
}