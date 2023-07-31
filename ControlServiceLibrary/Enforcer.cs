using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Runtime.InteropServices;
using Continuous.Management;
using System.Runtime.CompilerServices;
using Continuous.User.Users;
using Continuous.User;
using Microsoft.Data.Sqlite;

namespace ControlServiceLibrary
{
    public static class Enforcer
    {
        public static Dictionary<string, User> GetConfiguredUsers()
        {
            string folder = Path.GetDirectoryName("C:\\.NET Projects\\ParentalControls\\ControlService") ?? throw new Exception();
            string filePath = Path.Combine(folder, DatabaseFile);

            // Make sure the file exists
            if (File.Exists(filePath))
            {
                using (var connection = new SqliteConnection("Data Source=userdata.db"))
                {
                    SQLitePCL.Batteries.Init();
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText =
                    @"
                        SELECT * 
                        FROM users
                    ";
                    // If needed
                    //command.Parameters.AddWithValue("$id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var name = reader.GetString(0);

                            Console.WriteLine($"Hello, {name}!");
                        }
                    }
                }
                return new Dictionary<string, User>();
            }
            else throw new Exception();
        }
        public static string GetCurrentUser()
        {
            string fqName = WindowsIdentity.GetCurrent().Name;
            int i = fqName.IndexOf('\\');
            return fqName.Substring(i+1, fqName.Length - i - 1);
        }
        public static bool EnforceControls(User user)
        {
            if (CheckLogoffTime(user) || CheckElapsedTime(user))
            {
                LogoffUser();
                return true;
            }
            else return false;
        }
        private static bool CheckLogoffTime(User user)
        {
            DateTime setting = DateTime.Parse(user.EndTime);
            if (DateTime.Now > setting) return true;
            else return false;
        }            
        private static bool CheckElapsedTime(User user) 
        {
            if (user.ElapsedTime > user.DailyTime) return true;
            else return false;
        }
        private static void LogoffUser()
        {
            //Set user disabled? See readme
            //var factory = new ContinuousManagementFactory();
            //var userShell = factory.UserShell();
            // userShell.SetAccountDisabled(user.UserName, true);            
            //Logoff user
            ExitWindowsEx(4, 0);
        }

        [DllImport("user32.dll")]
        public static extern int ExitWindowsEx(int uFlags, int dwReason);

        private const string DatabaseFile = "userdata.db";
    }
}
