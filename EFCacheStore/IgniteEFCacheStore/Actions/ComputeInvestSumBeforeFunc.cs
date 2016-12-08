using System;
using Apache.Ignite.Core.Compute;

namespace IgniteEFCacheStore.Actions
{
    public class ComputeInvestSumBeforeFunc:IComputeFunc<decimal>
    {
        public decimal Invoke()
        {
            throw new NotImplementedException();
        }
    }
}