using OnlineTrainReserveSystem.Models; // Updated namespace for your project
using OnlineTrainReserveSystem.Dtos; // Updated namespace for DTOs
using Microsoft.EntityFrameworkCore; // For transactions

namespace OnlineTrainReserveSystem.Services
{
    public class PaymentService
    {
        private readonly TrainReservationDbContext _context;

        public PaymentService(TrainReservationDbContext context)
        {
            _context = context;
        }

        // Existing CalculateFare method (already added)
        public decimal CalculateFare(int trainId, string classType, int totalSeats)
        {
            var train = _context.Trains
                .FirstOrDefault(t => t.TrainId == trainId);

            if (train == null)
            {
                throw new Exception("Train not found.");
            }

            decimal multiplier = classType switch
            {
                "General" => 1.0m,
                "Sleeper" => 1.5m,
                "Tier3AC" => 2.0m,
                "Tier2AC" => 2.5m,
                "Tier1AC" => 3.0m,
                _ => throw new Exception("Invalid class type.")
            };

            decimal totalFare = train.BasicFare * multiplier * totalSeats;
            return totalFare;
        }

        // New method for booking and payment
        public PaymentResponse ProcessBooking(PaymentRequest request)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // Step 1: Check seat availability
                var availability = _context.TrainAvailabilities
                    .FirstOrDefault(ta => ta.TrainId == request.TrainId && ta.JourneyDate == DateOnly.FromDateTime(request.JourneyDate));

                if (availability == null)
                {
                    throw new Exception("No availability found for the selected train and date.");
                }

                // Check seats for the specific class
                int availableSeats = request.ClassType switch
                {
                    "General" => availability.General,
                    "Sleeper" => availability.Sleeper,
                    "Tier3AC" => availability.Tier3Ac,
                    "Tier2AC" => availability.Tier2Ac,
                    "Tier1AC" => availability.Tier1Ac,
                    _ => throw new Exception("Invalid class type.")
                };

                if (availableSeats < request.TotalSeats)
                {
                    throw new Exception($"Not enough seats available in {request.ClassType}. Available: {availableSeats}");
                }

                // Step 2: Calculate fare
                decimal totalFare = CalculateFare(request.TrainId, request.ClassType, request.TotalSeats);

                // Step 3: Generate a PNR number (simple approach: use timestamp + random)
                long pnrNo = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")) + new Random().Next(1000, 9999);

                // Step 4: Create a reservation
                var reservation = new Reservation
                {
                    Pnrno = pnrNo,
                    UserId = request.UserId,
                    TrainId = request.TrainId,
                    TotalSeats = request.TotalSeats,
                    JourneyDate = DateOnly.FromDateTime(request.JourneyDate),
                    ClassType = request.ClassType,
                    TotalFare = totalFare,
                    PaymentStatus = "Pending",
                    CancellationStatus = "Active",
                    CancellationTimestamp = null
                };
                _context.Reservations.Add(reservation);

                // Step 5: Simulate payment (mock success)
                var payment = new Payment
                {
                    Pnrno = pnrNo,
                    UserId = request.UserId,
                    Amount = totalFare,
                    PaymentMethod = request.PaymentMethod,
                    PaymentTimestamp = DateTime.Now,
                    PaymentStatus = "Completed" // Mocking a successful payment
                };
                _context.Payments.Add(payment);

                // Step 6: Update seat availability
                availability.TotalSeatsAvailable -= request.TotalSeats;
                switch (request.ClassType)
                {
                    case "General":
                        availability.General -= request.TotalSeats;
                        break;
                    case "Sleeper":
                        availability.Sleeper -= request.TotalSeats;
                        break;
                    case "Tier3AC":
                        availability.Tier3Ac -= request.TotalSeats;
                        break;
                    case "Tier2AC":
                        availability.Tier2Ac -= request.TotalSeats;
                        break;
                    case "Tier1AC":
                        availability.Tier1Ac -= request.TotalSeats;
                        break;
                }

                // Step 7: Update reservation status after payment
                reservation.PaymentStatus = "Confirmed";

                // Step 8: Save all changes
                _context.SaveChanges();
                transaction.Commit();

                // Step 9: Return response
                return new PaymentResponse
                {
                    PNRNo = pnrNo,
                    PaymentId = payment.PaymentId,
                    TotalFare = totalFare,
                    PaymentStatus = payment.PaymentStatus
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception($"Booking failed: {ex.Message}");
            }
        }


        public RefundResponse ProcessCancellation(long pnrNo)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // Step 1: Find the reservation
                var reservation = _context.Reservations
                    .FirstOrDefault(r => r.Pnrno == pnrNo);

                if (reservation == null)
                {
                    throw new Exception("Reservation not found.");
                }

                // Step 2: Check if the reservation can be cancelled
                if (reservation.CancellationStatus == "Cancelled")
                {
                    throw new Exception("Reservation is already cancelled.");
                }

                if (reservation.PaymentStatus != "Confirmed")
                {
                    throw new Exception("Cannot cancel a reservation that is not confirmed.");
                }

                // Step 3: Find the associated payment
                var payment = _context.Payments
                    .FirstOrDefault(p => p.Pnrno == pnrNo && p.PaymentStatus == "Completed");

                if (payment == null)
                {
                    throw new Exception("No completed payment found for this reservation.");
                }

                // Step 4: Find the train availability for the journey date
                var availability = _context.TrainAvailabilities
                    .FirstOrDefault(ta => ta.TrainId == reservation.TrainId && ta.JourneyDate == reservation.JourneyDate);

                if (availability == null)
                {
                    throw new Exception("Train availability data not found for this journey.");
                }

                // Step 5: Mark the reservation as cancelled
                reservation.CancellationStatus = "Cancelled";
                reservation.CancellationTimestamp = DateTime.Now;

                // Step 6: Restore seats in TrainAvailability
                availability.TotalSeatsAvailable += reservation.TotalSeats;
                switch (reservation.ClassType)
                {
                    case "General":
                        availability.General += reservation.TotalSeats;
                        break;
                    case "Sleeper":
                        availability.Sleeper += reservation.TotalSeats;
                        break;
                    case "Tier3AC":
                        availability.Tier3Ac += reservation.TotalSeats;
                        break;
                    case "Tier2AC":
                        availability.Tier2Ac += reservation.TotalSeats;
                        break;
                    case "Tier1AC":
                        availability.Tier1Ac += reservation.TotalSeats;
                        break;
                    default:
                        throw new Exception("Invalid class type in reservation.");
                }

                var timeUntilJourney = reservation.JourneyDate.ToDateTime(TimeOnly.MinValue) - DateTime.Now;
                decimal refundPercentage = timeUntilJourney.TotalHours switch
                {
                    > 48 => 0.9m,
                    > 24 => 0.5m,
                    _ => 0m
                };
                decimal refundAmount = reservation.TotalFare * refundPercentage;
                if (refundAmount == 0)
                {
                    throw new Exception("Cancellation not allowed within 24 hours of the journey.");
                }

                // Step 8: Create a refund record
                var refund = new Refund
                {
                    PaymentId = payment.PaymentId,
                    Pnrno = pnrNo,
                    UserId = reservation.UserId,
                    RefundAmount = refundAmount,
                    RefundTimestamp = DateTime.Now,
                    RefundStatus = "Processed"
                };
                _context.Refunds.Add(refund);

                // Step 9: Save all changes
                _context.SaveChanges();
                transaction.Commit();

                // Step 10: Return response
                return new RefundResponse
                {
                    PNRNo = pnrNo,
                    RefundId = refund.RefundId,
                    RefundAmount = refundAmount,
                    RefundStatus = refund.RefundStatus
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception($"Cancellation failed: {ex.Message}");
            }
        }

        public int CheckSeatAvailability(int trainId, DateTime journeyDate, string classType)
        {
            // Find the train availability for the given train and date
            var availability = _context.TrainAvailabilities
                .FirstOrDefault(ta => ta.TrainId == trainId && ta.JourneyDate == DateOnly.FromDateTime(journeyDate));

            if (availability == null)
            {
                throw new Exception("No availability found for the selected train and date.");
            }

            // Return the number of available seats for the specified class
            int availableSeats = classType switch
            {
                "General" => availability.General,
                "Sleeper" => availability.Sleeper,
                "Tier3AC" => availability.Tier3Ac,
                "Tier2AC" => availability.Tier2Ac,
                "Tier1AC" => availability.Tier1Ac,
                _ => throw new Exception("Invalid class type.")
            };

            return availableSeats;
        }


        public BookingDetailsResponse GetBookingDetails(long pnrNo)
        {
            var reservation = _context.Reservations
                .Include(r => r.User)
                .Include(r => r.Train)
                .FirstOrDefault(r => r.Pnrno == pnrNo);

            if (reservation == null)
            {
                throw new Exception("Reservation not found.");
            }

            return new BookingDetailsResponse
            {
                PNRNo = reservation.Pnrno,
                UserId = reservation.UserId,
                Username = reservation.User.Username,
                TrainId = reservation.TrainId,
                TrainName = reservation.Train.TrainName,
                Source = reservation.Train.Source,
                Destination = reservation.Train.Destination,
                JourneyDate = reservation.JourneyDate,
                ClassType = reservation.ClassType,
                TotalSeats = reservation.TotalSeats,
                TotalFare = reservation.TotalFare,
                PaymentStatus = reservation.PaymentStatus,
                CancellationStatus = reservation.CancellationStatus,
                CancellationTimestamp = reservation.CancellationTimestamp
            };
        }


    }
}