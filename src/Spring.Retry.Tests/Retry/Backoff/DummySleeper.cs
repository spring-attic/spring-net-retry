// -----------------------------------------------------------------------
// <copyright file="DummySleeper.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Spring.Retry.Retry.Backoff;

namespace Spring.Retry.Tests.Retry.Backoff
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Simple <see cref="ISleeper"/> implementation that just waits on a local object.
    /// </summary>
    /// <author>Dave Syer</author>
    /// <author>Joe Fitzgerald (.NET)</author>
    public class DummySleeper : ISleeper
    {
        private List<long> backOffs = new List<long>();

        /**
         * Public getter for the long.
         * @return the lastBackOff
         */
        public long GetLastBackOff()
        {
            return backOffs[backOffs.Count - 1];
        }

        public long[] GetBackOffs()
        {
            return backOffs.ToArray();

            // var result = new long[backOffs.Count];
            // var i = 0;
            // for (Iterator<Long> iterator = backOffs.iterator(); iterator.hasNext(); )
            // {
            //     Long value = iterator.next();
            //     result[i++] = value.longValue();
            // }
               
            // return result;
        }

        public void Sleep(long backOffPeriod)
        {
            this.backOffs.Add(backOffPeriod);
        }
    }
}
