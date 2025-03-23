namespace OnlineTrainReserveSystem.Dtos
{
    public class BookingDetailsResponse
    {
        public long PNRNo { get; set; }
        public int UserId { get; set; }
        public required string Username { get; set; }
        public int TrainId { get; set; }
        public required string TrainName { get; set; }
        public required string Source { get; set; }
        public required string Destination { get; set; }
        public DateOnly JourneyDate { get; set; }
        public required string ClassType { get; set; }
        public int TotalSeats { get; set; }
        public decimal TotalFare { get; set; }
        public required string PaymentStatus { get; set; }
        public required string CancellationStatus { get; set; }
        public DateTime? CancellationTimestamp { get; set; }
    }
}