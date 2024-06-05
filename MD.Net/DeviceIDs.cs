using System;
using System.Collections.Generic;

namespace MD.Net
{
    public static class DeviceIDs
    {
        public const int Sony = 0x054c;

        public const int Sharp = 0x04dd;

        public static readonly IDictionary<int, string> Vendors = new Dictionary<int, string>()
        {
            { Sony, "Sony" },
            { Sharp, "Sharp" }
        };

        public static readonly IDictionary<int, string> SonyProducts = new Dictionary<int, string>()
        {
            { 0x34, "Sony PCLK-XX" },
            { 0x36, "Sony (unknown model)" },
            { 0x6F, "Sony NW-E7" },
            { 0x75, "Sony MZ-N1" },
            { 0x7c, "Sony (unknown model)" },
            { 0x80, "Sony LAM-1" },
            { 0x81, "Sony MDS-JE780/JB980" },
            { 0x84, "Sony MZ-N505" },
            { 0x85, "Sony MZ-S1" },
            { 0x86, "Sony MZ-N707" },
            { 0x8e, "Sony CMT-C7NT" },
            { 0x97, "Sony PCGA-MDN1" },
            { 0xad, "Sony CMT-L7HD" },
            { 0xc6, "Sony MZ-N10" },
            { 0xc7, "Sony MZ-N910" },
            { 0xc8, "Sony MZ-N710/NE810/NF810" },
            { 0xc9, "Sony MZ-N510/NF610" },
            { 0xca, "Sony MZ-NE410/DN430/NF520" },
            { 0xeb, "Sony MZ-NE810/NE910" },
            { 0xe7, "Sony CMT-M333NT/M373NT" },
            { 0x101, "Sony LAM-10" },
            { 0x113, "Aiwa AM-NX1" },
            { 0x119, "Sony CMT-SE9" },
            { 0x13f, "Sony MDS-S500" },
            { 0x14c, "Aiwa AM-NX9" },
            { 0x17e, "Sony MZ-NH1 MD" },
            { 0x180, "Sony MZ-NH3D MD" },
            { 0x182, "Sony MZ-NH900 MD" },
            { 0x184, "Sony MZ-NH700/800 MD" },
            { 0x186, "Sony MZ-NH600/600D MD" },
            { 0x188, "Sony MZ-N920 MD" },
            { 0x18a, "Sony LAM-3 MD" },
            { 0x1e9, "Sony MZ-DH10P MD" },
            { 0x219, "Sony MZ-RH10 MD" },
            { 0x21b, "Sony MZ-RH910 MD" },
            { 0x21d, "Sony CMT-AH10 MD" },
            { 0x22c, "Sony CMT-AH10 MD" },
            { 0x23c, "Sony DS-HMD1 MD" },
            { 0x286, "Sony MZ-RH1 MD" }
         };

        public static readonly IDictionary<int, string> SharpProducts = new Dictionary<int, string>()
        {
            { 0x7202, "Sharp IM-MT880H/MT899H" },
            { 0x9013, "Sharp IM-DR400/DR410" },
            { 0x9014, "Sharp IM-DR80/DR420/DR580 or Kenwood DMC-S9NET" }
        };

        public static bool TryParse(string value, out int result)
        {
            try
            {
                result = Convert.ToInt32(value, 16);
                return true;
            }
            catch
            {
                result = 0;
                return false;
            }
        }
    }
}
