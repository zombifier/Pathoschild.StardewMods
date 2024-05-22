using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using StardewValley.Locations;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles Pam's bus arriving/departing animation.</summary>
    /// <remarks>See game logic in <see cref="BusStop.UpdateWhenCurrentLocation"/> and <see cref="Desert.UpdateWhenCurrentLocation"/>.</remarks>
    internal class PamBusHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public PamBusHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            if (Game1.currentLocation is BusStop stop)
                return stop.drivingOff || stop.drivingBack;
            if (Game1.currentLocation is Desert desert)
                return desert.drivingOff || desert.drivingBack;

            return false;
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            GameLocation location = Game1.currentLocation;

            this.ApplySkips(
                run: () => location.UpdateWhenCurrentLocation(Game1.currentGameTime),
                until: () => !this.IsEnabled(playerAnimationID)
            );
        }
    }
}
