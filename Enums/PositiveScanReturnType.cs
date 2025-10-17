using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceScan.Enums
{
    /// <summary>
    /// Defines the strategy for how to return positive matches during a face scan operation.
    /// </summary>
    public enum PositiveScanReturnType
    {
        /// <summary>
        /// Returns the first positive match found during the scan. The scan will end when the first match is found.
        /// </summary>
        ReturnFirstMatch,
        /// <summary>
        /// Returns the best match found during the scan, based on the highest similarity score.
        /// </summary>
        ReturnBestMatch
    }
}
