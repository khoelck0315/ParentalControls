using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ControlServiceLibrary
{
    public class User
    {
        public User(string friendlyName, string userName, int dailyTime, string endTime)
        {
            this.DisplayName = friendlyName;
            this.UserName = userName;
            this.DailyTime = dailyTime;
            this.EndTime = endTime;
        }


        public void HandleElapsedTime()
        {
            // Has a full day passed since last login? If so, reset the elapsed time
            // TODO handle the new years edge case 
            if (this.LogonTime.DayOfYear < DateTime.Now.DayOfYear)
            {
                this.ElapsedTime = 0;
            }
            else
            {
                this.ElapsedTime++;
            }
        }

        public void SetLogonTime()
        {
            this.LogonTime = DateTime.Now;
        }

        public DateTime LogonTime
        {
            get => _logonTime;
            set => _logonTime = value;
        }
        public int ElapsedTime
        {
            get => _elapsedTime;
            set => _elapsedTime = value;
        }
        public string DisplayName
        {
            get => _displayName;
            set => _displayName = value;
        }
        public string UserName
        {
            get => _userName;
            set => _userName = value;
        }
        public int DailyTime
        {
            get => _dailyTime;
            set => _dailyTime = value;
        }
        //Best way to handle this, DateTime object or parse from string? Begin with string
        public string EndTime
        {
            get => _endTime;
            set => _endTime = value;
        }
        private string _displayName;
        private string _userName;
        private int _dailyTime;
        private string _endTime;
        private DateTime _logonTime;
        private int _elapsedTime;
    }
}
