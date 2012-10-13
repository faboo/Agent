using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Shell;
using System.Windows.Threading;

namespace Agent {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp {
        [STAThread]
        public static void Main() {
            if (SingleInstance<App>.InitializeAsFirstInstance("Pad")) {
                var application = new App();
                application.Init();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        public void Init() {
            this.InitializeComponent();

            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        public bool SignalExternalCommandLineArgs(IList<string> args) {
            if(args.Contains("exit")) {
                MainWindow.Close();
            }
            else {
                MainWindow.Show();
                MainWindow.Activate();
                MainWindow.Focus();
            }
            return true;
        }


        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            if(!(e.Exception is ArgumentOutOfRangeException))
                MessageBox.Show("An error occurred: " + e.Exception.ToString() + "\n", "Agent Error");
            e.Handled = true;
        }
    }
}
