using System;
using System.Collections.Generic;
using System.Text;

namespace Exercise5
{
    class Airplane : Vehicle
    {
        public int NrOfEngines { get; set; }
        public Airplane(string regNo, string color, int nrOfWheels, string fuelType, int nrOfEngines)
            : base(regNo, color, nrOfWheels, fuelType)
        {
            NrOfEngines = nrOfEngines;
        }

        public override string GetDescription()
        {
            return $"Has {NrOfEngines} engine{(NrOfEngines != 1 ? "s" : "")}";
        }
    }
}
