using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using StardewValley.Menus;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the load game menu animations.</summary>
    /// <remarks>See game logic in <see cref="LoadGameMenu"/>.</remarks>
    internal class LoadGameMenuHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public LoadGameMenuHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            return
                Game1.activeClickableMenu is TitleMenu
                && TitleMenu.subMenu is LoadGameMenu loadGameMenu
                && loadGameMenu.timerToLoad > 0;
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            LoadGameMenu menu = (LoadGameMenu)TitleMenu.subMenu;

            this.ApplySkips(
                run: () => menu.update(Game1.currentGameTime),
                until: () => menu.timerToLoad <= 0
            );
        }
    }
}
