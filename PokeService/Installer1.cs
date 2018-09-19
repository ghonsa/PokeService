using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;


namespace PokeService
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        internal ServiceInstaller serviceInstaller;
        internal ServiceProcessInstaller processInstaller;

        public Installer1()
        {
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            // Service will run under system account
            processInstaller.Account = ServiceAccount.LocalSystem;


            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "PokeService";
            serviceInstaller.Description = "Poke Service";

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }

       
    }
}
