using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyToolkit.Build;

namespace MyToolkit.Tests.Wpf45.Utilities
{
    [TestClass]
    public class ProjectDependencyResolverTests
    {
        [TestMethod]
        public void When_loading_project_references_then_correct_paths_must_be_returned()
        {
            //// Arrange
            var path = "../../../MyToolkit.Extended.Wp8/MyToolkit.Extended.Wp8.csproj";

            //// Act
            var projects = ProjectDependencyResolver.GetProjectReferences(path);

            //// Assert
            Assert.AreEqual(1, projects.Count());
        }

        [TestMethod]
        public void When_loading_references_then_nuget_references_must_be_correct()
        {
            //// Arrange
            var path = "../../../MyToolkit.Extended.Wp8/MyToolkit.Extended.Wp8.csproj";

            //// Act
            var project = VisualStudioProject.FromFilePath(path);

            //// Assert
            Assert.AreEqual(1, project.NuGetReferences.Count());
            Assert.IsTrue(project.NuGetReferences.Any(r => r.Name == "WPtoolkit"));
        }

        [TestMethod]
        public void When_loading_all_projects_then_correct_list_must_be_returned()
        {
            //// Arrange
            var path = "../../../";

            //// Act
            var projects = VisualStudioProject.LoadAllFromDirectory(path);

            //// Assert
            Assert.IsTrue(projects.Count() > 0);
        }


        [TestMethod]
        public void When_generating_build_order_then_project_with_no_references_should_be_first()
        {
            //// Arrange
            var paths = new string[]
            {
                "../../../MyToolkit.Extended.Wp8/MyToolkit.Extended.Wp8.csproj", 
                "../../../MyToolkit/MyToolkit.csproj", 
            };

            //// Act
            var buildOrder = ProjectDependencyResolver.GetBuildOrder(paths).ToArray();

            //// Assert
            Assert.AreEqual(2, buildOrder.Length);
            Assert.IsTrue(buildOrder[0].Contains("MyToolkit.csproj"));
            Assert.IsTrue(buildOrder[1].Contains("MyToolkit.Extended.Wp8.csproj"));
        }
    }
}
