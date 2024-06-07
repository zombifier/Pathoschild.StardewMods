using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the event-run animation.</summary>
    /// <remarks>See game logic in <see cref="Event.Update"/>.</remarks>
    internal sealed class EventHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public EventHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool TryApply(int playerAnimationId)
        {
            return
                this.ShouldApply()
                && this.ApplySkipsWhile(
                () =>
                {
                    if (Game1.CurrentEvent.GetCurrentCommand().StartsWith("pause"))
                        Game1.updatePause(Game1.currentGameTime);
                    else
                        Game1.CurrentEvent.Update(Game1.currentLocation, Game1.currentGameTime);

                    return this.ShouldApply();
                });
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get whether the handler should be applied now.</summary>
        private bool ShouldApply()
        {
            return Game1.eventUp && !Game1.isFestival() && !Game1.fadeToBlack;
        }
    }
}
