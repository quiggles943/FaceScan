using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceScan
{
    public class FaceScanFactory
    {
        public FaceScanFactoryOptionsBuilder? Options { get; private set; }
        private ILogger Logger { get; set; } = new NullLogger<FaceScanFactory>();
        private ILoggerFactory LoggerFactory { get; set; } = NullLoggerFactory.Instance;
        /// <summary>
        /// Initializes a new instance of the FaceScanFactory class with the default configuration.
        /// </summary>
        public FaceScanFactory()
        {

        }

        /// <summary>
        /// Initializes a new instance of the FaceScanFactory class using the specified configuration action.
        /// </summary>
        /// <remarks>Use this constructor to customize the factory's options before creating face scan
        /// instances. The provided configuration action is invoked with a new FaceScanFactoryOptionsBuilder, allowing
        /// you to set options such as logging or other factory behaviors.</remarks>
        /// <param name="configure">A delegate that configures the FaceScanFactoryOptionsBuilder. Cannot be null.</param>
        public FaceScanFactory(Action<FaceScanFactoryOptionsBuilder> configure)
        {
            var options = new FaceScanFactoryOptionsBuilder();
            configure(options);
            Options = options;
            SetLoggerFactory(options.LoggerFactory);
        }

        /// <summary>
        /// Sets the logger instance to be used for logging operations.
        /// </summary>
        /// <param name="logger">The logger to use for logging. Cannot be null.</param>
        private void SetLogger(ILogger? logger)
        {
            ArgumentNullException.ThrowIfNull(logger);
            Logger = logger;
        }

        /// <summary>
        /// Sets the logger factory to be used for creating loggers within the factory and the corresponding objects.
        /// </summary>
        /// <param name="loggerFactory">The logger factory instance to use for creating loggers. Cannot be null.</param>
        private void SetLoggerFactory(ILoggerFactory? loggerFactory)
        {
            ArgumentNullException.ThrowIfNull(loggerFactory);
            LoggerFactory = loggerFactory;
            SetLogger(LoggerFactory.CreateLogger(typeof(FaceScanFactory)));
        }

        /// <summary>
        /// Creates and configures a new instance of the FaceScanGenerator using the current logger factory and options.
        /// </summary>
        /// <remarks>If no options are set, the FaceScanGenerator is created with default thresholds.</remarks>
        /// <returns>A FaceScanGenerator instance configured with the current logger and any specified options.</returns>
        public FaceScanGenerator CreateFaceScanGenerator()
        {
            Logger.LogDebug("Creating FaceScanGenerator with pre-configured options.");
            var builder = new FaceScanGenerator.Builder();
            builder.WithLogger(LoggerFactory.CreateLogger(typeof(FaceScanGenerator)));
            if (Options == null)
                return builder.Build();
            return builder.Build();
        }

        /// <summary>
        /// Creates a new instance of the FaceScanGenerator with custom configuration options.
        /// </summary>
        /// <remarks>Use this method to customize thresholds or other settings for the FaceScanGenerator
        /// before instantiation.</remarks>
        /// <param name="configure">A delegate that configures the FaceScanFactoryOptionsBuilder to specify custom options for the
        /// FaceScanGenerator. Cannot be null.</param>
        /// <returns>A FaceScanGenerator instance configured with the specified options.</returns>
        public FaceScanGenerator CreateFaceScanGenerator(Action<FaceScanFactoryOptionsBuilder> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);
            Logger.LogDebug("Creating FaceScanGenerator with override options.");
            var options = new FaceScanFactoryOptionsBuilder();
            configure(options);

            var builder = new FaceScanGenerator.Builder();
            builder.WithLogger(LoggerFactory.CreateLogger(typeof(FaceScanGenerator)));
            return builder.Build();
        }

        public FaceScanComparator CreateFaceScanComparator()
        {
            Logger.LogDebug("Creating FaceScanComparator with pre-configured options.");
            var builder = new FaceScanComparator.Builder();
            builder.WithLogger(LoggerFactory.CreateLogger(typeof(FaceScanComparator)));
            if (Options == null)
                return builder.Build();
            if (Options.PotentialMatchThreshold.HasValue)
                builder.WithPotentialMatchThreshold(Options.PotentialMatchThreshold.Value);
            if (Options.PositiveMatchThreashold.HasValue)
                builder.WithPositiveMatchThreashold(Options.PositiveMatchThreashold.Value);
            return builder.Build();
        }

        public FaceScanComparator CreateFaceScanComparator(Action<FaceScanFactoryOptionsBuilder> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);
            Logger.LogDebug("Creating FaceScanComparator with override options.");
            var options = new FaceScanFactoryOptionsBuilder();
            configure(options);

            Logger.LogTrace("Override options - PositiveMatchThreshold:{PositiveMatchThreashold}, PotentialMatchThreshold:{PotentialMatchThreshold}", options.PositiveMatchThreashold, options.PotentialMatchThreshold);
            var builder = new FaceScanComparator.Builder();
            if (options.PotentialMatchThreshold.HasValue)
                builder.WithPotentialMatchThreshold(options.PotentialMatchThreshold.Value);
            if (options.PositiveMatchThreashold.HasValue)
                builder.WithPositiveMatchThreashold(options.PositiveMatchThreashold.Value);
            return builder.Build();
        }
    }
}
