using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PRG281_Project
{
    internal class Program
    {
        delegate void MachineAlertHandler(Machine machine, string message);

        static List<Machine> machines = new List<Machine>();
        static int nextId = 1;
        public enum MachineStatus
        {
            Operational,
            Failed
        }

        static void Main(string[] args)
        {
            MonitoringSystem monitor = new MonitoringSystem(machines);
            monitor.Start();


            string input;
            do
            {
                Console.Clear();
                Console.WriteLine("1. View All Machines");
                Console.WriteLine("2. View Machine Details");
                Console.WriteLine("3. Add Machine");
                Console.WriteLine("4. Remove Machine");
                Console.WriteLine("5. Repair All Machines");
                Console.WriteLine("6. Perform Machine Operation");
                Console.WriteLine("7. Exit");

                input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        viewAllMachines();
                        break;
                    case "2":
                        viewMachineDetails();
                        break;
                    case "3":
                        AddMachine();
                        break;
                    case "4":
                        removeMachine();
                        break;
                    case "5":
                        RepairAllMachines();
                        break;
                    case "6":
                        PerformMachineOperation();
                        break;
                    case "7":
                        monitor.Stop();
                        return;
                    default:
                        Console.WriteLine("\nInvalid option. Please enter a number between 1 and 7.");
                        Console.WriteLine("Press any key to return to the main menu...");
                        Console.ReadKey();
                        break;
                }

            } while (true);
        }
        static Machine FindMachine(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new Exception("Machine ID cannot be empty.");
            }

            Machine machine = machines.Find(m => string.Equals(m.Id, id, StringComparison.OrdinalIgnoreCase));

            if (machine == null)
            {
                throw new MachineNotFoundException($"No machine found with ID '{id}'.");
            }

            return machine;
        }
        public static void removeMachine()
        {
            Console.WriteLine("Enter Machine ID: ");
            string id = Console.ReadLine();

            try
            {
                Machine machine = FindMachine(id);

                lock (machines)
                {
                    machines.Remove(machine);
                }

                Console.WriteLine($"\n{machine.Name}, id: {machine.Id} was removed.");
            }
            catch (MachineNotFoundException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nSomething went wrong: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nPress any key to return to the main menu...");
                Console.ReadKey();
            }
        }
        public static void viewMachineDetails()
        {
            Console.WriteLine("Enter Machine ID: ");
            string id = Console.ReadLine();

            try
            {
                Machine machine = FindMachine(id);
                Console.WriteLine();
                machine.DisplayInfo();
            }
            catch (MachineNotFoundException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nSomething went wrong: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nPress any key to return to the main menu...");
                Console.ReadKey();
            }
        }
        public static void viewAllMachines()
        {
            if (machines.Count == 0)
            {
                Console.WriteLine("No machines to display.");
            }
            else
            {
                foreach (Machine machine in machines)
                {
                    machine.DisplayInfo();
                    Console.WriteLine("============");
                }
            }
            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey();
        }
        public static void PerformMachineOperation()
        {
            Console.WriteLine("Enter Machine ID: ");
            string id = Console.ReadLine();

            try
            {
                Machine machine = FindMachine(id);

                if (machine.Status == MachineStatus.Failed)
                {
                    throw new Exception($"{machine.Name} has failed and cannot perform operations until repaired.");
                }

                Console.WriteLine();
                machine.PerformOperation();
            }
            catch (MachineNotFoundException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nSomething went wrong: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nPress any key to return to the main menu...");
                Console.ReadKey();
            }
        }
        public static void AddMachine()
        {
            string id = $"M{nextId:000}";
            Machine machine;

            try
            {
                Console.WriteLine("Select machine type:");
                Console.WriteLine("1. Conveyor Belt");
                Console.WriteLine("2. Robotic Arm");
                Console.WriteLine("3. Forklift");
                Console.WriteLine("4. Sorting Machine");

                Console.Write("Enter Selection: ");
                string input = Console.ReadLine();

                Console.Write("Enter Machine Name: ");
                string machineName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(machineName))
                {
                    throw new Exception("Machine name cannot be empty.");
                }

                switch (input)
                {
                    case "1":
                        machine = new ConveyorBelt(id, machineName);
                        break;

                    case "2":
                        machine = new RoboticArm(id, machineName);
                        break;

                    case "3":
                        machine = new Forklift(id, machineName);
                        break;
                    case "4":
                        machine = new SortingMachine(id, machineName);
                        break;

                    default:
                        throw new Exception("Invalid machine type selected.");
                }

                machine.MachineFailed += OnMachineFailure;
                machine.MachineRecovered += OnMachineRecovered;

                lock (machines)
                {
                    machines.Add(machine);
                }
                nextId++;

                Console.WriteLine("\nMachine added successfully!\n");
                machine.DisplayInfo();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nCould not add machine: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nPress any key to return to the main menu...");
                Console.ReadKey();
            }
        }
        public static void RepairAllMachines()
        {
            Console.Clear();
            Console.WriteLine("Dispatching maintenance team...\n");

            Console.Write("[");
            for (int i = 0; i < 30; i++)
            {
                Console.Write("=");
                Thread.Sleep(100);
            }
            Console.WriteLine("]");

            int repaired = 0;

            foreach (Machine machine in machines)
            {
                if (machine.Status == MachineStatus.Failed)
                {
                    machine.Repair();
                    repaired++;
                }
            }

            Console.WriteLine();

            if (repaired == 0)
            {
                Console.WriteLine("No machines required repairs.");
            }
            else
            {
                Console.WriteLine($"{repaired} machine(s) repaired successfully.");
            }

            Console.ResetColor();
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
        class MachineNotFoundException : Exception
        {
            public MachineNotFoundException(string message) : base(message)
            {
            }
        }
        abstract class Machine
        {
            private string id;
            private string name;
            private MachineStatus status;
            private string type;

            public string Id { get { return id; } set { id = value; } }
            public string Name { get { return name; } set { name = value; } }
            public MachineStatus Status { get { return status; } set { status = value; } }
            public string Type { get { return type; } set { type = value; } }

            protected Machine(string id, string name)
            {
                Id = id;
                Name = name;
                Status = MachineStatus.Operational;
            }
            public abstract void PerformOperation();
            public virtual void DisplayInfo()
            {
                Console.WriteLine($"ID: {Id}");
                Console.WriteLine($"Name: {Name}");
                Console.WriteLine($"Type: {Type}");
                Console.WriteLine($"Status: {Status}");
            }
            public event MachineAlertHandler MachineFailed;
            public event MachineAlertHandler MachineRecovered;

            protected void RaiseFailure(string message)
            {
                MachineFailed?.Invoke(this, message);
            }

            protected void RaiseRecovery(string message)
            {
                MachineRecovered?.Invoke(this, message);
            }

            public void Fail()
            {
                if (Status != MachineStatus.Failed)
                {
                    Status = MachineStatus.Failed;
                    RaiseFailure($"{Name} has failed.");
                }
            }

            public void Repair()
            {
                if (Status == MachineStatus.Failed)
                {
                    Status = MachineStatus.Operational;
                    RaiseRecovery($"{Name} has been repaired.");
                }
            }
        }
        class ConveyorBelt : Machine
        {
            public ConveyorBelt(string id, string name) : base(id, name)
            {
                this.Type = "Conveyor Belt";
            }
            public override void PerformOperation()
            {
                Console.WriteLine($"{Name} is conveying items.");
            }
        }
        class RoboticArm : Machine
        {
            public RoboticArm(string id, string name) : base(id, name)
            {
                this.Type = "Robotic Arm";
            }
            public override void PerformOperation()
            {
                Console.WriteLine($"{Name} is grabbing and moving items.");
            }
        }
        class Forklift : Machine
        {
            public Forklift(string id, string name) : base(id, name)
            {
                this.Type = "Forklift";
            }
            public override void PerformOperation()
            {
                Console.WriteLine($"{Name} is forking and lifting items.");
            }
        }
        class SortingMachine : Machine
        {
            public SortingMachine(string id, string name) : base(id, name)
            {
                this.Type = "Sorting Machine";
            }
            public override void PerformOperation()
            {
                Console.WriteLine($"{Name} is sorting items.");
            }
        }
        class MonitoringSystem
        {
            List<Machine> machines;
            Thread monitorThread;
            volatile bool running = true;

            Random random = new Random();

            public MonitoringSystem(List<Machine> machineList)
            {
                machines = machineList;
            }

            public void Start()
            {
                monitorThread = new Thread(MonitorMachines);
                monitorThread.IsBackground = true;
                monitorThread.Start();
            }

            public void Stop()
            {
                running = false;
            }

            private void MonitorMachines()
            {
                while (running)
                {
                    lock (machines)
                    {
                        foreach (Machine machine in machines)
                        {
                            if (machine.Status == MachineStatus.Operational)
                            {
                                if (random.Next(100) < 10)
                                {
                                    machine.Fail();
                                }
                            }
                        }
                    }
                    Thread.Sleep(5000);
                }
            }
        }

        static void OnMachineFailure(Machine machine, string message)
        {
            Console.WriteLine();
            Console.WriteLine("==================================");
            Console.WriteLine("ALERT!");
            Console.WriteLine(message);
            Console.WriteLine($"Machine ID: {machine.Id}");
            Console.WriteLine($"Machine Type: {machine.Type}");
            Console.WriteLine("==================================");
        }

        static void OnMachineRecovered(Machine machine, string message)
        {
            Console.WriteLine(message);
        }
    }
}
