using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using StardewValley.Menus;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the wheel-spinning animation.</summary>
    /// <remarks>See game logic in <see cref="WheelSpinGame.update"/>.</remarks>
    internal class SpinningWheelHandle : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="multiplier">The animation speed multiplier to apply.</param>
        public SpinningWheelHandle(float multiplier)
            : base(multiplier) { }

        /// <summary>Get whether the animation is currently active.</summary>
        /// <param name="playerAnimationID">The player's current animation ID.</param>
        public override bool IsEnabled(int playerAnimationID)
        {
            return Game1.activeClickableMenu is WheelSpinGame { arrowRotationVelocity: > 0 };
        }

        /// <summary>Perform any logic needed on update while the animation is active.</summary>
        /// <param name="playerAnimationID">The player's current animation ID.</param>
        public override void Update(int playerAnimationID)
        {
            WheelSpinGame menu = (WheelSpinGame)Game1.activeClickableMenu;

            this.ApplySkips(
                () => menu.update(Game1.currentGameTime)
            );
        }
    }
}
