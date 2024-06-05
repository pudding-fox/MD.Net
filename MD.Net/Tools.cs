using System.IO;

namespace MD.Net
{
    public class Tools
    {
        public static string Location
        {
            get
            {
                return Path.GetDirectoryName(typeof(Tools).Assembly.Location);
            }
        }

        public static string ATRACDENC
        {
            get
            {
                return Path.Combine(Location, "x86", "atracdenc.exe");
            }
        }

        public static string HIMDCLI
        {
            get
            {
                return Path.Combine(Location, "x86", "himdcli.exe");
            }
        }

        public static string NETMDCLI
        {
            get
            {
                return Path.Combine(Location, "x86", "netmdcli.exe");
            }
        }
    }
}