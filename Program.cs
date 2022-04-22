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

var hacerTransferencia = false;
if (args.Length > 0)
{
    hacerTransferencia = args[0] == "-c";
}
var conexiones = config.GetRequiredSection("ConnectionStrings").Get<ConnectionStrings>();
var origen = conexiones.Origen;
var destino = conexiones.Destino;

var setDestino = await destino.GetContent();
var setOrigen = await origen.GetContent();
System.Console.WriteLine($"Contenedores en origen: {origen.Containers.Count}");
System.Console.WriteLine($"Contenedores en destino: {destino.Containers.Count}");
System.Console.WriteLine($"Archivos en origen: {origen.FilesCount}");
System.Console.WriteLine($"Archivos en destino: {destino.FilesCount}");
foreach (var contenedorOrigen in origen.Containers)
{
    if (destino.Containers.Exists(x => x.Name == contenedorOrigen.Name))
    {
        System.Console.WriteLine($"el folder {contenedorOrigen.Name} existe en el StorageAccount {destino.Name}");
        foreach (var file in contenedorOrigen.Files)
        {
            var contenedorDestino = destino.Containers.Where(x => x.Name == contenedorOrigen.Name).FirstOrDefault();
            if (contenedorDestino == null)
                continue;
            if (contenedorDestino.Files.Exists(x => x.Name == file.Name))
            {
                System.Console.WriteLine($"el archivo {file.Name} existe en el  StorageAccount {destino.Name}");
            }
            else
            {
                System.Console.WriteLine($"el archivo {file.Name} no existe en el  StorageAccount {destino.Name} se debe copiar");
                var toCopy = origen.Containers.Where(x => x.Name == contenedorOrigen.Name).FirstOrDefault().Files.Where(x => x.Name == file.Name).FirstOrDefault();
                var copiar = new BlobClient(origen.ConnectionString, toCopy.BlobContainerName, toCopy.Name);
                var destinourl = $"https://{destino.Name}.blob.core.windows.net{toCopy.Uri.LocalPath}{destino.SAS}";
                var copiarurl = $"https://{origen.Name}.blob.core.windows.net{toCopy.Uri.LocalPath}{origen.SAS}";
                if (hacerTransferencia)
                {
                    System.Console.WriteLine($"copiando {file.Name}");
                    destino.CopyBlobAsync(copiarurl, destinourl);
                }
            }
        }
    }
    else
    {
        System.Console.WriteLine($"el folder {contenedorOrigen.Name} no existe en el StorageAccount {destino.Name}, se debe copiar");
        var destinourl = $"https://{destino.Name}.blob.core.windows.net/{contenedorOrigen.Name}{destino.SAS}";
        var copiarurl = $"https://{origen.Name}.blob.core.windows.net/{contenedorOrigen.Name}{origen.SAS}";
        if (hacerTransferencia)
        {
            System.Console.WriteLine($"copiando contenido de {contenedorOrigen.Name}");
            destino.CopyBlobAsync(copiarurl, destinourl);
        }
    }
}
System.Console.WriteLine();