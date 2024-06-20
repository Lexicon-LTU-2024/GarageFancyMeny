using System;
using System.Collections.Generic;
using System.Text;

namespace Exercise5.Garage
{
    public class ParkingResult
    {
        public bool Success { get; }
        public string Message { get; }

        public ParkingResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
