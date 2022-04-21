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

var setDestino = await destino.GetContent();
var setOrigen = await origen.GetContent();

foreach (var contenedorOrigen in origen.Containers)
{
    if (destino.Containers.Exists(x=> x.Name == contenedorOrigen.Name))
    {
        System.Console.WriteLine($"el folder {contenedorOrigen.Name} existe en el StorageAccount {destino.Name}");
    }else
    {
        System.Console.WriteLine($"el folder {contenedorOrigen.Name} no existe en el StorageAccount {destino.Name}, se debe copiar");
        
    }
    
}

System.Console.WriteLine();