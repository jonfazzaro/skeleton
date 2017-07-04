using Skeleton.Web.Cards;
using Skeleton.Web.Models;
using Skeleton.Web.Properties;
using Skeleton.Web.State;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

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
        [Route("map/{projectName}/{areaName}")]
        public ActionResult Map(string projectName, string areaName = null, int depth = 0)
        {
            if (_provider.Session == null)
                return RedirectToAction("signin");

            if (string.IsNullOrWhiteSpace(projectName))
                return RedirectToAction("projects");

            return View(MapViewModel(projectName, areaName, depth));
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
        public async Task<ActionResult> Projects(ProjectsViewModel model)
        {
            if (!HasASelectedProject(model))
                return View(WithProjects(model));

            if (!HasASelectedArea(model))
                return View(WithProjects(await WithAreas(model)));

            return Redirect(MapUrl(model.SelectedProject, model.SelectedArea));
        }

        private ProjectsViewModel WithProjects(ProjectsViewModel model)
        {
            model.Projects = _provider.Session.Projects.OrderBy(p => p);
            return model;
        }

        private async Task<ProjectsViewModel> WithAreas(ProjectsViewModel model)
        {
            model.Areas = (await _projects.GetAreaNames(model.SelectedProject)).OrderBy(a => a);
            return model;
        }

        private static bool HasASelectedArea(ProjectsViewModel model)
        {
            return !string.IsNullOrWhiteSpace(model.SelectedArea);
        }

        private static bool HasASelectedProject(ProjectsViewModel model)
        {
            return !string.IsNullOrWhiteSpace(model.SelectedProject);
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

        private static string MapUrl(string projectName, string areaName)
        {
            return $"map/{projectName}/{areaName}".ToLower();
        }

        private MapViewModel MapViewModel(string projectName, string areaName, int depth)
        {
            return new MapViewModel
            {
                Title = projectName,
                ProjectName = projectName,
                AreaName = areaName,
                BaseUrl = _provider.Session.Url,
                Depth = depth
            };
        }

        private ProjectsViewModel ProjectsViewModel()
        {
            return WithProjects(new ProjectsViewModel());
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