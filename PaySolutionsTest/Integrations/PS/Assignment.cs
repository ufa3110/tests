using System.Reflection;

namespace PaySolutionsTest.Integrations.PS
{
    internal class Assignment
    {
        public required CardInfo CardInfo { get; set; }

        public Dictionary<string, object> GetAsDictionary()
        {
            return new Dictionary<string, object>()
            {
                { "CardInfo", new Dictionary<string, string>() { { "Pan", CardInfo.Pan } } }
            };
        }
    }
}
