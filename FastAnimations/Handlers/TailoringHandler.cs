using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using StardewValley.Menus;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the tailoring animation.</summary>
    /// <remarks>See game logic in <see cref="TailoringMenu.receiveLeftClick"/>.</remarks>
    internal class TailoringHandler : BaseAnimationHandler
    {
        /// <inheritdoc />
        public TailoringHandler(float multiplier) : base(multiplier)
        {
        }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            return Game1.activeClickableMenu is TailoringMenu menu && menu.IsBusy();
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            TailoringMenu menu = (TailoringMenu)Game1.activeClickableMenu;

            this.ApplySkips(
                () => menu.update(Game1.currentGameTime),
                () => !this.IsEnabled(playerAnimationID)
            );
        }
    }
}
