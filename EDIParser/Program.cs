using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using log4net;
using log4net.Config;
using Microsoft.Azure.Cosmos;

internal class Program
{
    public static readonly ILog log = LogManager.GetLogger(typeof(Program));

    private static async Task Main(string[] args)
    {
        XmlConfigurator.Configure(new FileInfo("log4net.config"));
        string sourceFolderPath = @"D:/POC/Drop_EDI"; // Folder with EDI files
        string processedFolderPath = Path.Combine(sourceFolderPath, "Processed"); // Folder to move processed files

        try
        {
            await CosmosDbHelper.CreateDatabaseAndContainerAsync();
            
            var ediProcessorService = new EdiProcessorService(sourceFolderPath, processedFolderPath, log);
            await ediProcessorService.ProcessEdiFilesAsync();
        }
        catch (InvalidOperationException ex)
        {
            log.Error("Invalid operation: " + ex.Message);
            Console.WriteLine("Error: " + ex.Message);
        }
        catch (Exception ex)
        {
            log.Error("An error occurred: " + ex.Message, ex);
            Console.WriteLine("An unexpected error occurred.");
        }
    }
}
