using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using StardewValley.Menus;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the volcano forge animations.</summary>
    /// <remarks>See game logic in <see cref="ForgeMenu.receiveLeftClick"/>.</remarks>
    internal class ForgeHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public ForgeHandler(float multiplier)
            : base(multiplier)
        {
        }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            return Game1.activeClickableMenu is ForgeMenu menu && menu.IsBusy();
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            ForgeMenu menu = (ForgeMenu)Game1.activeClickableMenu;

            this.ApplySkips(
                () => menu.update(Game1.currentGameTime),
                () => !this.IsEnabled(playerAnimationID)
            );
        }
    }
}
