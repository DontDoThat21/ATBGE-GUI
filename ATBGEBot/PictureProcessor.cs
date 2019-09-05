using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramATBGEBot
{
    class PictureProcessor
    {
        public static string RedditLinkVerification(string RawLink)
        {
            int PeriodLocation = RawLink.LastIndexOf('.'); // Should get the int value right before the .com

            if (RawLink.Substring(PeriodLocation, PeriodLocation-9) != ".com")
            {

                // Not a .com link, clearly not a reddit link unless reddit.com changes.
                return String.Empty;
            }
            else if (true)
            {

            }
            // lets check if this is a reddit link or not.
            return "";
        }
    }
}
