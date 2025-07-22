using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Models
{
    public class HealthCheckRequest
    {

        public string Id { get; set; }
        public string SystemTime { get; set; }
        public int NumberofConnectedClients { get; set; }
    }
}
