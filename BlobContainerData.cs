using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
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
        public List<BlobFile> Files { get; set; }
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
                var blobServiceClient = new BlobServiceClient(this.ConnectionString);
                var storageAccount = CloudStorageAccount.Parse(this.ConnectionString);
                this.Containers = new List<Container>();
                this.Name = blobServiceClient.AccountName;
                await foreach (var blobItem in blobServiceClient.GetBlobContainersAsync())
                {
                    var containerClient = blobServiceClient.GetBlobContainerClient(blobItem.Name);
                    var containerBlobs = containerClient.GetBlobs();
                    var files = new List<BlobFile>();
                    foreach (var b in containerBlobs)
                    {
                        var blobClient = containerClient.GetBlobClient(b.Name);
                        files.Add(new BlobFile()
                        {
                            Name = blobClient.Name,
                            Uri = blobClient.Uri
                        });
                    }
                    this.Containers.Add(new Container()
                    {
                        Name = blobItem.Name,
                        Files = files
                    });
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
