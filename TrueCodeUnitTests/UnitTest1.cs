using TrueCodeTestSolution.Controllers;

namespace TrueCodeUnitTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        private static IEnumerable<TestCaseData> GetLastNByUserIdTestData
        {
            get
            {
                yield return new TestCaseData(1, "author_1_ID");
                yield return new TestCaseData(100, "author_1_ID");
                yield return new TestCaseData(-1, "author_1_ID");
                yield return new TestCaseData(0, "author_1_ID");

                yield return new TestCaseData(1, "author_2_ID");
                yield return new TestCaseData(100, "author_2_ID");
                yield return new TestCaseData(-1, "author_2_ID");
                yield return new TestCaseData(0, "author_2_ID");
            }
        }


        [Test, TestCaseSource(nameof(GetLastNByUserIdTestData))]
        public void GetLastNByUserIdTests(int number, string userId)
        {
            PublicationController controller = new();

            Assert.DoesNotThrow(() => controller.GetLastNByUserId(number, userId));
        }

        private static IEnumerable<TestCaseData> GetLastNTestData
        {
            get
            {
                yield return new TestCaseData(1, 1);
                yield return new TestCaseData(1, 100);
                yield return new TestCaseData(100, 1);
                yield return new TestCaseData(100, 100);
                yield return new TestCaseData(100, -1);
                yield return new TestCaseData(-1, -1);
                yield return new TestCaseData(0, -1);
                yield return new TestCaseData(-1, 0);
                yield return new TestCaseData(0, 0);
            }
        }

        [Test, TestCaseSource(nameof(GetLastNTestData))]
        public void GetLastNTests(int number, int userCount)
        {
            PublicationController controller = new();

            Assert.DoesNotThrow(() => controller.GetLastN(number, userCount));
        }

    }
}