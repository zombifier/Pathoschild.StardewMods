using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the parrot-express animation.</summary>
    /// <remarks>See game logic in <see cref="ParrotPlatform.Update"/>.</remarks>
    internal class ParrotExpressHandler : BaseAnimationHandler
    {
        /// <inheritdoc />
        public ParrotExpressHandler(float multiplier) : base(multiplier)
        {
        }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            var platform = ParrotPlatform.activePlatform;
            return platform is not null &&
                   platform.takeoffState > ParrotPlatform.TakeoffState.Idle;
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            var platform = ParrotPlatform.activePlatform;
            this.ApplySkips(
                () => platform.Update(Game1.currentGameTime),
                () => !this.IsEnabled(playerAnimationID)
            );
        }
    }
}
