namespace Skeleton.Web.Tests.Controllers
{
    using Cards;
    using Fakes;
    using Models;
    using Moq;
    using NUnit.Framework;
    using Skeleton.Web.Controllers;
    using State;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [TestFixture]
    public class The_home_controller
    {
        private static ISessionProvider _sessionProvider;
        private static HomeController _controller;
        private static IEnumerable<string> _projectNames = new List<string> { "Hector", "Projector", "Ribbon Reflector", "A Picture of Nectar" };
        private static IEnumerable<string> _areaNames = new List<string> { "Larry", "Moe", "Shemp" };
        private const string _tfsUrl = "https://my.tfs.collection/word";
        private static Mock<ICardsClient> _cardsClientMock;
        private static Mock<IProjectsClient> _projectsClientMock;

        [Test]
        public void loads_the_signin_page()
        {
            Arrange();
            var result = _controller.Signin() as ViewResult;
            var model = result.Model as SignInViewModel;
            Assert.AreEqual("Sign in", model.Title);
        }

        [TestFixture]
        public class when_signing_in
        {

            [Test]
            public async Task sets_the_values_in_the_session()
            {
                Arrange();
                ArrangeSession();
                var model = new SignInViewModel { Url = _tfsUrl, Username = "dude@bro.com", Password = "12345", RememberMe = true };
                await _controller.Signin(model);
                Assert.AreEqual(_tfsUrl, _sessionProvider.Session.Url);
                Assert.AreEqual("dude@bro.com", _sessionProvider.Session.Username);
                Assert.AreEqual("12345", _sessionProvider.Session.Password);
                Assert.AreEqual(true, _sessionProvider.Session.RememberMe);
            }

            [Test]
            public async Task gets_a_list_of_project_names_and_fields_and_redirects_to_projects()
            {
                Arrange();
                ArrangeSession();
                ArrangeProjectsClient();
                var result = await _controller.Signin(new SignInViewModel()) as RedirectToRouteResult;
                CollectionAssert.AreEqual(_projectNames, _sessionProvider.Session.Projects);
                CollectionAssert.AreEqual(new Dictionary<string, string>(), _sessionProvider.Session.ProjectPriorityFieldNames);
                Assert.AreEqual("projects", result.RouteValues["action"]);
            }

            [TestFixture]
            public class given_an_error_when_getting_projects
            {
                [Test]
                public async Task shows_an_error_on_the_signin_page()
                {
                    Arrange();
                    ArrangeSession();
                    ArrangeProjectsClientToThrow();
                    var result = await _controller.Signin(new SignInViewModel()) as ViewResult;
                    var model = result.Model as SignInViewModel;
                    Assert.IsTrue(model.HasError);
                    Assert.AreEqual("Let's try that again.", model.ErrorMessage);
                }
            }

            [TestFixture]
            public class given_invalid_input
            {

                [TestCase]
                public async Task returns_to_the_page()
                {
                    Arrange();
                    _controller.ViewData.ModelState.AddModelError(string.Empty, string.Empty);
                    var result = await _controller.Signin(new SignInViewModel()) as ViewResult;
                    Assert.IsInstanceOf(typeof(SignInViewModel), result.Model);
                }
            }

        }

        [TestFixture]
        public class when_signing_out
        {
            [Test]
            public void it_ends_the_session_and_redirects_to_index()
            {
                Arrange();
                ArrangeSession();
                var result = _controller.Signout() as RedirectToRouteResult;
                Assert.IsNull(_sessionProvider.Session);
                Assert.AreEqual("index", result.RouteValues["action"]);
            }
        }

        [TestFixture]
        public class given_no_session
        {
            [Test]
            public void the_index_action_redirects_to_signin()
            {
                Arrange();
                var result = _controller.Index() as RedirectToRouteResult;
                Assert.AreEqual("signin", result.RouteValues["action"]);
            }

            [Test]
            public void the_projects_action_redirects_to_signin()
            {
                Arrange();
                var result = _controller.Projects() as RedirectToRouteResult;
                Assert.AreEqual("signin", result.RouteValues["action"]);
            }

            [Test]
            public void the_map_action_redirects_to_signin()
            {
                Arrange();
                var result = _controller.Map("wha") as RedirectToRouteResult;
                Assert.AreEqual("signin", result.RouteValues["action"]);
            }
        }

        [TestFixture]
        public class given_a_valid_session
        {

            [Test]
            public void redirects_to_projects()
            {
                Arrange();
                ArrangeSession();
                var result = _controller.Index() as RedirectToRouteResult;
                Assert.AreEqual("projects", result.RouteValues["action"]);
            }

            [Test]
            public void projects_has_a_list_of_project_names_and_a_title()
            {
                Arrange();
                ArrangeSession();
                var result = _controller.Projects() as ViewResult;
                var model = result.Model as ProjectsViewModel;
                Assert.AreEqual(_projectNames, model.Projects);
                Assert.AreEqual("Pick your poison", model.Title);
            }

            [TestFixture]
            public class given_a_blank_project_name
            {
                [Test]
                public async Task it_returns_the_view_as_is()
                {
                    Arrange();
                    ArrangeSession();
                    ArrangeProjectsClient();
                    var modelIn = new ProjectsViewModel();
                    var result = await _controller.Projects(modelIn) as ViewResult;
                    var modelOut = result.Model as ProjectsViewModel;
                    CollectionAssert.AreEqual(_projectNames, modelOut.Projects);
                }
            }

            [TestFixture]
            public class given_a_blank_area_name
            {
                [Test]
                public async Task it_loads_the_areas()
                {
                    Arrange();
                    ArrangeSession();
                    ArrangeProjectsClient();
                    var modelIn = new ProjectsViewModel { SelectedProject = "Projector" };
                    var result = await _controller.Projects(modelIn) as ViewResult;
                    var modelOut = result.Model as ProjectsViewModel;
                    CollectionAssert.AreEqual(_projectNames, modelOut.Projects);
                    CollectionAssert.AreEqual(_areaNames, modelOut.Areas);
                }
            }

            [TestFixture]
            public class given_a_project_and_area
            {
                [Test]
                public async Task it_redirects_to_the_map_page()
                {
                    Arrange();
                    ArrangeSession();
                    var result = await _controller.Projects(new ProjectsViewModel
                    {
                        SelectedProject = "Projector",
                        SelectedArea = "DieArea"
                    }) as RedirectResult;
                    Assert.AreEqual("map/projector/diearea", result.Url);
                }
            }

            [TestFixture]
            public class when_loading_the_map
            {

                [Test]
                public void has_a_route_with_the_project_name()
                {
                    CollectionAssert.Contains(_controller.RoutesOf("Map"),
                        "map/{projectName}");
                }

                [Test]
                public void has_a_route_with_the_project_name_and_area_name()
                {
                    CollectionAssert.Contains(_controller.RoutesOf("Map"),
                        "map/{projectName}/{areaName}");
                }

                [Test]
                public void it_sets_the_view_model_with_the_project_name_and_base_url()
                {
                    Arrange();
                    ArrangeSession();
                    var result = _controller.Map("OhYeah") as ViewResult;
                    var model = result.Model as MapViewModel;
                    Assert.AreEqual("OhYeah", model.Title);
                    Assert.AreEqual("OhYeah", model.ProjectName);
                    Assert.AreEqual(_tfsUrl, model.BaseUrl);
                }


                [TestFixture]
                public class when_the_user_specifies_an_area
                {
                    [Test]
                    public void passes_the_area_arg()
                    {
                        Arrange();
                        ArrangeSession();
                        var result = _controller.Map("OhYeah", areaName: "coolAid") as ViewResult;
                        var model = result.Model as MapViewModel;
                        Assert.AreEqual("coolAid", model.AreaName);
                    }
                }

                [TestFixture]
                public class when_the_user_specifies_a_depth
                {
                    [Test]
                    public void passes_the_depth_arg()
                    {
                        Arrange();
                        ArrangeSession();
                        var result = _controller.Map("OhYeah", depth: 5) as ViewResult;
                        var model = result.Model as MapViewModel;
                        Assert.AreEqual(5, model.Depth);
                    }
                }

                [TestFixture]
                public class given_a_blank_project_name
                {
                    [Test]
                    public void it_returns_the_projects_view()
                    {
                        Arrange();
                        ArrangeSession();
                        var result = _controller.Map(string.Empty) as RedirectToRouteResult;
                        Assert.AreEqual("projects", result.RouteValues["action"]);
                    }
                }
            }
        }

        public static void Arrange()
        {
            _sessionProvider = new FakeSessionProvider();
            _projectsClientMock = new Mock<IProjectsClient>();
            _controller = new HomeController(_sessionProvider, _projectsClientMock.Object);
        }

        private static void ArrangeSession()
        {
            _sessionProvider.Session = new SkeletonSession
            {
                Url = _tfsUrl,
                Projects = _projectNames
            };
        }

        private static void ArrangeProjectsClient()
        {
            _projectsClientMock.Setup(c => c.GetProjectNames())
                .Returns(Task.FromResult(_projectNames));
            _projectsClientMock.Setup(c => c.GetAreaNames("Projector"))
                .Returns(Task.FromResult(_areaNames));
            _projectsClientMock.Setup(c => c.GetProjectPriorityFieldNames(_projectNames))
                .Returns(Task.FromResult(new Dictionary<string, string>()));
        }

        private static void ArrangeProjectsClientToThrow()
        {
            _projectsClientMock.Setup(c => c.GetProjectNames()).Throws(new Exception("wtf"));
        }
    }
}