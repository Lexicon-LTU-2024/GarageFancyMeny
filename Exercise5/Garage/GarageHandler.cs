using Exercise5.Garage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Transactions;
using System.Linq;

namespace Exercise5
{
    public class GarageHandler : IGarageHandler
    {
        private Garage<Vehicle> garage;
        private Stack<Menu> menuStack;
        private EventLog log;
        private IUI ui;

        public GarageHandler()
        {
            garage = new Garage<Vehicle>(15);
            log = new EventLog(12);
            menuStack = new Stack<Menu>();
            ui = new ConsoleUI();
        }

        public void Run()
        {
            Initialize();

            while (true)
            {
                var menu = menuStack.Peek();
                var choice = menu.Run();
                choice.Action?.Invoke(); // if the selected menu option has an action...
                if (choice.SubMenu != null) // ...or has a sub menu
                {
                    choice.SubMenu.ResetCursorIndex();
                    menuStack.Push(choice.SubMenu);
                }
            }
        }

        private void Initialize()
        {
            Menu.Init(ui, log);
            var mainMenu = GetMenuTree();
            menuStack.Push(mainMenu);
        }

        private void Quit()
        {
            ui.Clear();
            Environment.Exit(0);
        }

        private void GoBack()
        {
            menuStack.Pop();
        }

        /// <summary>
        /// Gets the entire navigation tree.
        /// 
        /// This menu is a brand new idea of mine, made from scratch.
        /// 
        /// Seems to work, but it would have been even better
        /// with the possibility to add parameters (for some of
        /// the action calls). Probably in a later version.
        /// </summary>
        /// <returns>Returns the tree in the form of a menu object</returns>
        private Menu GetMenuTree()
        {
            var quitOption = new MenuOption("Quit the application", new Action(Quit));
            var backOption = new MenuOption("Go back", new Action(GoBack));

            var parkingSingleMenu = Menu.Create("Parking a single vehicle");
            parkingSingleMenu.Add(new MenuOption("Park a car", new Action(ParkCar)));
            parkingSingleMenu.Add(new MenuOption("Park a bus", new Action(ParkBus)));
            parkingSingleMenu.Add(new MenuOption("Park an airplane", new Action(ParkAirplane)));
            parkingSingleMenu.Add(new MenuOption("Park a motorcycle", new Action(ParkMotorcycle)));
            parkingSingleMenu.Add(new MenuOption("Park a boat", new Action(ParkBoat)));
            parkingSingleMenu.Add(backOption);

            var parkingMenu = Menu.Create("Parking new vehicles");
            parkingMenu.Add(new MenuOption("Park single vehicle", parkingSingleMenu));
            parkingMenu.Add(new MenuOption("Fill garage with 10 vehicles", new Action(ParkSomeVehicles)));
            parkingMenu.Add(backOption);

            var vehicleSearchMenu = Menu.Create("Search vehicles");
            vehicleSearchMenu.Add(new MenuOption("Search via parameters", new Action(ParametricSearch)));
            vehicleSearchMenu.Add(backOption);

            var garageAdminMenu = Menu.Create("Building construction");
            garageAdminMenu.Add(new MenuOption("Create new garage", new Action(CreateGarage)));
            garageAdminMenu.Add(backOption);

            var listSpecificMenu = Menu.Create("Listing specific vehicle types");
            listSpecificMenu.Add(new MenuOption("List airplanes", new Action(ListAirplanes)));
            listSpecificMenu.Add(new MenuOption("List boats", new Action(ListBoats)));
            listSpecificMenu.Add(new MenuOption("List busses", new Action(ListBusses)));
            listSpecificMenu.Add(new MenuOption("List cars", new Action(ListCars)));
            listSpecificMenu.Add(new MenuOption("List motorcycles", new Action(ListMotorcycles)));
            listSpecificMenu.Add(backOption);

            var listingMenu = Menu.Create("Listing parked vehicles");
            listingMenu.Add(new MenuOption("List all parked vehicles", new Action(ListParkedVehicles)));
            listingMenu.Add(new MenuOption("List specific types", listSpecificMenu));
            listingMenu.Add(backOption);

            var mainMenu = Menu.Create("Garage ver 0.1    Main menu");
            mainMenu.Add(new MenuOption("Show parked vehicles", listingMenu));
            mainMenu.Add(new MenuOption("Park vehicles", parkingMenu));
            mainMenu.Add(new MenuOption("Unpark vehicles", new Action(UnparkVehicle)));
            mainMenu.Add(new MenuOption("Search", vehicleSearchMenu));
            mainMenu.Add(new MenuOption("Repaint vehicles", new Action(RepaintVehicle)));
            mainMenu.Add(new MenuOption("Building management", garageAdminMenu));
            mainMenu.Add(quitOption);

            return mainMenu;
        }

        private void ParametricSearch()
        {
            ui.Clear();
            ui.DisplayInputHeader("Search for vehicles");

            ui.Write("Please enter parameters to search for!\n");
            ui.Write("Leave any unimportant parameters empty!\n");
            string regNo = ui.GetTextFromUser("Registration number: ").ToUpper();
            string color = ui.GetTextFromUser("Color: ".PadLeft(21));
            int nrOfWheels = ui.GetIntegerFromUser("Nr of wheels: ".PadLeft(21), Const.AcceptEmptyString);
            string fuelType = ui.GetTextFromUser("Fuel type: ".PadLeft(21));

            var matchList = new Garage<Vehicle>(garage.Count);
            foreach (var v in garage)
            {
                bool match = true; // positive default
                if (regNo != "" && v.RegNo != regNo)
                    match = false; // miss
                if (color != "" && v.Color != color)
                    match = false; // miss
                if (nrOfWheels != -1 && v.NrOfWheels != nrOfWheels)
                    match = false; // miss
                if (fuelType != "" && v.FuelType != fuelType)
                    match = false; // miss
                if (match)
                {
                    matchList.ParkVehicle(v); // not really "parked" but added
                                              // to the list of matching vehicles
                }
            }
            ui.Write("\n");
            if (matchList.Count > 0)
            {
                ui.Write("Matching vehicles:\n");
                DisplayVehicleList(matchList);
            }
            else
            {
                ui.WriteWarning("No vehicles matched your search.");
            }
            ui.WaitAndClear();
        }

        private void CreateGarage()
        {
            ui.Clear();
            ui.DisplayInputHeader("Search for vehicles");
            ui.WriteWarning("\nThis will demolish the existing garage!!\n");
            ui.WriteLine("(just press ENTER to keep the garage)");
            int size = ui.GetIntegerFromUser("Number of parking stalls in garage: ", Const.AcceptEmptyString);
            if (size != -1)
            {
                garage = new Garage<Vehicle>(size);
                log.Add("Demolished old garage...");
                string msg = $"New garage created: {garage.Capacity} stalls";
                log.Add(msg);
                ui.WriteSuccess(msg);
                ui.PromptUserForKey();
                ui.Clear();
            }
        }

        private void ParkBoat()
        {
            if (IsPreparedForParking("Boat"))
            {
                var dto = GetParkingParametersFromUser();
                int length = ui.GetIntegerFromUser("Length: ".PadLeft(25), Const.ForbidEmptyString);
                var boat = new Boat(dto.RegNo, dto.Color, dto.NrOfWheels, dto.FuelType, length);
                ParkVehicle(boat, Const.Verbose);
            }
            ui.WaitAndClear();
        }

        private void ParkMotorcycle()
        {
            if (IsPreparedForParking("Motorcycle"))
            {
                var dto = GetParkingParametersFromUser();
                string make = ui.GetTextFromUser("Brand: ".PadLeft(25), Const.ForbidEmptyString);
                var motorcycle = new Motorcycle(dto.RegNo, dto.Color, dto.NrOfWheels, dto.FuelType, make);
                ParkVehicle(motorcycle, Const.Verbose);
            }
            ui.WaitAndClear();
        }

        private void ParkAirplane()
        {
            if (IsPreparedForParking("Airplane"))
            {
                var dto = GetParkingParametersFromUser();
                int engines = ui.GetIntegerFromUser("Number of engines: ".PadLeft(25), Const.ForbidEmptyString);
                var airplane = new Airplane(dto.RegNo, dto.Color, dto.NrOfWheels, dto.FuelType, engines);
                ParkVehicle(airplane, Const.Verbose);
            }
            ui.WaitAndClear();
        }

        private void ParkBus()
        {
            if (IsPreparedForParking("Bus"))
            {
                var dto = GetParkingParametersFromUser();
                int seats = ui.GetIntegerFromUser("Number of seats: ".PadLeft(25), Const.ForbidEmptyString);
                var bus = new Bus(dto.RegNo, dto.Color, dto.NrOfWheels, dto.FuelType, seats);
                ParkVehicle(bus, Const.Verbose);
            }
            ui.WaitAndClear();
        }

        private void ParkCar()
        {
            if (IsPreparedForParking("Car"))
            {
                var dto = GetParkingParametersFromUser();
                string make = ui.GetTextFromUser("Car brand: ".PadLeft(25), Const.ForbidEmptyString);
                var car = new Car(dto.RegNo, dto.Color, dto.NrOfWheels, dto.FuelType, make);
                ParkVehicle(car, Const.Verbose);
            }
            ui.WaitAndClear();
        }

        private bool IsPreparedForParking(string type)
        {
            ui.Clear();
            ui.DisplayInputHeader($"Parking a vehicle - {type}");
            ui.WriteWarning(garage.IsFull() ? "\nThe garage is full!" : "");
            return !garage.IsFull();
        }

        private VehicleDto GetParkingParametersFromUser()
        {
            string regNo;
            while (true)
            {
                regNo = ui.GetTextFromUser("New registration number: ", Const.ForbidEmptyString);
                if (garage.GetVehicle(regNo) != null) // if not already in garage
                {
                    ui.WriteWarning("That registration number is already in use!\n");
                }
                else
                {
                    break;
                }
            }

            string color = ui.GetTextFromUser("Color: ".PadLeft(25), Const.ForbidEmptyString);
            int wheels = ui.GetIntegerFromUser("Nr of wheels: ".PadLeft(25), Const.ForbidEmptyString);
            string fueltype = ui.GetTextFromUser("Fuel type: ".PadLeft(25), Const.ForbidEmptyString);

            var dto = new VehicleDto(regNo, color, wheels, fueltype);
            return dto;
        }

        private ParkingResult ParkVehicle(Vehicle vehicle, bool verbose = false)
        {
            var result = garage.ParkVehicle(vehicle);
            if (result.Success)
            {
                var msg = $"{vehicle.RegNo} is now parked";
                log.Add(msg);
                ui.WriteSuccess((verbose) ? msg : "");
            }
            else
            {
                var msg = $"ERROR - {result.Message}";
                log.Add(msg);
                ui.WriteWarning((verbose) ? msg : "");
            }
            return result;
        }

        private void ParkSomeVehicles()
        {
            ParkVehicle(new Car("ABC123", "Red", 4, "Gasoline", "Nissan"));
            ParkVehicle(new Bus("XYZ456", "Green", 4, "Diesel", 38));
            ParkVehicle(new Airplane("SE-ABCD", "Blue", 3, "JetA1", 4));
            ParkVehicle(new Boat("M/S Lagunia", "Yellow", 0, "Diesel", 12));
            ParkVehicle(new Motorcycle("HOJ345", "Maroon", 2, "Gasoline", "Harley"));
            ParkVehicle(new Car("HUB981", "Yellow", 4, "Gasoline", "BMW"));
            ParkVehicle(new Airplane("JA-37", "Green", 3, "JetA1", 1));
            ParkVehicle(new Bus("BYT256", "White", 4, "Diesel", 28));
            ParkVehicle(new Airplane("SN8", "Silver", 0, "Methane", 3));
            ParkVehicle(new Car("Z80CPU", "Black", 0, "Voltage", "Zilog"));
            ui.Clear();
        }

        private void UnparkVehicle()
        {
            ui.Clear();
            DisplayVehicleList(garage);
            ui.Write("\n");
            if (garage.Count > 0)
            {
                ui.DisplayInputHeader("Unparking a vehicle");
                string regNo = ui.GetTextFromUser("Enter registration number: ");
                if (regNo != "")
                {
                    Vehicle vehicle = garage.UnparkVehicle(regNo);
                    if (vehicle == null)
                    {
                        ui.WriteWarning("That vehicle does not exist!");
                    }
                    else
                    {
                        var text = $"{vehicle.RegNo} was unparked";
                        ui.WriteSuccess(text);
                        log.Add(text);
                    }
                }
            }
            else
            {
                ui.WriteWarning("There are no vehicles to unpark.");
            }
            ui.WaitAndClear();
        }

        private void ListParkedVehicles()
        {
            DisplayGarage(garage);
        }

        private void ListMotorcycles()
        {
            DisplayVehicleList(garage, "Motorcycle");
        }

        private void ListCars()
        {
            DisplayVehicleList(garage, "Car");
        }

        private void ListBusses()
        {
            DisplayVehicleList(garage, "Bus");
        }

        private void ListBoats()
        {
            DisplayVehicleList(garage, "Boat");
        }

        private void ListAirplanes()
        {
            DisplayVehicleList(garage, "Airplane");
        }

        private void RepaintVehicle()
        {
            ui.Clear();
            DisplayVehicleList(garage);
            ui.Write("\n");
            if (garage.Count > 0)
            {
                ui.DisplayInputHeader("Repainting a vehicle");
                string regNo = ui.GetTextFromUser("Enter registration number: ");
                if (regNo != "")
                {
                    Vehicle vehicle = garage.GetVehicle(regNo);
                    if (vehicle == null)
                    {
                        ui.WriteWarning("That vehicle does not exist!");
                    }
                    else
                    {
                        string newColor = ui.GetTextFromUser("Enter name of color to use: ");
                        vehicle.Color = newColor;
                        var text = $"{vehicle.RegNo} was repainted";
                        ui.WriteSuccess(text);
                        log.Add(text);
                    }
                }
            }
            else
            {
                ui.WriteWarning("There are no vehicles to repaint.");
            }
            ui.WaitAndClear();
        }

        private void DisplayGarage(Garage<Vehicle> garage)
        {
            var nr = garage.Count;
            var free = garage.Capacity - garage.Count;
            ui.SetColorNormal();
            ui.Clear();
            DisplayVehicleList(garage);
            ui.Write($"\nThere {(nr != 1 ? "are" : "is")} {nr} parked vehicle{(nr != 1 ? "s" : "")} in the garage. ");
            ui.Write($"{(free == 0 ? "No more" : $"Another {free}")} vehicle{(free > 1 ? "s" : "")} can be parked.\n");
            ui.PromptUserForKey();
            ui.Clear();
        }

        private void DisplayVehicleList(Garage<Vehicle> vehicleList)
        {
            ui.SetColor(Const.listHeaderFG, Const.listHeaderBG);
            ui.WriteLine(" Regnr       Type        Color       Wheels    Fueltype    Extra info        ");
            var sb = new StringBuilder();
            ui.SetColor(Const.listFG, Const.normalBG);
            foreach (var v in vehicleList)
            {
                sb.Append(" ");
                sb.Append(v.RegNo.PadRight(12, ' '));
                sb.Append(v.ToString().Split('.').Last().PadRight(12, ' '));
                sb.Append(v.Color.PadRight(12, ' '));
                sb.Append(v.NrOfWheels.ToString().PadRight(10, ' '));
                sb.Append(v.FuelType.PadRight(12, ' '));
                sb.Append(v.GetDescription().PadRight(12, ' '));
                ui.WriteLine(sb.ToString());
                sb.Clear();
            }
            ui.SetColorNormal();
        }

        private void DisplayVehicleList(Garage<Vehicle> vehicleList, string typeName)
        {
            var list = new Garage<Vehicle>(vehicleList.Count);
            foreach (var v in vehicleList)
            {
                if (v.GetType().Name == typeName)
                {
                    list.ParkVehicle(v);
                }
            }
            ui.Clear();
            DisplayVehicleList(list);
            if (list.Count > 0)
            {
                ui.Write($"\nA total number of {list.Count} vehicle{(list.Count != 1 ? "s" : "")} of the type {typeName}.");
            }
            else
            {
                ui.WriteWarning($"\nThere are no vehicles of the type {typeName} in the garage.");
            }
            ui.PromptUserForKey();
            ui.Clear();
        }
    }
}
