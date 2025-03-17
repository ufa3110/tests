using PaySolutionsTest.Infrastructure.SimilarRest;
using PaySolutionsTest.Integrations.PS;
using PaySolutionsTest.Integrations.PS.Querys;
using System;
using System.Collections;

namespace PaySolutionsTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Case("4111111111111111");
            Case("4627100101654724");
            Case("4486441729154030");
            Case("4024007123874108");
            ;
        }

        private static void Case(string pan)
        {
            var result = TestAssignments.GetTestAssignmentsQueryAsync(pan).ConfigureAwait(false).GetAwaiter().GetResult();
            Console.WriteLine(result.Status);
        }
    }
}
