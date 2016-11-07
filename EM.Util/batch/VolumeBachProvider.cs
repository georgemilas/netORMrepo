using System;
using System.Collections.Generic;
using System.Text;

namespace EM.Batch
{
    /// <summary>
    /// flush batch collection when number of items in the collection reaches a specified volume
    /// </summary>
    public class VolumeBatchProvider: BatchProvider
    {
        public int volume;
        public VolumeBatchProvider(int volume): base()
        {
            if (volume > 1)
            {
                this.volume = volume;
            }
            else
            {
                throw new IndexOutOfRangeException("A volume must be any number greater then 1");
            }
        }

        public override bool couldFlush
        {
            get { return this.batchContent.Count >= this.volume; }
        }
       
    }
}
