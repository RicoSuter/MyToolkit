using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Composition;

namespace MyToolkit.Tests.WinRT.Composition
{
    [TestClass]
    public class CompositionContextTests
    {
        #region Mocks

        [Export(typeof(ExportClass), Name = "1")]
        public class ExportClass
        {

        }

        [Export(typeof(ExportClass))]
        public class ExportClass1 : ExportClass
        {

        }

        [Export(typeof(ExportClass))]
        public class ExportClass2 : ExportClass
        {

        }

        [Export(typeof(ExportClass))]
        public class ExportClassWithCtor : ExportClass
        {
            public ExportClass1 Import { get; private set; }

            public ExportClassWithCtor(ExportClass1 import)
            {
                Import = import;
            }
        }

        [Export(typeof(ExportClass))]
        public class ExportClassWithCtorAndSelfImport : ExportClass
        {
            public ExportClassWithCtorAndSelfImport Import { get; private set; }

            public ExportClassWithCtorAndSelfImport(ExportClassWithCtorAndSelfImport import)
            {
                Import = import;
            }
        }

        public class ImportClass
        {
            [Import(typeof(ExportClass))]
            public ExportClass Object { get; set; }
        }

        public class ImportManyClass
        {
            [ImportMany(typeof(ExportClass))]
            public IEnumerable<ExportClass> Objects { get; set; }
        }

        public class ImportClass2
        {
            [Import(typeof(ExportClass), Name = "1")]
            public ExportClass Object1 { get; set; }

            [Import(typeof(ExportClass), Name = "2")]
            public ExportClass Object2 { get; set; }
        }

        #endregion

        [TestMethod]
        public void When_adding_by_type_then_object_should_automatically_be_instantiated()
        {
            var ctx = new CompositionContext();
            ctx.AddPart<ExportClass, ExportClass>();

            var obj = new ImportClass();
            ctx.SatisfyImports(obj);

            Assert.IsTrue(obj.Object != null);
        }

        [TestMethod]
        public void When_adding_object_then_it_should_be_loadable()
        {
            var ctx = new CompositionContext();
            var part = new ExportClass();
            ctx.AddPart<ExportClass, ExportClass>(part);

            var obj = new ImportClass();
            ctx.SatisfyImports(obj);

            Assert.AreEqual(part, obj.Object);
        }

        [TestMethod]
        public void When_adding_object_and_name_then_it_should_be_loadable()
        {
            var ctx = new CompositionContext();
            var part1 = new ExportClass();
            var part2 = new ExportClass();

            ctx.AddPart<ExportClass, ExportClass>(part1, "1");
            ctx.AddPart<ExportClass, ExportClass>(part2, "2");

            var obj = new ImportClass2();
            ctx.SatisfyImports(obj);

            Assert.AreEqual(part1, obj.Object1);
            Assert.AreEqual(part2, obj.Object2);
        }

        [TestMethod]
        public void When_loading_objects_from_assembly_then_they_should_be_accessible()
        {
            var ctx = new CompositionContext();
            ctx.AddPartsFromAssembly(GetType().GetTypeInfo().Assembly);

            var obj = new ImportClass2();
            ctx.SatisfyImports(obj);

            Assert.IsTrue(obj.Object1 != null);
        }

        // TODO: Fix this test
        //[TestMethod]
        //public void When_importing_many_objects_then_a_list_should_be_accessible()
        //{
        //    var ctx = new CompositionContext();

        //    ctx.AddPart<ExportClass, ExportClass1>();
        //    ctx.AddPart<ExportClass, ExportClass2>();

        //    var obj = new ImportManyClass();
        //    ctx.SatisfyImports(obj);

        //    Assert.AreEqual(2, obj.Objects.Count());
        //}

        [TestMethod]
        public void When_instantiating_class_with_ctor_arguments_then_the_needed_objects_get_created()
        {
            var ctx = new CompositionContext();

            ctx.AddPart<ExportClass1, ExportClass1>();
            ctx.AddPart<ExportClassWithCtor, ExportClassWithCtor>();

            var part = ctx.GetPart<ExportClassWithCtor>();

            Assert.IsNotNull(part.Import);
            Assert.AreEqual(part.Import, ctx.GetPart<ExportClass1>());
        }

        //[TestMethod]
        //public void When_importing_itself_via_ctor_then_exeption_should_be_thrown()
        //{
        //    //// Arrange
        //    var ctx = new CompositionContext();
        //    ctx.AddPart<ExportClassWithCtorAndSelfImport, ExportClassWithCtorAndSelfImport>();

        //    //// Act
        //    try
        //    {
        //        var part = ctx.GetPart<ExportClassWithCtorAndSelfImport>();
        //    }
        //    catch (Exception exception)
        //    {
        //        //// Assert
        //        return;
        //    }
        //    Assert.Fail();
        //}
    }
}
