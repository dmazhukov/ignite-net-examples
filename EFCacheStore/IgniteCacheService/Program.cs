using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.Ignite.Core;

namespace IgniteCacheService
{
    class Program
    {
        static void Main(string[] args)
        {
            var cfg = new IgniteConfiguration
            {
                
            };
            var ignite = Ignition.Start();
            Console.ReadLine();
        }
    }
}
