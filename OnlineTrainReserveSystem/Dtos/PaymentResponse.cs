namespace OnlineTrainReserveSystem.Dtos
{
    public class PaymentResponse
    {
        public long PNRNo { get; set; }
        public int PaymentId { get; set; }
        public decimal TotalFare { get; set; }
        public string PaymentStatus { get; set; } // e.g., "Completed", "Pending", "Failed"
    }
}