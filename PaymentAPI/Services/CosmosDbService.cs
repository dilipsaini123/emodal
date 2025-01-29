using Microsoft.Azure.Cosmos;  
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentAPI.Models;  

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

    public async Task<CosmosContainer> GetContainerByIdAsync(string containerId)
    {
        try
        {
            var query = "SELECT * FROM c WHERE c.ContainerId = @containerId";
            var queryDefinition = new QueryDefinition(query).WithParameter("@containerId", containerId);

            var iterator = _container.GetItemQueryIterator<CosmosContainer>(queryDefinition);

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                if (response.Any())
                {
                    return response.FirstOrDefault(); 
                }
            }

            Console.WriteLine($"No container found with ContainerId: {containerId}");
            return null; 
        }
        catch (CosmosException ex)
        {
            Console.WriteLine($"Cosmos DB query failed for ContainerId {containerId}: {ex.StatusCode}, {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error while fetching container {containerId}: {ex.Message}");
            return null;
        }
    }

    public async Task UpdateContainerInCosmosDb(string containerId)
    {
        try
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.ContainerId = @containerId").WithParameter("@containerId", containerId);
            var response = await _container.GetItemQueryIterator<CosmosContainer>(query).ReadNextAsync();

            var cosmosItem = response.FirstOrDefault();
            if (cosmosItem != null)
            {
                cosmosItem.Holds = "No";
                cosmosItem.AdditionalFees = 0;
                cosmosItem.DemurrageFees = 0;
                await _container.UpsertItemAsync(cosmosItem, new PartitionKey(containerId));
            }
            else
            {
                Console.WriteLine($"Container with ID {containerId} not found in Cosmos DB.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cosmos DB update failed for container {containerId}:  {ex.Message}");
        }
    }

}
