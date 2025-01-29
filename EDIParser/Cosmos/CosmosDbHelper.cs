using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

public static class CosmosDbHelper
{
    private static CosmosClient cosmosClient;
    private static Container container;
    public static readonly ILog log = LogManager.GetLogger(typeof(Program));
    

    public static string cosmosEndpoint= "https://localhost:8081";
    public static string cosmosKey="C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

    public static string databaseId = "EDIParserDb"; 
    public static string containerId = "EDIContainer";

    static CosmosDbHelper()
    {
        
       cosmosClient = new CosmosClient(cosmosEndpoint, cosmosKey);       
    }

    public static async Task CreateDatabaseAndContainerAsync()
    {
        try
        {
            
            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            log.Info($"Database '{databaseId}' created or already exists.");

            var containerProperties = new ContainerProperties
            {
                Id = containerId,
                PartitionKeyPath = "/ContainerId"
            };

            container = await database.CreateContainerIfNotExistsAsync(containerProperties);
            log.Info($"Container '{containerId}' created or already exists.");
        }
        catch (Exception ex)
        {
            log.Error($"Error creating database or container: {ex.Message}", ex);
            throw;
        }
    }

    public static async Task UploadToCosmosDbAsync(List<CNT> containers)
    { 
        try
        {
            foreach (var containerItem in containers)
            {   
                containerItem.ContainerDetails=null;
                if(containerItem.AdditionalFees!=0 || containerItem.DemurrageFees!=0){
                    containerItem.Holds="yes";
                }else{
                    containerItem.Holds="No";
                }
                containerItem.id = Guid.NewGuid().ToString();
                await container.CreateItemAsync(containerItem, new PartitionKey(containerItem.ContainerId));
                log.Info($"Uploaded container with ID: {containerItem.ContainerId} to Cosmos DB.");
            }
        }
        catch (CosmosException cosmosEx)
        {
            log.Error($"Cosmos DB error: {cosmosEx.Message}");
        }
        catch (Exception ex)
        {
            log.Error($"An error occurred during Cosmos DB upload: {ex.Message}", ex);
        }
    }
}
