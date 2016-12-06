using System;
using Apache.Ignite.Core.Compute;

namespace IgniteEFCacheStore.Actions
{
    public class PrintMessageAction : IComputeAction
    {
        public void Invoke()
        {
            Console.WriteLine(Message);
        }

        public string Message { get; set; }
    }
}