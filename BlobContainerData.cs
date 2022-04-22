using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public string SAS { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public int FilesCount { get; set; }
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
                        FilesCount++;
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

        public void CopyBlobAsync(string origen, string destino)
        {
            try
            {
                // if (sourceBlob.Exists())
                // {
                // var cmd = $"azcopy.exe copy \"{origen}\" \"{destino}\" --recursive";
                // System.Console.WriteLine(cmd);
                var run = RunExternalExe("azcopy.exe", $"copy \"{origen}\" \"{destino}\" --recursive");
                System.Console.WriteLine(run);
                // var dest = new BlobClient(this.ConnectionString, sourceBlob.BlobContainerName, sourceBlob.Name);

                // var blobServiceClient = new BlobServiceClient(this.ConnectionString);
                // blobServiceClient.GetBlobContainerClient(sourceBlob.BlobContainerName);
                // var r = dest.StartCopyFromUriAsync(sourceBlob.Uri).Result;

                // }
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();

            }
        }


        public string RunExternalExe(string filename, string arguments = null)
        {
            var process = new Process();

            process.StartInfo.FileName = filename;
            if (!string.IsNullOrEmpty(arguments))
            {
                process.StartInfo.Arguments = arguments;
            }

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            var stdOutput = new StringBuilder();
            process.OutputDataReceived += (sender, args) => stdOutput.AppendLine(args.Data); // Use AppendLine rather than Append since args.Data is one line of output, not including the newline character.

            string stdError = null;
            try
            {
                process.Start();
                process.BeginOutputReadLine();
                stdError = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                return "OS error while executing " + Format(filename, arguments) + ": " + e.Message;
            }

            if (process.ExitCode == 0)
            {
                return stdOutput.ToString();
            }
            else
            {
                var message = new StringBuilder();

                if (!string.IsNullOrEmpty(stdError))
                {
                    message.AppendLine(stdError);
                }

                if (stdOutput.Length != 0)
                {
                    message.AppendLine("Std output:");
                    message.AppendLine(stdOutput.ToString());
                }

                return Format(filename, arguments) + " finished with exit code = " + process.ExitCode + ": " + message;
            }
        }

        private string Format(string filename, string arguments)
        {
            return "'" + filename +
                ((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments) +
                "'";
        }
    }
}
