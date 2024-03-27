using System;
using System.Threading.Tasks;
using mqtt_broker;

class Program
{
    static void Main(string[] args)
    {
        Task.Run(async () =>
        {
            await DataLogic.Server(); // Call the static method using the class name
        }).GetAwaiter().GetResult(); // Ensure the main method does not exit prematurely
    }
}