using NUnit.Framework;

namespace GoCommando.Tests
{
    public abstract class FixtureBase
    {
        [SetUp]
        public void SetUp()
        {
            DoSetUp();
        }

        protected virtual void DoSetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
            DoTearDown();
        }

        protected virtual void DoTearDown()
        {
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            DoTestFixtureSetUp();
        }

        protected virtual void DoTestFixtureSetUp()
        {
        }
    }
}