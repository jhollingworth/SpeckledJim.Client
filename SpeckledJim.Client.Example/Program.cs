using System;
using System.Threading;

namespace SpeckledJim.Client.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new SpeckledJimClient(new Uri("http://localhost:3000"))) 
            {
                new Thread(() => client.Connect("ping")).Start();

                
                Thread.CurrentThread.Join();
            }
        }
    }
}
