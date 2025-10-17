using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceScan.Interfaces
{
    public interface IFaceScanFactory
    {
        /// <summary>
        /// Creates and configures a new instance of the FaceScanGenerator using the current logger factory and options.
        /// </summary>
        /// <remarks>If no options are set, the FaceScanGenerator is created with default thresholds.</remarks>
        /// <returns>A FaceScanGenerator instance configured with the current logger and any specified options.</returns>
        public IFaceScanGenerator CreateFaceScanGenerator();
        /// <summary>
        /// Creates a new instance of the FaceScanGenerator with custom configuration options.
        /// </summary>
        /// <remarks>Use this method to customize thresholds or other settings for the FaceScanGenerator
        /// before instantiation.</remarks>
        /// <param name="configure">A delegate that configures the FaceScanFactoryOptionsBuilder to specify custom options for the
        /// FaceScanGenerator. Cannot be null.</param>
        /// <returns>A FaceScanGenerator instance configured with the specified options.</returns>
        public IFaceScanGenerator CreateFaceScanGenerator(Action<FaceScanFactoryOptionsBuilder> configure);

        public IFaceScanComparator CreateFaceScanComparator();

        public IFaceScanComparator CreateFaceScanComparator(Action<FaceScanFactoryOptionsBuilder> configure);
    }
}
