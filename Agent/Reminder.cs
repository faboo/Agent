using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;

namespace Agent {
    public class Reminder : Freezable {
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public Frequency Frequency { get; set; }

        private Reminders Reminders;
        private Timer Timer;

        public Reminder(Reminders reminders) {
            Reminders = reminders;
        }

        protected override Freezable CreateInstanceCore() {
            return new Reminder(Reminders);
        }

        public void Init() {
            try {
                Timer = new Timer(OnTime, null, Time - DateTime.Now, TimeSpan.FromDays(1));
            }
            catch {
                Show();
            }
        }

        void OnTime(object state) {
            Timer.Dispose();
            Show();
        }

        void Show() {
            if(String.IsNullOrWhiteSpace(Text))
                MessageBox.Show(Name, "Agent Message", MessageBoxButton.OK);
            else
                MessageBox.Show(Text, String.Format("{0} - Agent Message", Name), MessageBoxButton.OK);
            Reminders.Remove(this);
        }
    }
}
