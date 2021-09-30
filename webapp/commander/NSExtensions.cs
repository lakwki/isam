using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace com.next.nsextensions
{
    public static class NSExtensions
    {
        public static bool In(this object o, IEnumerable c)
        {
            foreach (object i in c)
            {
                if (i.Equals(o))
                    return true;
            }
            return false;
        }

        public static bool IsValidEmailAddress(this string s)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(s);
        }

    }
}
