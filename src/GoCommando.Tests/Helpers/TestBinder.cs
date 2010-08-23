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
    }
}