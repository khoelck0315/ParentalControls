using ControlServiceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ControlService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        
        static void Main()
        {
            // Test block
            
            try
            {
                Dictionary<string, User> testusers = new Dictionary<string, User>(); 
                testusers = Enforcer.GetConfiguredUsers();
                foreach(string val in testusers.Keys) {
                    Console.WriteLine(testusers[val]);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ControlService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
