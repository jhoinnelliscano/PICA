using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Commons.Objects.Responses.Order
{
    public class EpayCoResponse
    {
        public bool success { get; set; }
        public string title_response { get; set; }
        public string text_response { get; set; }
        public string last_action { get; set; }
        public EpayCoDataResponse data { get; set; }
    }
}
