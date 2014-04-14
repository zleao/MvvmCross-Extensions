using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvvmCrossUtilities.Plugins.Gps
{
    public interface IGpsProvider : IDisposable
    {
        #region Properties

        double CurrentLatitude { get; }
        double CurrentLongitude { get; }
        double CurrentSpeed { get; }
        int NumberOfSatellites { get; }
        int SignalQuality { get; }

        #endregion

        #region Methods

        void Start();

        void Stop();

        #endregion
    }
}
