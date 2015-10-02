using System.Collections.Generic;
using System.Linq;
using GoCommando.Attributes;
using GoCommando.Helpers;
using GoCommando.Parameters;
using NUnit.Framework;

namespace GoCommando.Tests.Helpers
{
    [TestFixture]
    public class TestBinder : FixtureBase
    {
        Binder binder;

        protected override void DoSetUp()
        {
            binder = new Binder();
        }

        [Test]
        public void UsesDefaultIfSpecified()
        {
            var target = new HasDefaultParameters();

            binder.Bind(target, ListWith());

            Assert.AreEqual("hello!", target.Something1);
            Assert.IsNull(target.Something2);
        }

        class HasDefaultParameters
        {
            [NamedArgument("something1", "s1", Default = "hello!")]
            public string Something1 { get; set; }

            [NamedArgument("something2", "s2")]
            public string Something2 { get; set; }
        }

        [Test]
        public void GeneratesReportOfUnboundParameters()
        {
            var target = new HasProperties();

            var report = binder.Bind(target, ListWith());

            Assert.AreEqual(4, report.PropertiesNotBound.Count);
            Assert.AreEqual(0, report.PropertiesBound.Count);
        }

        [Test]
        public void GeneratesReportOfBoundParameters()
        {
            var target = new HasProperties();

            var report = binder.Bind(target, ListWith(PositionalParameter(1, "1"),
                                                      PositionalParameter(2, "2"),
                                                      NamedParameter("bim1", "3"),
                                                      NamedParameter("bim2", "4")));

            Assert.AreEqual(0, report.PropertiesNotBound.Count);
            Assert.AreEqual(4, report.PropertiesBound.Count);
        }

        [Test]
        public void CanBindSimpleIntegerParameter()
        {
            var target = new HasPrimitiveTypes();
            
            binder.Bind(target, ListWith(NamedParameter("int", "23")));

            Assert.AreEqual(23, target.IntValue);
        }

        [Test]
        public void CanBindSimpleStringParameter()
        {
            var target = new HasPrimitiveTypes();

            binder.Bind(target, ListWith(NamedParameter("string", "yo!")));

            Assert.AreEqual("yo!", target.StringValue);
        }

        [Test]
        public void CanBindSimpleDoubleParameter()
        {
            var target = new HasPrimitiveTypes();

            binder.Bind(target, ListWith(NamedParameter("double", "23,5")));

            Assert.AreEqual(23.5, target.DoubleValue);
        }

        class HasProperties
        {
            [PositionalArgument]
            public string Arg1 { get; set; }
            
            [PositionalArgument]
            public string Arg2 { get; set; }

            [NamedArgument("bim1", "b1")]
            public string Arg3 { get; set; }

            [NamedArgument("bim2", "b2")]
            public string Arg4 { get; set; }
        }

        class HasPrimitiveTypes
        {
            [NamedArgument("int", "i")]
            public int IntValue { get; set; }

            [NamedArgument("double", "d")]
            public double DoubleValue { get; set; }

            [NamedArgument("string", "s")]
            public string StringValue { get; set; }
        }

        IEnumerable<CommandLineParameter> ListWith(params CommandLineParameter[] parameters)
        {
            return parameters.ToList();
        }

        CommandLineParameter NamedParameter(string name, string value)
        {
            return new NamedCommandLineParameter(name, value);
        }

        CommandLineParameter PositionalParameter(int index, string value)
        {
            return new PositionalCommandLineParameter(index, value);
        }
    }
}