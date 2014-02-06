using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snarkdown_WPF
{
    class GfmSyntaxProvider
    {
        static List<string> tags = new List<string>
        {
            " _",
            " *",
            " __",
            " **",
            " ~~",
            "<!--",
            "-->"
        };
        static List<string> flags = new List<string>
        {
            "#",
            " * ",
            " + ",
            " - ",
            "\t"
        };
        public static bool StringContainsTags (string s)
        {
            bool r = false;
            foreach (string t in tags)
            {
                if (s.Contains(t) == true)
                    r = true;
            }
            return r;
        }
        public static bool StringContainsFlags (string s)
        {
            bool r = false;
            // remove tabs down to a max of 1 tab, to cover nested lists
            while (s.Contains("\t\t"))
            {
                s.Replace("\t\t", "\t");
            }
            foreach (string f in flags)
            {
                if (s.StartsWith(f) == true)
                    r = true;
            }
            return r;
        }
    }
}
