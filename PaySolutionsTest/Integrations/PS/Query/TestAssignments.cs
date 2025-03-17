using Newtonsoft.Json;
using PaySolutionsTest.Infrastructure.SimilarRest;
using PaySolutionsTest.Integrations.PS.Response;

namespace PaySolutionsTest.Integrations.PS.Querys
{
    internal static class TestAssignments
    {
        //тут вроде как можно заюзать медиатр, популярно, но имхо не даёт плюсов к архитектуре.
        public async static Task<TestAssignmentsResponse> GetTestAssignmentsQueryAsync(string pan, CancellationToken ct = default)
        {
            var request = new Assignment()
            { 
                CardInfo  = new CardInfo()
                            { 
                                Pan = pan
                            }
            };

            var response = await SimilarRestHelper.SendQueryAsync(request.GetAsDictionary(), HttpMethod.Post, "api/testassignments/pan",  ct);
            //ну тут nre может быть, в идеале бы предусмотреть.
            try
            {
                return JsonConvert.DeserializeObject<TestAssignmentsResponse>(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
