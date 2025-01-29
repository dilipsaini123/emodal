using System.Collections.Generic;

namespace PaymentAPI.Models
{
    public class PaymentRequest
    {
        public List<PaymentContainer> Containers { get; set; }
        public string Username { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string Expiry { get; set; }
        public decimal TotalFees { get; set; }
    }

    public class PaymentContainer
    {
        public string ContainerId { get; set; }
        public int Fees { get; set; }
       
       
    }
    public class ContainerMessage
{
    public List<PaymentContainer> Containers { get; set; }
}
}
