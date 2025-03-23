using System.ComponentModel.DataAnnotations;

namespace OnlineTrainReserveSystem.Dtos
{
    public class RefundRequest
    {
        [Required(ErrorMessage = "PNRNo is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "PNRNo must be a positive number.")]
        public long PNRNo { get; set; }
    }
}