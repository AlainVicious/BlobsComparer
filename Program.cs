using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobsComparer;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var conexiones = config.GetRequiredSection("ConnectionStrings").Get<ConnectionStrings>();
var origen = conexiones.Origen;
var destino = conexiones.Destino;

BlobServiceClient blobServiceClient = new BlobServiceClient(destino.ConnectionString);
List<string> blos = new List<string>();
await foreach (var blobItem in blobServiceClient.GetBlobContainersAsync())
{
    blos.Add(blobItem.Name);
}


// Get Reference to Blob Container
CloudStorageAccount storageAccount = CloudStorageAccount.Parse(destino.ConnectionString);
CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
CloudBlobContainer container = blobClient.GetContainerReference(destino.Name);

// Fetch info about files in the container
// Note: Loop with BlobContinuationToken to fetch results in pages. Pass null as currentToken to fetch all results.
BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(currentToken: null);
IEnumerable<IListBlobItem> blobItems = resultSegment.Results;

// Extract the URI of the files into a new list
List<string> fileUris = new List<string>();
foreach (var blobItem in blobItems)
{
	fileUris.Add(blobItem.StorageUri.PrimaryUri.ToString());
}

Console.WriteLine("");

