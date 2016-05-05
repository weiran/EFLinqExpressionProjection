using System;
using System.Linq.Expressions;
using System.Linq;
using LinqExpressionProjection.Test.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable All

namespace LinqExpressionProjection.Test
{
    [TestClass]
    public class LinqExpressionsProjection_TestFixture
    {
        private static readonly Expression<Func<Project, double>> ProjectAverageEffectiveAreaSelectorStatic =
            proj => proj.Subprojects.Where(sp => sp.Area < 1000).Average(sp => sp.Area);

        private readonly Expression<Func<Project, double>> _projectAverageEffectiveAreaSelector =
            proj => proj.Subprojects.Where(sp => sp.Area < 1000).Average(sp => sp.Area);

        public static Expression<Func<Project, double>> GetProjectAverageEffectiveAreaSelectorStatic()
        {
            return proj => proj.Subprojects.Where(sp => sp.Area < 1000).Average(sp => sp.Area);
        }

        public Expression<Func<Project, double>> GetProjectAverageEffectiveAreaSelector()
        {
            return proj => proj.Subprojects.Where(sp => sp.Area < 1000).Average(sp => sp.Area);
        }

        public Expression<Func<Project, double>> GetProjectAverageEffectiveAreaSelectorWithLogic(bool isOverThousandIncluded = false)
        {
            return isOverThousandIncluded
                       ? (Expression<Func<Project, double>>)((Project proj) => proj.Subprojects.Where(sp => sp.Area < 1000).Average(sp => sp.Area))
                       : (Project proj) => proj.Subprojects.Average(sp => sp.Area);
        }

        [ClassInitialize]
        public static void RunBeforeAnyTests(TestContext testContext)
        {
            ClearDb();
            PopulateDb();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ProjectingExpressionFailsOnNormalCases_Test()
        {
            Expression<Func<Project, double>> localSelector =
                proj => proj.Subprojects.Where(sp => sp.Area < 1000).Average(sp => sp.Area);
            using (var ctx = new ProjectsDbContext())
            {
                var v = (from p in ctx.Projects
                         select new
                                {
                                    Project = p,
                                    AEA = localSelector
                                }).ToArray();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ProjectingExpressionFailsWithNoCallToAsExpressionProjectable_Test()
        {
            Expression<Func<Project, double>> localSelector =
                proj => proj.Subprojects.Where(sp => sp.Area < 1000).Average(sp => sp.Area);
            using (var ctx = new ProjectsDbContext())
            {
                var projects = (from p in ctx.Projects
                                select new
                                       {
                                           Project = p,
                                           AEA = localSelector.Project(p)
                                       }).ToArray();
                Assert.AreEqual(150, projects[0].AEA);
                Assert.AreEqual(400, projects[1].AEA);
            }
        }

        [TestMethod]
        public void ProjectingExpressionByLocalVariable_Test()
        {
            Expression<Func<Project, double>> localSelector =
                proj => proj.Subprojects.Where(sp => sp.Area < 1000).Average(sp => sp.Area);
            using (var ctx = new ProjectsDbContext())
            {
                var projects = (from p in ctx.Projects.AsExpressionProjectable()
                                select new
                                       {
                                           Project = p,
                                           AEA = localSelector.Project(p)
                                       }).ToArray();
                Assert.AreEqual(150, projects[0].AEA);
                Assert.AreEqual(400, projects[1].AEA);
            }
        }

        [TestMethod]
        public void ProjectingExpressionByStaticField_Test()
        {
            using (var ctx = new ProjectsDbContext())
            {
                var projects = (from p in ctx.Projects.AsExpressionProjectable()
                                select new
                                       {
                                           Project = p,
                                           AEA = ProjectAverageEffectiveAreaSelectorStatic.Project(p)
                                       }).ToArray();
                Assert.AreEqual(150, projects[0].AEA);
                Assert.AreEqual(400, projects[1].AEA);
            }
        }


        [TestMethod]
        public void ProjectingExpressionByNonStaticField_Test()
        {
            using (var ctx = new ProjectsDbContext())
            {
                var projects = (from p in ctx.Projects.AsExpressionProjectable()
                                select new
                                       {
                                           Project = p,
                                           AEA = _projectAverageEffectiveAreaSelector.Project(p)
                                       }).ToArray();
                Assert.AreEqual(150, projects[0].AEA);
                Assert.AreEqual(400, projects[1].AEA);
            }
        }

        [TestMethod]
        public void ProjectingExpressionByStaticMethod_Test()
        {
            using (var ctx = new ProjectsDbContext())
            {
                var projects = (from p in ctx.Projects.AsExpressionProjectable()
                                select new
                                       {
                                           Project = p,
                                           AEA = GetProjectAverageEffectiveAreaSelectorStatic().Project(p)
                                       }).ToArray();
                Assert.AreEqual(150, projects[0].AEA);
                Assert.AreEqual(400, projects[1].AEA);
            }
        }

        [TestMethod]
        public void ProjectingExpressionByNonStaticMethod_Test()
        {
            using (var ctx = new ProjectsDbContext())
            {
                var projects = (from p in ctx.Projects.AsExpressionProjectable()
                                select new
                                       {
                                           Project = p,
                                           AEA = GetProjectAverageEffectiveAreaSelector().Project(p)
                                       }).ToArray();
                Assert.AreEqual(150, projects[0].AEA);
                Assert.AreEqual(400, projects[1].AEA);
            }
        }

        [TestMethod]
        public void ProjectingExpressionByNonStaticMethodWithLogic_Test()
        {
            using (var ctx = new ProjectsDbContext())
            {
                var projects = (from p in ctx.Projects.AsExpressionProjectable()
                                select new
                                       {
                                           Project = p,
                                           AEA = GetProjectAverageEffectiveAreaSelectorWithLogic(false).Project(p)
                                       }).ToArray();
                Assert.AreEqual(150, projects[0].AEA);
                Assert.AreEqual(3600, projects[1].AEA);
            }
            using (var ctx = new ProjectsDbContext())
            {
                var projects = (from p in ctx.Projects.AsExpressionProjectable()
                                select new
                                       {
                                           Project = p,
                                           AEA = GetProjectAverageEffectiveAreaSelectorWithLogic(true).Project(p)
                                       }).ToArray();
                Assert.AreEqual(150, projects[0].AEA);
                Assert.AreEqual(400, projects[1].AEA);
            }
        }

        [TestMethod]
        public void ValidateAndLog_ProjectsDbContext_Connection()
        {
            using (var ctx = new ProjectsDbContext())
            {
                Console.WriteLine(ctx.Database.Connection.ConnectionString);
            }
        }

        [TestMethod]
        public void Can_Project_StaticField_BasicExpression()
        {
            using (var ctx = new ProjectsDbContext())
            {
                var subprojects = ctx.Subprojects.AsExpressionProjectable().Select(
                    subproject => new
                                  {
                                      subproject,
                                      testResult = Subproject.StaticFieldOnType_BasicExpression.Project(subproject)
                                  }).ToArray();

                Assert.AreEqual("StaticFieldOnType_BasicExpression - Area: 100", subprojects.ElementAt(0).testResult);
                Assert.AreEqual("StaticFieldOnType_BasicExpression - Area: 450", subprojects.ElementAt(3).testResult);
            }
        }

        private static class TestExpressions
        {
            public static readonly Expression<Func<Subproject, string>> BasicMemberExpression = subProject => "Area: " + subProject.Area;

            public static readonly Expression<Func<Project, string>> MemberOfMemberExpression = project => "Subprojects Count: " + project.Subprojects.Count;
        }

        [TestMethod]
        public void Can_Project_BasicMemberExpression()
        {
            using (var ctx = new ProjectsDbContext())
            {
                var subprojects = ctx.Subprojects.AsExpressionProjectable().Select(
                    subproject => new
                                  {
                                      subproject,
                                      testResult = TestExpressions.BasicMemberExpression.Project(subproject)
                                  }).ToArray();

                Assert.AreEqual("Area: 100", subprojects.ElementAt(0).testResult);
                Assert.AreEqual("Area: 450", subprojects.ElementAt(3).testResult);
            }
        }

        [TestMethod]
        public void Can_Project_MemberOfMemberExpression()
        {
            using (var ctx = new ProjectsDbContext())
            {
                var subprojects = ctx.Subprojects.AsExpressionProjectable().Select(
                    subproject => new
                                  {
                                      subproject,
                                      testResult = TestExpressions.MemberOfMemberExpression.Project(subproject.Project)
                                  }).ToArray();

                Assert.AreEqual("Subprojects Count: 2", subprojects.ElementAt(0).testResult);
                Assert.AreEqual("Subprojects Count: 2", subprojects.ElementAt(1).testResult);
                Assert.AreEqual("Subprojects Count: 3", subprojects.ElementAt(2).testResult);
                Assert.AreEqual("Subprojects Count: 3", subprojects.ElementAt(3).testResult);
                Assert.AreEqual("Subprojects Count: 3", subprojects.ElementAt(4).testResult);
            }
        }

        [TestMethod]
        public void Can_Project_MemberExpressionOfMainLambdaParameter()
        {
            Expression<Func<User, string>> projectionExpression = user => user.Name + "-somepostfix";

            using (var ctx = new ProjectsDbContext())
            {
                var projects = ctx.Projects.AsExpressionProjectable().Select(
                    project => new
                               {
                                   testResult1 = projectionExpression.Project(project.CreatedBy),
                                   testResult2 = projectionExpression.Project(project.ModifiedBy)
                               }).ToArray();

                Assert.AreEqual("user1-somepostfix", projects.ElementAt(0).testResult1);
                Assert.AreEqual("user3-somepostfix", projects.ElementAt(0).testResult2);

                Assert.AreEqual("user2-somepostfix", projects.ElementAt(1).testResult1);
                Assert.AreEqual("user4-somepostfix", projects.ElementAt(1).testResult2);
            }
        }

        [TestMethod]
        public void Can_Project_InnerExpression()
        {
            Expression<Func<string, string>> projectionExpression = s => s + "-somepostfix";

            using (var ctx = new ProjectsDbContext())
            {
                var projects = ctx.Projects.AsExpressionProjectable().Select(
                    project => new
                               {
                                   testResult1 = projectionExpression.Project("hello world"),
                                   testResult2 = projectionExpression.Project((2 + 10).ToString())
                               }).ToArray();

                Assert.AreEqual("hello world-somepostfix", projects.ElementAt(0).testResult1);
                Assert.AreEqual("12-somepostfix", projects.ElementAt(0).testResult2);

                Assert.AreEqual("hello world-somepostfix", projects.ElementAt(1).testResult1);
                Assert.AreEqual("12-somepostfix", projects.ElementAt(1).testResult2);
            }
        }

        [TestMethod]
        public void Can_Project_ExpressionWith2Parameters()
        {
            Expression<Func<User, User, string>> projectionExpression = (user1, user2) => user1.Name + "::" + user2.Name;

            using (var ctx = new ProjectsDbContext())
            {
                var projects = ctx.Projects.AsExpressionProjectable().Select(
                    project => new
                    {
                        testResult = projectionExpression.Project(project.CreatedBy, project.ModifiedBy)
                    }).ToArray();

                Assert.AreEqual("user1::user3", projects.ElementAt(0).testResult);
                Assert.AreEqual("user2::user4", projects.ElementAt(1).testResult);
            }
        }

        [TestMethod]
        public void Can_Project_ExpressionWith3Parameters()
        {
            Expression<Func<User, User, int, string>> projectionExpression = (user1, user2, x1) => user1.Name + "::" + user2.Name + "-" + x1;

            using (var ctx = new ProjectsDbContext())
            {
                var projects = ctx.Projects.AsExpressionProjectable().Select(
                    project => new
                    {
                        testResult = projectionExpression.Project(project.CreatedBy, project.ModifiedBy, 3 + 5)
                    }).ToArray();

                Assert.AreEqual("user1::user3-8", projects.ElementAt(0).testResult);
                Assert.AreEqual("user2::user4-8", projects.ElementAt(1).testResult);
            }
        }

        [TestMethod]
        public void Can_Project_ExpressionWith4Parameters()
        {
            Expression<Func<User, User, int, string, string>> projectionExpression = (user1, user2, x1, x2) => user1.Name + "::" + user2.Name + "-" + x1 + "~" + x2;

            using (var ctx = new ProjectsDbContext())
            {
                var projects = ctx.Projects.AsExpressionProjectable().Select(
                    project => new
                    {
                        testResult = projectionExpression.Project(project.CreatedBy, project.ModifiedBy, 3 + 5, "test")
                    }).ToArray();

                Assert.AreEqual("user1::user3-8~test", projects.ElementAt(0).testResult);
                Assert.AreEqual("user2::user4-8~test", projects.ElementAt(1).testResult);
            }
        }

        [TestMethod]
        public void Can_Project_ExpressionWith5Parameters()
        {
            Expression<Func<User, User, int, string, int, string>> projectionExpression = (user1, user2, x1, x2, x3) => user1.Name + "::" + user2.Name + "-" + x1 + "~" + x2 + "_" + x3;

            using (var ctx = new ProjectsDbContext())
            {
                var projects = ctx.Projects.AsExpressionProjectable().Select(
                    project => new
                    {
                        testResult = projectionExpression.Project(project.CreatedBy, project.ModifiedBy, 3 + 5, "test", 10 * 3)
                    }).ToArray();

                Assert.AreEqual("user1::user3-8~test_30", projects.ElementAt(0).testResult);
                Assert.AreEqual("user2::user4-8~test_30", projects.ElementAt(1).testResult);
            }
        }

        private static void ClearDb()
        {
            using (var ctx = new ProjectsDbContext())
            {
                foreach (var subproject in ctx.Subprojects)
                {
                    ctx.Subprojects.Remove(subproject);
                }
                foreach (var project in ctx.Projects)
                {
                    ctx.Projects.Remove(project);
                }
                foreach (var user in ctx.Users)
                {
                    ctx.Users.Remove(user);
                }
                ctx.SaveChanges();
            }
        }

        private static void PopulateDb()
        {
            using (var ctx = new ProjectsDbContext())
            {
                User user1 = ctx.Users.Add(new User { Name = "user1" });
                User user2 = ctx.Users.Add(new User { Name = "user2" });
                User user3 = ctx.Users.Add(new User { Name = "user3" });
                User user4 = ctx.Users.Add(new User { Name = "user4" });

                ctx.SaveChanges();

                Project p1 = ctx.Projects.Add(new Project { CreatedBy = user1, ModifiedBy = user3 });
                Project p2 = ctx.Projects.Add(new Project { CreatedBy = user2, ModifiedBy = user4 });

                ctx.Subprojects.Add(new Subproject { Area = 100, Project = p1 });
                ctx.Subprojects.Add(new Subproject { Area = 200, Project = p1 });
                ctx.Subprojects.Add(new Subproject { Area = 350, Project = p2 });
                ctx.Subprojects.Add(new Subproject { Area = 450, Project = p2 });
                ctx.Subprojects.Add(new Subproject { Area = 10000, Project = p2 });

                ctx.SaveChanges();
            }
        }
    }
}
