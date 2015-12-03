using System;
using System.ServiceProcess;
using System.Threading;

namespace Alfred.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Environment.UserInteractive)
            {
                ServiceBase.Run(new Service());
            }
            else
            {

                try
                {
                    Server.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format(
                        "Message : {0}. Inner Exception : {1}",
                        e.Message,
                        e.InnerException != null ? e.InnerException.Message : string.Empty));
                }
                Console.Read();
            }
        }
    }

    class Service : ServiceBase
    {
        public Service()
        {
            var thread = new Thread(Actions);
            thread.Start();
        }

        public void Actions()
        {
            Server.Start();
        }
    }
}
