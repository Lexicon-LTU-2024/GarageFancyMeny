using System;
using System.Collections.Generic;
using System.Text;

namespace Exercise5
{
    class Motorcycle : Vehicle
    {
        public string Make { get; set; }
        public Motorcycle(string regNo, string color, int nrOfWheels, string fuelType, string make)
            : base(regNo, color, nrOfWheels, fuelType)
        {
            Make = make;
        }

        public override string GetDescription()
        {
            return $"Brand is {Make}";
        }
    }
}
