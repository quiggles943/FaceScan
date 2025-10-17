using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceScan.Enums
{
    public enum AggregateFaceModelUpdateType
    {
        /// <summary>
        /// Add new vectors to the aggregate model
        /// </summary>
        Add,
        /// <summary>
        /// Remove vectors from the aggregate model
        /// </summary>
        Remove,
    }
}
