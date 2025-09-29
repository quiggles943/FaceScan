using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceScan
{
    public class FaceScanFactoryOptionsBuilder
    {
        /// <summary>
        /// Gets or sets the factory used to create logger instances for this component.
        /// </summary>
        /// <remarks>Assign a custom <see cref="ILoggerFactory"/> to control logging behavior. By default,
        /// logging is disabled unless a non-null factory is provided.</remarks>
        internal ILoggerFactory LoggerFactory { get; set; } = NullLoggerFactory.Instance;
        /// <summary>
        /// The minimum similarity score required to consider two items a positive match.
        /// </summary>
        /// <remarks>This property defines the threshold above which items are treated as positive
        /// matches. A higher value increases the strictness of matching. If null, the default threshold is
        /// used.</remarks>
        internal float? PositiveMatchThreashold { get; set; }
        /// <summary>
        /// The minimum similarity score required to consider two items a potential match.
        /// </summary>
        /// <remarks>This property defines the threshold above which items are treated as potential
        /// matches. A higher value increases the strictness of matching. If null, the default threshold is
        /// used.</remarks>
        internal float? PotentialMatchThreshold { get; set; }

        /// <summary>
        /// Indicates whether the search operation should stop after the first positive match is
        /// found.
        /// </summary>
        /// <remarks>When <see langword="false"/>, the facescan will continue searching for additional
        /// matches after a positive result is found. By default, the search stops after the first positive
        /// match.</remarks>
        internal bool StopSearchAfterPositiveMatch { get; set; } = true;

        /// <summary>
        /// Sets the threshold value used to determine a positive face match.
        /// </summary>
        /// <param name="threshold">The minimum similarity score, as a floating-point value, required to consider two faces a positive match.
        /// Must be a valid floating-point number.</param>
        /// <returns>The current <see cref="FaceScanFactoryOptionsBuilder"/> instance with the updated positive match threshold.</returns>
        public FaceScanFactoryOptionsBuilder SetPositiveMatchThreshold(float threshold)
        {
            ArgumentNullException.ThrowIfNull(threshold);
            PositiveMatchThreashold = threshold;
            return this;
        }

        /// <summary>
        /// Sets the threshold value used to determine whether two face scans are considered a potential match.
        /// </summary>
        /// <param name="threshold">The threshold value for potential face scan matches. Must be a valid floating-point number.</param>
        /// <returns>The current <see cref="FaceScanFactoryOptionsBuilder"/> instance with the updated threshold value.</returns>
        public FaceScanFactoryOptionsBuilder SetPotentialMatchThreshold(float threshold)
        {
            ArgumentNullException.ThrowIfNull(threshold);
            PotentialMatchThreshold = threshold;
            return this;
        }

        /// <summary>
        /// Sets the logger factory to be used for diagnostic and operational messages.
        /// </summary>
        /// <param name="logger">The logger factory instance to create loggers. Cannot be null.</param>
        /// <returns>The current <see cref="FaceScanFactoryOptionsBuilder"/> instance with the updated logger.</returns>
        public FaceScanFactoryOptionsBuilder UseLoggerFactory(ILoggerFactory loggerFactory)
        {
            ArgumentNullException.ThrowIfNull(loggerFactory);
            LoggerFactory = loggerFactory;
            return this;
        }

        /// <summary>
        /// Configures whether multiple positive face matches are allowed during the scan process.
        /// </summary>
        /// <remarks>When multiple positive matches are allowed, the scan will continue after the first
        /// positive match is found, potentially reporting additional matches. If not allowed, the scan stops after the
        /// first positive match.</remarks>
        /// <param name="allowMultiplePositiveMatches">true to allow multiple positive matches to be reported; otherwise, false. The default is false.</param>
        /// <returns>The current instance of the FaceScanFactoryOptionsBuilder for method chaining.</returns>
        public FaceScanFactoryOptionsBuilder AllowMultiplePositiveMatches(bool allowMultiplePositiveMatches = false)
        {
            StopSearchAfterPositiveMatch = !allowMultiplePositiveMatches;
            return this;
        }
    }
}
