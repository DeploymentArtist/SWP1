using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MonitoringDemoCSharp.Models;
using System.Reflection;
using System.IO;

namespace MonitoringDemoCSharp.Controllers
{
    public class MDTMonitorEventController : ApiController
    {
        [HttpGet]
        public string PostEvent([FromUri]MonitorEvent monitorEvent)
        {

            if (string.IsNullOrEmpty(monitorEvent.uniqueID))
            {
                monitorEvent.uniqueID = Guid.NewGuid().ToString();
            }

            string param = "";
            string csv = "";
            var props = typeof(MonitorEvent).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object value = prop.GetValue(monitorEvent) ?? "";
                string strValue = value.ToString();
                param += string.Format("{0}={1}&", prop.Name, strValue);
                csv += string.Format("{0};", value.ToString());
            }

            // Create URL
            string MDTURL = "http://MDT01.corp.viamonstra.com:9800/MDTMonitorEvent/PostEvent?";
            Uri MDTURI = new Uri(MDTURL + param);

            // Send data 
            WebClient client = new WebClient();
            //Stream data = client.OpenRead(MDTURI);
            client.OpenRead(MDTURI);

            csv += Environment.NewLine;
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/");
            File.AppendAllText(path + "MonitorEvents.csv", csv);
            
            return monitorEvent.uniqueID;
        }

        [HttpGet]
        public string GetSettings(string uniqueID)
        {
            return "";
        }
    }
}
