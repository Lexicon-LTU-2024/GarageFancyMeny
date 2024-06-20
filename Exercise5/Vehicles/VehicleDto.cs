using System;
using System.Collections.Generic;
using System.Text;

namespace Exercise5
{
    public class VehicleDto : Vehicle
    {
        public VehicleDto(string regNo, string color, int nrOfWheels, string fuelType)
        {
            RegNo = regNo.ToUpper(); // Registration number always in uppercase
            Color = color;
            NrOfWheels = nrOfWheels;
            FuelType = fuelType;
        }
        public override string GetDescription()
        {
            return "DTO";
        }
    }
}
