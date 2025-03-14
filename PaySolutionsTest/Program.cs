using PaySolutionsTest.Integrations.PS.Querys;

namespace PaySolutionsTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //ну метод есть, только 404 почему-то начали вылетать)
            var result = TestAssignments.GetTestAssignmentsQueryAsync("4111111111111111").ConfigureAwait(false).GetAwaiter().GetResult();
            Console.WriteLine(result.Status);
        }
    }
}
