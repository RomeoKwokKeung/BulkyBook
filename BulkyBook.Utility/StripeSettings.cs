using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.Utility
{
    public class StripeSettings
    {
        //GET appsettings.json
        //load it in startup.cs
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
    }
}
