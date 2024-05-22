using System.Collections.Generic;
using System.Linq;
using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using StardewValley.Objects;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the chest-open animation.</summary>
    /// <remarks>See game logic in <see cref="Chest.checkForAction"/>.</remarks>
    internal class OpenChestHandler : BaseAnimationHandler
    {
        /*********
        ** Fields
        *********/
        /// <summary>The chests in the current location.</summary>
        private readonly List<Chest> Chests = new();


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="multiplier">The animation speed multiplier to apply.</param>
        public OpenChestHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            return this.GetOpeningChest() != null;
        }

        /// <inheritdoc />
        public override void OnNewLocation(GameLocation location)
        {
            this.Chests.Clear();
            this.Chests.AddRange(
                location.objects.Values.OfType<Chest>()
            );
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            this.ApplySkips(
                () => this.GetOpeningChest()!.updateWhenCurrentLocation(Game1.currentGameTime),
                () => this.GetOpeningChest() == null
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the chest in the current location which is currently opening.</summary>
        private Chest? GetOpeningChest()
        {
            foreach (Chest chest in this.Chests)
            {
                if (chest.frameCounter.Value > -1)
                    return chest;
            }

            return null;
        }
    }
}
