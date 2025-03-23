﻿using System.ComponentModel.DataAnnotations;

namespace OnlineTrainReserveSystem.Dtos
{
    public class FareCalculationRequest
    {
        [Required(ErrorMessage = "TrainId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "TrainId must be a positive integer.")]
        public int TrainId { get; set; }

        [Required(ErrorMessage = "ClassType is required.")]
        [RegularExpression("^(General|Sleeper|Tier3AC|Tier2AC|Tier1AC)$", ErrorMessage = "ClassType must be one of: General, Sleeper, Tier3AC, Tier2AC, Tier1AC.")]
        public required string ClassType { get; set; }

        [Required(ErrorMessage = "TotalSeats is required.")]
        [Range(1, 10, ErrorMessage = "TotalSeats must be between 1 and 10.")]
        public int TotalSeats { get; set; }
    }
}