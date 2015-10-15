namespace Skeleton.Web.Controllers {
    using Cards;
    using Models;
    using Properties;
    using State;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [RoutePrefix("")]
    public class HomeController : Controller {
        readonly ISessionProvider _provider;
        readonly IProjectsClient _projects;

        public HomeController(ISessionProvider sessionProvider, IProjectsClient projects) {
            _provider = sessionProvider;
            _projects = projects;
        }

        [Route("")]
        public ActionResult Index() {
            if (_provider.Session == null)
                return RedirectToAction("signin");

            return RedirectToAction("projects");
        }

        [Route("projects")]
        public ActionResult Projects() {
            if (_provider.Session == null)
                return RedirectToAction("signin");

            return View(ProjectsViewModel());
        }

        private ProjectsViewModel ProjectsViewModel() {
            return new ProjectsViewModel {
                Projects = _provider.Session.Projects
            };
        }

        [Route("projects")]
        [HttpPost]
        public ActionResult Projects(ProjectsViewModel model) {
            if (string.IsNullOrWhiteSpace(model.SelectedProject))
                return View(model);

            return Redirect(MapUrl(model.SelectedProject));
        }

        private static string MapUrl(string projectName) {
            return "map/" + projectName.ToLower();
        }

        [Route("map/{projectName}")]
        public ActionResult Map(string projectName, int depth = 0) {
            if (_provider.Session == null)
                return RedirectToAction("signin");

            if (string.IsNullOrWhiteSpace(projectName))
                return RedirectToAction("projects");

            return View(MapViewModel(projectName, depth));
        }

        private MapViewModel MapViewModel(string projectName, int depth) {
            return new MapViewModel {
                Title = projectName,
                ProjectName = projectName,
                BaseUrl = _provider.Session.Url,
                Depth = depth
            };
        }

        [Route("signout")]
        public ActionResult Signout() {
            _provider.Session = null;
            return RedirectToAction("index");
        }

        [Route("signin")]
        public ActionResult Signin() {
            return View(new SignInViewModel());
        }

        [Route("signin")]
        [HttpPost]
        public async Task<ActionResult> Signin(SignInViewModel model) {
            if (!ModelState.IsValid)
                return View(model);

            try {
                StartSession(model);
                await LoadProjects();
                return RedirectToAction("projects");
            } catch (Exception) {
                return Error(model);
            }
        }

        private void StartSession(SignInViewModel model) {
            var session = new SkeletonSession {
                Url = model.Url,
                Username = model.Username,
                Password = model.Password
            };

            _provider.Session = session;
        }

        private async Task LoadProjects() {
            _provider.Session.Projects = await _projects.GetProjectNames();
            _provider.Session.ProjectPriorityFieldNames =
                await _projects.GetProjectPriorityFieldNames(_provider.Session.Projects);
        }

        private ActionResult Error(IViewModel model) {
            model.HasError = true;
            model.ErrorMessage = Resources.ConnectionErrorMessage;
            return View(model);
        }
    }
}