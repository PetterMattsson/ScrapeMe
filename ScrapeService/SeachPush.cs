using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace ScrapeService
{
    class SeachService
    {
        string searchServiceName = "scrapeme";
        string apiKey = "00C5B3884A280C3821E3875E16680FA7";
        string index = "housing";

        public SeachService ()
        {
            
               
                
        }

        public void ListUpload (List <HousingObject> Hoes)
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
        public void DeleteIndex()
        {
            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            SearchIndexClient indexClient = serviceClient.Indexes.GetClient(index);

            if (serviceClient.Indexes.Exists(index))
            {
                serviceClient.Indexes.Delete(index);
            }

        }

        public void BuildIndex()
        {
            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));

            var definition = new Index()
            {
                Name = "housing",
                Fields = new[]
                {
                    new Field("HousingId", DataType.String)                     { IsKey = true },
                    new Field("Title", DataType.String)                         { IsSearchable = true, IsFilterable = true },
                    new Field("Description", DataType.String)                   { IsSearchable = true, IsFilterable = true },
                    new Field("Size", DataType.Double)                          { IsFilterable = true, IsSortable = true },
                    new Field("Category", DataType.String)                      { IsSearchable = true, IsFilterable = true, IsSortable = true, IsFacetable = true },
                    new Field("Updated", DataType.DateTimeOffset)               { IsFilterable = true, IsSortable = true, IsFacetable = true },
                    new Field("Rooms", DataType.Int32)                          { IsFilterable = true, IsSortable = true, IsFacetable = true },
                    new Field("Fee", DataType.Int32)                            { IsFilterable = true, IsSortable = true, IsFacetable = true },
                    new Field("Area", DataType.String)                          { IsSearchable = true, IsFilterable = true },
                    new Field("City", DataType.String)                          { IsSearchable = true, IsFilterable = true },
                    new Field("Municipality", DataType.String)                  { IsSearchable = true, IsFilterable = true },
                    new Field("County", DataType.String)                        { IsSearchable = true, IsFilterable = true },
                    new Field("Address", DataType.String)                       { IsSearchable = true, IsFilterable = true },
                    new Field("SourceUrl", DataType.String)                     { IsSearchable = false, IsFilterable = true },
                    new Field("SourceName", DataType.String)                    { IsSearchable = false, IsFilterable = true }
                }
            };

            //serviceClient.Indexes.Create();
        }
    }
}
