using System.Collections.Generic;
using System.Linq;
using GoCommando.Attributes;
using GoCommando.Helpers;
using NUnit.Framework;
namespace GoCommando.Tests.Helpers
{
    [TestFixture]
    public class TestHelper : FixtureBase
    {
        Helper helper;

        protected override void DoSetUp()
        {
            helper = new Helper();
        }

        [Test]
        public void CanRepresentParametersAndDescriptionsAsExpected()
        {
            var parameters = helper.GetParameters(new SomeClass());

            AssertPositionalParameter(parameters[0], "hello there!", 1, "FirstPositional", "ex1", "ex2");
            AssertPositionalParameter(parameters[1], "hello there!", 2, "SecondPositional", "yet another lengthy example");
            AssertNamedParameter(parameters[2], "hello again! there!", "AnotherString", "name", "nm");
        }

        void AssertPositionalParameter(Parameter parameter, string expectedDescription, int expectedPosition, string expectedPropertyName, params string[] expectedExamples)
        {
            AssertParameter(expectedDescription, parameter, expectedPropertyName);
            Assert.AreEqual(expectedPosition, parameter.Position);
            Assert.IsTrue(parameter.ArgumentAttribute is PositionalArgumentAttribute, "Expected PositionalArgumentAttribute, was {0}", parameter.ArgumentAttribute.GetType().Name);
            AssertExamples(parameter, expectedExamples);
        }

        void AssertNamedParameter(Parameter parameter, string expectedDescription, string expectedPropertyName, string expectedArgumentName, string expectedArgumentShorthand, params string[] expectedExamples)
        {
            AssertParameter(expectedDescription, parameter, expectedPropertyName);
            Assert.IsTrue(parameter.ArgumentAttribute is NamedArgumentAttribute, "Expected NamedArgumentAttribute, was {0}", parameter.ArgumentAttribute.GetType().Name);
            Assert.AreEqual(expectedArgumentName, ((NamedArgumentAttribute)parameter.ArgumentAttribute).Name);
            Assert.AreEqual(expectedArgumentShorthand, ((NamedArgumentAttribute) parameter.ArgumentAttribute).ShortHand);
            AssertExamples(parameter, expectedExamples);
        }

        void AssertExamples(Parameter parameter, IEnumerable<string> examples)
        {
            Assert.IsTrue(parameter.Examples.OrderBy(e => e.Text).Select(e => e.Text).SequenceEqual(examples.OrderBy(s => s)));
        }

        void AssertParameter(string expectedDescription, Parameter para, string expectedPropertyName)
        {
            Assert.AreEqual(expectedDescription, para.Description);
            Assert.AreEqual(expectedPropertyName, para.PropertyInfo.Name);
        }

        class SomeClass
        {
            public string ShouldNotBeIncluded { get; set; }

            [PositionalArgument]
            [Attributes.Description("hello there!")]
            [Example("ex1")]
            [Example("ex2")]
            public string FirstPositional { get; set; }

            [PositionalArgument]
            [Attributes.Description("hello there!")]
            [Example("yet another lengthy example")]
            public string SecondPositional { get; set; }

            [NamedArgument("name", "nm")]
            [Attributes.Description("hello again! there!")]
            public string AnotherString { get; set; }
        }
    }
}