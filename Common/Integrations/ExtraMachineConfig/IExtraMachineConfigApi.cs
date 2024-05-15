using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using StardewValley.GameData.Machines;
using StardewValley.TerrainFeatures;

namespace Pathoschild.Stardew.Common.Integrations.ExtraMachineConfig
{
    /// <summary>The API provided by the Custom Bush mod.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "The naming convention is defined by the Custom Bush mod.")]
    public interface IExtraMachineConfigApi
    {
        /// <summary>Retrieves the extra fuels consumed by this recipe.</summary>
        IList<(string, int)> GetExtraRequirements(MachineItemOutput outputData);

        /// <summary>Retrieves the extra tag-defined fuels consumed by this recipe.</summary>
        IList<(string, int)> GetExtraTagsRequirements(MachineItemOutput outputData);
    }
}
