using Exercise5.Garage;

namespace Exercise5
{
    public interface IGarage
    {
        int Capacity { get; }
        int Count { get; }

        Vehicle GetVehicle(string regNo);
        int IndexOf(string regNo);
        bool IsFull();
        ParkingResult ParkVehicle(Vehicle vehicle);
        Vehicle UnparkVehicle(string regNo);
    }
}