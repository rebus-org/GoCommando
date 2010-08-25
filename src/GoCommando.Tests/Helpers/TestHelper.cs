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

            var firstParameter = parameters[0];
            Assert.AreEqual("hello there!", firstParameter.Description);
            Assert.AreEqual(1, firstParameter.Position);

            var secondParameter = parameters[1];
            Assert.AreEqual("hello there!", secondParameter.Description);
            Assert.AreEqual(2, secondParameter.Position);

            var thirdParameter = parameters[2];
            Assert.AreEqual("hello again! there!", thirdParameter.Description);
            Assert.AreEqual("name", thirdParameter.Name);
            Assert.AreEqual("nm", thirdParameter.Shorthand);
        }

        class SomeClass
        {
            public string ShouldNotBeIncluded { get; set; }

            [PositionalArgument]
            [Attributes.Description("hello there!")]
            public string FirstPositional { get; set; }

            [PositionalArgument]
            [Attributes.Description("hello there!")]
            public string SecondPositional { get; set; }

            [NamedArgument("name", "nm")]
            [Attributes.Description("hello again! there!")]
            public string AnotherString { get; set; }
        }
    }
}