using System.Web.Configuration;

namespace PhoneDex.Core
{
    public class LocationHelper
    {
        private string LOCALIP = WebConfigurationManager.AppSettings["AllowedLocalIP"];
        private string EXTERNALIP = WebConfigurationManager.AppSettings["AllowedServerIP"];

        public bool IsInSestekNetwork(string clientIP)
        {
            string ip1 = clientIP;

            string shortLocalIP;

            if (ip1 != null && ip1.Contains("."))
            {
                string[] ipValues = ip1.Split('.');
                shortLocalIP = ipValues[0] + "." + ipValues[1];
            }
            else
                shortLocalIP = "192.168";
            
            return (shortLocalIP == LOCALIP || ip1 == EXTERNALIP);
        }
    }
}