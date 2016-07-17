using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace ScrapeService
{
    class SeachPush
    {
        string searchServiceName = "scrapeme";
        string apiKey = "00C5B3884A280C3821E3875E16680FA7";
        string index = "housing";

        public SeachPush (List <HousingObject> Hoes)
        {
            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            SearchIndexClient indexClient = serviceClient.Indexes.GetClient(index);
               
                try
                {
                    var batch = IndexBatch.Upload(Hoes);
                    indexClient.Documents.Index(batch);
                }
                catch (IndexBatchException e)
                {
                    // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                    // the batch. Depending on your application, you can take compensating actions like delaying and
                    // retrying. For this simple demo, we just log the failed document keys and continue.
                    Console.WriteLine(
                        "Failed to index some of the documents: {0}",
                        String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
                }
        }
    }
}
