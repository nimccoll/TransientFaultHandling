using Azure.Storage.Blobs;
using Newtonsoft.Json;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Web;

namespace TransientHandler.Web.Models
{
    public static class ConfigurationSettingsManager
    {
        public static ConfigurationSettings Load()
        {
            ConfigurationSettings settings = null;
            string filePath = HttpContext.Current.Server.MapPath("~/App_Data/configurationSettings.json");
            int retryCount = 0;
            int maxRetryCount = 3;
            int retryDelay = 300;

            do
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        string json = File.ReadAllText(filePath);
                        settings = JsonConvert.DeserializeObject<ConfigurationSettings>(json);
                        break;
                    }
                    catch // Read failed
                    {
                        retryCount++;
                        retryDelay = retryDelay * retryCount;
                        Thread.Sleep(retryDelay);
                    }
                }
                else
                {
                    retryCount++;
                    retryDelay = retryDelay * retryCount;
                    Thread.Sleep(retryDelay);
                }
            } while (retryCount <= maxRetryCount);

            return settings;
        }

        public static ConfigurationSettings LoadFromBlobStorage()
        {
            ConfigurationSettings settings = null;

            string containerName = "configuration";
            string connectionString = ConfigurationManager.AppSettings["AZURE_STORAGE_CONNECTION_STRING"];
            int retryCount = 0;
            int maxRetryCount = 3;
            int retryDelay = 300;

            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // Create the container and return a container client object
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient("configurationSettings.json");

            do
            {
                if (blobClient.Exists())
                {
                    using (StreamReader reader = new StreamReader(blobClient.OpenRead()))
                    {
                        try
                        {
                            string json = reader.ReadToEnd();
                            settings = JsonConvert.DeserializeObject<ConfigurationSettings>(json);
                            break;
                        }
                        catch // Read failed
                        {
                            retryCount++;
                            retryDelay = retryDelay * retryCount;
                            Thread.Sleep(retryDelay);
                        }
                    }
                }
                else
                {
                    retryCount++;
                    retryDelay = retryDelay * retryCount;
                    Thread.Sleep(retryDelay);
                }

            } while (retryCount <= maxRetryCount);
            
            return settings;
        }
    }
}