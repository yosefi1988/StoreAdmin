using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationStoreAdmin.Models
{
    public class RequestParameters
    {
        public string MerchantId { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public string CallbackUrl { get; set; }
        public string[] metadata { get; set; }

        public RequestParameters(string merchantId, string amount, string description, string callbackUrl, string mobile, string email)
        {
            MerchantId = merchantId;
            Amount = amount;
            Description = description;
            CallbackUrl = callbackUrl;
            this.metadata = new string[2];
            if (mobile != null)
            {
                this.metadata[0] = mobile;
            }
            if (email != null)
            {
                this.metadata[1] = email;
            }
        }
    }
}
