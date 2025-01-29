using System;

namespace PaymentAPI.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string CardNumber { get; set; }
        public decimal TotalFees { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; }
        public string Username { get; set; }
        public string ContainerId { get; set; }
        public decimal ContainerFee { get; set; } 
    }
}
