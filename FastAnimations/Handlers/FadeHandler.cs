using Microsoft.Xna.Framework;
using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using StardewValley.BellsAndWhistles;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the screen fade-to-black and fade-in animations.</summary>
    /// <remarks>See game logic in <see cref="Game1._update"/>.</remarks>
    internal class FadeHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public FadeHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            return Game1.fadeToBlack;
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            this.ApplySkips(
                this.UpdateFadeToBlack,
                () => !this.IsEnabled(playerAnimationID)
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Handler the screen fade to black.</summary>
        /// <remarks>Derived from <see cref="ScreenFade.UpdateFadeAlpha"/>.</remarks>
        private void UpdateFadeToBlack()
        {
            GameTime time = Game1.currentGameTime;

            if (Game1.fadeIn)
                Game1.fadeToBlackAlpha += (Game1.eventUp || Game1.farmEvent != null ? 0.0008f : 0.0019f) * time.ElapsedGameTime.Milliseconds;
            else if (!Game1.messagePause && !Game1.dialogueUp)
                Game1.fadeToBlackAlpha -= (Game1.eventUp || Game1.farmEvent != null ? 0.0008f : 0.0019f) * time.ElapsedGameTime.Milliseconds;
        }
    }
}
