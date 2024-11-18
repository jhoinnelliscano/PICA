using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace eCommerce.Commons.Objects.Requests.Payments
{
    public class PaymentEpayCo
    {
        private string _response {get; set;}

        //Parametros compra (obligatorio)
        [Required] public string name { get; set; }
        [Required] public string description { get; set; }
        [Required] public string invoice { get; set; }
        [Required] public string amount { get; set; }
        [Required] public string lang { get; set; }
        [Required] public string currency { get; set; }
        [Required] public string country { get; set; }
        [Required] public string tax_base { get; set; }
        [Required] public string tax { get; set; }
        //Atributos opcionales
        public string extra1 { get; set; }
        public string extra2 { get; set; }
        public string extra3 { get; set; }
        //Atributos cliente
        public string email_billing { get; set; }
        public string name_billing { get; set; }
        public string address_billing { get; set; }
        public string mobilephone_billing { get; set; }
        //Config parametros
        [Required] public string external { get; set; }
        [Required] public string[] methodsDisable { get; set; }
        [Required] public string response 
        { 
            get { return _response.Replace("{ORDERID}", invoice); } 
            set { _response = value; } 
        }
        public string acepted { get; set; }
        public string rejected { get; set; }
        public string pending { get; set; }
        public string confirmation { get; set; }
        public string methodconfirmation { get; set; }
        public string autoclick { get; set; }
    }
}
