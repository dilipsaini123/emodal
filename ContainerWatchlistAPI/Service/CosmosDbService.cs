using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContainerWatchlistAPI.Model; // Make sure to import the model containing UserData

namespace ContainerWatchlistAPI.Service
{
    public class CosmosDbService
    {
        private readonly Container _container;
        private readonly CosmosClient _cosmosClient;

        public CosmosDbService(IConfiguration configuration)
        {
            var endpointUri = configuration["CosmosDb:EndpointUri"];
            var primaryKey = configuration["CosmosDb:PrimaryKey"];
            var databaseName = configuration["CosmosDb:DatabaseName"];
            var containerName = configuration["CosmosDb:ContainerName"];
            _cosmosClient = new CosmosClient(endpointUri, primaryKey);
            _container = _cosmosClient.GetContainer(databaseName, containerName);
        }


        public async Task<IEnumerable<ContainerData>> GetContainersAsync()
        {
            var query = "SELECT * FROM c";
            var iterator = _container.GetItemQueryIterator<ContainerData>(query);
            var results = new List<ContainerData>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        // Fetch container data by containerId
        public async Task<ContainerData> GetContainerByIdAsync(string containerId)
        {
            var query = $"SELECT * FROM c WHERE c.ContainerId = '{containerId}'";
            var iterator = _container.GetItemQueryIterator<ContainerData>(query);

            if (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                return response.FirstOrDefault(); 
            }
            return null;
        }






    }

    public class ContainerData
    {
        public string id { get; set; }
        public string ContainerId { get; set; }
        public string TradeType { get; set; }
        public string Status { get; set; }
        public string Holds { get; set; }
        public string PregateTicket { get; set; }
        public string EModalPregate { get; set; }
        public string GateStatus { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string CurrentLocation { get; set; }
        public string line { get; set; }
        public string VesselName { get; set; }
        public string VesselCode { get; set; }
        public string Voyage { get; set; }
        public string SizeType { get; set; }
        public int AdditionalFees { get; set; }
        public int DemurrageFees { get; set; }
        public DateOnly Date { get; set; }
    }



}
