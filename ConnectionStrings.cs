using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobsComparer
{
    internal class ConnectionStrings
    {
        public BlobContainerData Origen { get; set; }
        public BlobContainerData Destino { get; set; }
    }
}
