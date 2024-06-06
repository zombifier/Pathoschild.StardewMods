using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the event-run animation.</summary>
    /// <remarks>See game logic in <see cref="Event.Update"/>.</remarks>
    internal class EventHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public EventHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            return Game1.eventUp && !Game1.isFestival() && !Game1.fadeToBlack;
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            var location = Game1.currentLocation;

            this.ApplySkips(
                () =>
                {
                    if (Game1.CurrentEvent.GetCurrentCommand().StartsWith("pause"))
                        Game1.updatePause(Game1.currentGameTime);
                    else
                        Game1.CurrentEvent.Update(location, Game1.currentGameTime);
                },
                () => !this.IsEnabled(playerAnimationID)
            );
        }
    }
}
