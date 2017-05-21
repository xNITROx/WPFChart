using System;

namespace ChartControls.CommonModels.DataModels
{
    internal sealed class ChartSettings : ICloneable
    {
        public Scope Scope { get; }


        public ChartSettings()
        {
            Scope = new Scope();
        }

        public ChartSettings Clone()
        {
            return (ChartSettings)MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}