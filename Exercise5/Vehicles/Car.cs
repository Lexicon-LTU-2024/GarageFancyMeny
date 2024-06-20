using System;
using System.Collections.Generic;
using System.Text;

namespace Exercise5
{
    public class Car : Vehicle
    {
        public string Make { get; }

        public Car(string regNo, string color, int nrOfWheels, string fuelType, string make) 
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
