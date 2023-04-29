using System;
using System.Collections.Generic;
using System.Text;

namespace Lab07
{
    class Client
    {
        public event EventHandler<ProcEventArgs> request;
        Server server;
        public Client(Server server)
        {
            this.server = server;
            this.request += server.proc;
        }
        protected virtual void OnProc(ProcEventArgs e)
        {
            EventHandler<ProcEventArgs> handler = request;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        public void Run(int id)
        {
            ProcEventArgs e = new ProcEventArgs();
            e.id = id;
            OnProc(e);
        }

    }
}
