using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Configuration.Install;
using System.Diagnostics;


namespace PokeService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        static void Main(string[] args)
        {
            string args0 = "";
            string args1 = "";
            string pfPath = Environment.GetEnvironmentVariable("ProgramFiles");

            if (args.Length > 0)
                args0 = args[0];

            if (args0.ToLower() == "-install")
            {
                //MessageBox.Show("PokeService Install", "Poke Service");
                TransactedInstaller ti = new TransactedInstaller();
                Installer1 pi = new Installer1();
                ti.Installers.Add(pi);

                InstallContext ctx = new InstallContext();
                ti.Context = ctx;
                ctx.Parameters.Add("assemblypath", pfPath + "\\Honsa Consulting\\PokeSrvice.exe");
                ti.Install(new Hashtable());
            }

            else if (args0.ToLower() == "-uninstall")
            {
                TransactedInstaller ti = new TransactedInstaller();
                Installer1 pi = new Installer1();
                ti.Installers.Add(pi);

                InstallContext ctx = new InstallContext();
                ctx.Parameters.Add("assemblypath", pfPath + "\\Honsa Consulting\\PokeSrvice.exe");
                ti.Uninstall(null);
            }
            else if (args0 != "")
            {

             
            }

            // start up the service
            System.ServiceProcess.ServiceBase[] ServiceToRun;
            ServiceToRun = new System.ServiceProcess.ServiceBase[] { new Service1() };
            System.ServiceProcess.ServiceBase.Run(ServiceToRun);

        }
    }
}
