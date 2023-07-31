using ControlServiceLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace ControlService
{
    public partial class ControlService : ServiceBase
    {
        public ControlService()
        {
            InitializeComponent();
            CanHandleSessionChangeEvent = true;

            /*string source = "ControlService";
            string log = "Application";
            this.eventLog = new EventLog();
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(
                    source, log);
            }
            eventLog.Source = source;
            eventLog.Log = log;
            */
            
        }

        protected override void OnStart(string[] args)
        {
            this.eventLog.WriteEntry("Parental Controls service has started.");

            //Read config
            this.UserList = Enforcer.GetConfiguredUsers();

            //Do polling here
            Timer timer = new Timer();
            timer.Interval = 60000; // 60 seconds
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        protected override void OnStop()
        {
            this.eventLog.WriteEntry("Parental Controls service has stopped.");
        }

        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            // Get logged in user
            string currentUser = Enforcer.GetCurrentUser();

            // Check to see if logged in user is managed, then make sure they are allowed to be logged in.
            // If not, handle it
            if (this.UserList.TryGetValue(currentUser, out User user))
            {
                // Handle the elapsed time.  Reset it if necessary, or increment it
                user.HandleElapsedTime();

                // Check controls, log off user if necessary
                _ = Enforcer.EnforceControls(user);
            }
            
            // If the OnSessionChange event handler can't handle preventing logins, add functionality to periodically check for disabled accounts and if
            // they need to be re-enabled here.

            this.eventLog.WriteEntry("Polling activity", EventLogEntryType.Information, eventID++);
        }
        //Intercept the logon process, check to see if user is authorized
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            string currentUser = Enforcer.GetCurrentUser();
            switch (changeDescription.Reason)
            {
                case SessionChangeReason.SessionLogon:
                    if (this.UserList.TryGetValue(currentUser, out User user))
                    {
                        // On user login, check to see if it is allowed and take action accordingly
                        bool userLoggedOff = Enforcer.EnforceControls(user);
                        // Check to see if user was allowed to log in.  If they were, begin the login time.
                        if (!userLoggedOff)
                        {
                            user.SetLogonTime();
                            this.eventLog.WriteEntry("User has allowed time remaining.  Logging in, setting login time.", EventLogEntryType.Information, eventID++);
                        }
                        else
                        {
                            this.eventLog.WriteEntry("User has exceeded allowed time for today.  Logging off.", EventLogEntryType.Information, eventID++);
                        }
                    }
                    break;
                case SessionChangeReason.SessionLogoff:
                    break;
                default:
                    this.eventLog.WriteEntry("No matching parameters for session change event handler.  No action taken.", EventLogEntryType.Information, eventID++);
                    break;
            }
        }

        protected override void OnShutdown()
        {
            // Find storage for the session data
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            return base.OnPowerEvent(powerStatus);
        }
        //Other over rideable methods:
        /*
         * protected override void OnPause
         * protected override void OnContinue
         * protected override void OnShutdown
         * protected override void OnPowerEvent
         */

        private static string CheckDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }

        private Dictionary<string,User> UserList { get; set; }   
        private int eventID = 1;
        private readonly EventLog eventLog;

        // Proper storage of data: https://www.codeproject.com/Tips/370232/Where-should-I-store-my-data
        public static Guid AppGuid
        {
            get
            {
                Assembly asm = Assembly.GetEntryAssembly();
                object[] attr = (asm.GetCustomAttributes(typeof(GuidAttribute), true));
                return new Guid((attr[0] as GuidAttribute).Value);
            }
        }
        // C:\ProgramData\...
        public static string AllUsersDataFolder
        {
            get
            {
                Guid appGuid = AppGuid;
                string folderBase = Environment.GetFolderPath
                                    (Environment.SpecialFolder.CommonApplicationData);
                string dir = string.Format(@"{0}\{1}\",
                             folderBase, appGuid.ToString("B").ToUpper());
                return CheckDir(dir);
            }
        }
    }
}
