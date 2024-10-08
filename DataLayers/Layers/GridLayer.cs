using Microsoft.Xna.Framework;
using Pathoschild.Stardew.DataLayers.Framework;
using StardewValley;

namespace Pathoschild.Stardew.DataLayers.Layers
{
    /// <summary>A data layer which just shows the tile grid.</summary>
    internal class GridLayer : BaseLayer
    {
        /*********
        ** Fields
        *********/
        /// <summary>A cached empty tile group list.</summary>
        private readonly TileGroup[] NoGroups = [];


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="config">The data layer settings.</param>
        public GridLayer(LayerConfig config)
            : base(I18n.Grid_Name(), config)
        {
            this.Legend = [];
            this.AlwaysShowGrid = true;
        }

        /// <inheritdoc />
        public override TileGroup[] Update(GameLocation location, in Rectangle visibleArea, in Vector2[] visibleTiles, in Vector2 cursorTile)
        {
            return this.NoGroups;
        }
    }
}
