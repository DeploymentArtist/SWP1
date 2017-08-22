using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonitoringDemoCSharp.Models
{
    public class MonitorEvent
    {
        public string uniqueID { get; set; }
        public string computerName { get; set; }
        public string messageID { get; set; }
        public string severity { get; set; }
        public string stepName { get; set; }
        public Int16 currentstep { get; set; }
        public Int16 totalSteps { get; set; }
        public string id { get; set; }
        public string message { get; set; }
        public string dartIP { get; set; }
        public string dartPort { get; set; }
        public string dartTicket { get; set; }
        public string vmHost { get; set; }
        public string vmName { get; set; }

    }
}