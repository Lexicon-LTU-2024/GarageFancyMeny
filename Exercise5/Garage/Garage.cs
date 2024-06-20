using Exercise5.Garage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Exercise5
{
    public class Garage<T> : IEnumerable<T>, IGarage where T : Vehicle
    {
        private Vehicle[] vehicles;

        private int count;
        public int Count
        {
            get { return count; }
        }

        public int Capacity
        {
            get { return vehicles.Length; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="capacity">Number of parking stalls in the garage</param>
        public Garage(int capacity) // number of parking stalls
        {
            vehicles = new Vehicle[capacity];
            count = 0;
        }

        /// <summary>
        /// Parks a vehicle in the garage
        /// </summary>
        /// <param name="vehicle">The vehicle to park</param>
        /// <returns>Returns a PerkingResult object with info about how things worked out</returns>
        public ParkingResult ParkVehicle(Vehicle vehicle)
        {
            ParkingResult result;
            if (count < vehicles.Length) // if not full
            {
                if (IndexOf(vehicle.RegNo) == -1) // if not duplicate
                {
                    vehicles[count] = vehicle; // add it
                    count++;
                    result = new ParkingResult(true, null);
                }
                else
                {
                    result = new ParkingResult(false, $"{vehicle.RegNo} not unique!");
                }
            }
            else
            {
                result = new ParkingResult(false, "Garage is full");
            }
            return result;
        }

        /// <summary>
        /// Unparks a vehicle from the garage
        /// </summary>
        /// <param name="regNo">The registration number of the vehicle</param>
        /// <returns>Returns a reference to the unparked vehicle, or null if not found</returns>
        public Vehicle UnparkVehicle(string regNo)
        {
            Vehicle vehicle = null; // not found yet

            var matchIndex = IndexOf(regNo.ToUpper());
            if (matchIndex != -1) // if vehicle found
            {
                vehicle = vehicles[matchIndex];
                count--;
                for (int i = matchIndex; i < count; i++) // cover the space where the vehicle was
                {
                    vehicles[i] = vehicles[i + 1];
                }
            }

            return vehicle;
        }

        /// <summary>
        /// Returns the index of the vehicle with the registration number
        /// </summary>
        /// <param name = "regNo" > The registration number to search for. This will be converted to uppercase before searching.</param>
        /// <returns>Returns index if found or -1 if not found</returns>
        public int IndexOf(string regNo)
        {
            int matchIndex = -1; // not found yet
            for (int i = 0; i < count; i++)
            {
                if (vehicles[i].RegNo == regNo.ToUpper())
                {
                    matchIndex = i;
                    break;
                }
            }
            return matchIndex;
        }

        /// <summary>
        /// Gets a specific vehicle via registration number
        /// </summary>
        /// <returns>Returns a reference to the vehicle, or null if not found</returns>
        public Vehicle GetVehicle(string regNo)
        {
            var index = IndexOf(regNo);
            return (index != -1) ? vehicles[index] : null;
        }

        public bool IsFull()
        {
            return (vehicles.Length == count);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
            {
                yield return vehicles[i] as T;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
