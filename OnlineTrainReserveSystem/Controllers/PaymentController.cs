using Microsoft.AspNetCore.Mvc;
using OnlineTrainReserveSystem.Dtos; // Updated namespace for DTOs
using OnlineTrainReserveSystem.Services; // Updated namespace for PaymentService

namespace OnlineTrainReserveSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("book")]
        public IActionResult BookTicket([FromBody] PaymentRequest request)
        {
            try
            {
                // Validate the request
                if (request == null || request.UserId <= 0 || request.TrainId <= 0 || request.TotalSeats <= 0)
                {
                    return BadRequest("Invalid booking request.");
                }

                // Call the PaymentService to process the booking
                var response = _paymentService.ProcessBooking(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("cancel/{PNRNo}")]
        public IActionResult CancelBooking(long PNRNo, [FromBody] RefundRequest request)
        {
            try
            {
                // Validate PNRNo consistency
                if (PNRNo != request.PNRNo)
                {
                    return BadRequest("PNR number in URL and request body do not match.");
                }

                // Call PaymentService to process cancellation and refund
                var response = _paymentService.ProcessCancellation(PNRNo);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cancellation failed: {ex.Message}. Inner Exception: {ex.InnerException?.Message}");
            }
        }

        [HttpPost("calculate-fare")]
        public IActionResult CalculateFare([FromBody] FareCalculationRequest request)
        {
            try
            {
                // Validate the request
                if (request == null || request.TrainId <= 0 || string.IsNullOrEmpty(request.ClassType) || request.TotalSeats <= 0)
                {
                    return BadRequest("Invalid fare calculation request.");
                }

                // Call PaymentService to calculate the fare
                var totalFare = _paymentService.CalculateFare(request.TrainId, request.ClassType, request.TotalSeats);
                var response = new FareCalculationResponse
                {
                    TotalFare = totalFare
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Fare calculation failed: {ex.Message}");
            }
        }

        [HttpPost("check-availability")]
        public IActionResult CheckSeatAvailability([FromBody] SeatAvailabilityRequest request)
        {
            try
            {
                // Validate the request
                if (request == null || request.TrainId <= 0 || string.IsNullOrEmpty(request.ClassType))
                {
                    return BadRequest("Invalid seat availability request.");
                }

                // Call PaymentService to check seat availability
                var availableSeats = _paymentService.CheckSeatAvailability(request.TrainId, request.JourneyDate, request.ClassType);
                var response = new SeatAvailabilityResponse
                {
                    AvailableSeats = availableSeats
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Seat availability check failed: {ex.Message}");
            }
        }

        [HttpGet("booking/{PNRNo}")]
        public IActionResult GetBookingDetails(long PNRNo)
        {
            try
            {
                var response = _paymentService.GetBookingDetails(PNRNo);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to retrieve booking details: {ex.Message}");
            }
        }
    }
}