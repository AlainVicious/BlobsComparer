using Azure.Storage;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobsComparer
{
    internal class BlobContainerData
    {
        public string Sas { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ConnectionString { get; set; }

        public void GetBlobServiceClient(ref BlobServiceClient blobServiceClient)
        {
            StorageSharedKeyCredential sharedKeyCredential =
                new StorageSharedKeyCredential(Name, Sas);

            string blobUri = "https://" + Name + ".blob.core.windows.net";

            blobServiceClient = new BlobServiceClient
                (new Uri(blobUri), sharedKeyCredential);
        }
    }
}
