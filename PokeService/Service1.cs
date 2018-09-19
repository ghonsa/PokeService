using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace PokeService
{
    public partial class Service1 : ServiceBase
    {
        internal static Listener listenerClass;
        Thread doBroadcast_thread;
        public Service1()
        {
            InitializeComponent();
            listenerClass = new Listener();
            doBroadcast_thread = new Thread(new ThreadStart(listenerClass.DoBroadcast));
        }

        protected override void OnStart(string[] args)
        {
            doBroadcast_thread.Start();
        }

        protected override void OnStop()
        {
            doBroadcast_thread.Abort();
        }

    }
}
