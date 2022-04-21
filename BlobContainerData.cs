using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobsComparer
{
    internal class BlobFile
    {
        public string Name { get; set; }
        public Uri Uri { get; set; }
    }
    internal class Container
    {
        public string Name { get; set; }
        public List<BlobClient> Files { get; set; }

    }
    internal class BlobContainerData
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public List<Container> Containers { get; private set; }

        public async Task<bool> GetContent()
        {
            try
            {
                if (string.IsNullOrEmpty(this.ConnectionString))
                {
                    throw new Exception("La cadena de conexion esta vacia");
                }
                var blobServiceClient = new BlobServiceClient(this.ConnectionString);

                // var storageAccount = CloudStorageAccount.Parse(this.ConnectionString);

                this.Containers = new List<Container>();
                this.Name = blobServiceClient.AccountName;
                await foreach (var blobItem in blobServiceClient.GetBlobContainersAsync())
                {
                    var containerClient = blobServiceClient.GetBlobContainerClient(blobItem.Name);
                    var containerBlobs = containerClient.GetBlobs();
                    var files = new List<BlobClient>();
                    foreach (var b in containerBlobs)
                    {
                        files.Add(containerClient.GetBlobClient(b.Name));
                    }
                    this.Containers.Add(new Container()
                    {
                        Name = blobItem.Name,
                        Files = files
                    });
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error al intentar obtener la informacion de los blobs: " + ex.Message);
                return false;
            }
        }

        public async Task CopyBlobAsync(BlobClient sourceBlob)
        {
            try
            {
                if (await sourceBlob.ExistsAsync())
                {

                    var dest = new BlobClient(this.ConnectionString, sourceBlob.BlobContainerName, sourceBlob.Name);

                    var blobServiceClient = new BlobServiceClient(this.ConnectionString);
                    blobServiceClient.GetBlobContainerClient(sourceBlob.BlobContainerName);
                    await dest.StartCopyFromUriAsync(sourceBlob.Uri);

                }
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
                
            }
        }
    }
}
