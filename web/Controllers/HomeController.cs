using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Skeleton.Web.Cards;
using Skeleton.Web.Models;
using Skeleton.Web.Properties;
using Skeleton.Web.State;

namespace Skeleton.Web.Controllers
{
    [RoutePrefix("")]
    public class HomeController : Controller
    {
        private readonly IProjectsClient _projects;
        private readonly ISessionProvider _provider;

        public HomeController(ISessionProvider sessionProvider, IProjectsClient projects)
        {
            _provider = sessionProvider;
            _projects = projects;
        }

        [Route("")]
        public ActionResult Index()
        {
            if (_provider.Session == null)
                return RedirectToAction("signin");

            return RedirectToAction("projects");
        }

        [Route("map/{projectName}")]
        public ActionResult Map(string projectName, int depth = 0)
        {
            if (_provider.Session == null)
                return RedirectToAction("signin");

            if (string.IsNullOrWhiteSpace(projectName))
                return RedirectToAction("projects");

            return View(MapViewModel(projectName, depth));
        }

        [Route("projects")]
        public ActionResult Projects()
        {
            if (_provider.Session == null)
                return RedirectToAction("signin");

            return View(ProjectsViewModel());
        }

        [Route("projects")]
        [HttpPost]
        public ActionResult Projects(ProjectsViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SelectedProject))
                return View(model);

            return Redirect(MapUrl(model.SelectedProject));
        }

        [Route("signin")]
        public ActionResult Signin()
        {
            return View(new SignInViewModel());
        }

        [Route("signin")]
        [HttpPost]
        public async Task<ActionResult> Signin(SignInViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                StartSession(model);
                await LoadProjects();
                return RedirectToAction("projects");
            }
            catch (Exception)
            {
                return Error(model);
            }
        }

        [Route("signout")]
        public ActionResult Signout()
        {
            _provider.Session = null;
            return RedirectToAction("index");
        }

        private ActionResult Error(IViewModel model)
        {
            model.HasError = true;
            model.ErrorMessage = Resources.ConnectionErrorMessage;
            return View(model);
        }

        private async Task LoadProjects()
        {
            _provider.Session.Projects = await _projects.GetProjectNames();
            _provider.Session.ProjectPriorityFieldNames =
                await _projects.GetProjectPriorityFieldNames(_provider.Session.Projects);
        }

        private static string MapUrl(string projectName)
        {
            return "map/" + projectName.ToLower();
        }

        private MapViewModel MapViewModel(string projectName, int depth)
        {
            return new MapViewModel
            {
                Title = projectName,
                ProjectName = projectName,
                BaseUrl = _provider.Session.Url,
                Depth = depth
            };
        }

        private ProjectsViewModel ProjectsViewModel()
        {
            return new ProjectsViewModel
            {
                Projects = _provider.Session.Projects
            };
        }

        private void StartSession(SignInViewModel model)
        {
            var session = new SkeletonSession
            {
                Url = model.Url,
                Username = model.Username,
                Password = model.Password,
                RememberMe = model.RememberMe
            };

            _provider.Session = session;
        }
    }
}