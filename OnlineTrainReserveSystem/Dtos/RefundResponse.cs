namespace OnlineTrainReserveSystem.Dtos
{
    public class RefundResponse
    {
        public long PNRNo { get; set; }
        public int RefundId { get; set; }
        public decimal RefundAmount { get; set; }
        public required string RefundStatus { get; set; }
    }
}