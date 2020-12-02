using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace spotifyAdBlock
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Spotify Ad Blocker";
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.SetWindowSize(55, 15);
            Console.Clear();

            if (IsAdministrator())
            {
                Timer timer = new Timer(TimerCallback, null, 0, 500);
                Console.In.ReadToEnd();
            }
            else
            {
                Console.WriteLine("Please Run as Adminstrator.");
                Console.ReadLine();
            }
        }
        static int say = 0;
        private static void TimerCallback(Object o)
        {
            Process[] control = Process.GetProcessesByName("spotify");
            if (control.Length > 0)
            {
                Console.Write("\rListening Spotify: " + DateTime.Now.ToShortTimeString());
                bool titleName = true;
                foreach (var item in control)
                {
                    if (!String.IsNullOrEmpty(item.MainWindowTitle))
                    {
                        if (item.MainWindowTitle.Trim().ToLower().Contains("spotify"))
                        {
                            if (titleName && say <= 0)
                            {
                                RestartService("Audiosrv");
                                Console.Write("\rSpotify Blocked Ad: " + item.MainWindowTitle + " " + DateTime.Now.ToShortTimeString()+"\n");
                                titleName = false;
                                say++;
                            }
                        }
                        else if(!item.MainWindowTitle.Contains(" - "))
                        {
                            RestartService("Audiosrv");
                            Console.Write("\rSpotify Blocked Ad: " + item.MainWindowTitle +" "+ DateTime.Now.ToShortTimeString()+"\n");
                            titleName = true;
                            say = 0;
                        }
                        else
                        {
                            titleName = true;
                            say = 0;
                        }
                    }
                }
            }
            else
            {
                Console.Write("\rNot Listening Spotify: " + DateTime.Now.ToShortTimeString());
            }

            GC.Collect();
        }
        public static void RestartService(string serviceName)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);
            }
            catch
            {
            }
        }
        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                    .IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
