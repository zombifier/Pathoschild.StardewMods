using Pathoschild.Stardew.FastAnimations.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the prize machine animation.</summary>
    /// <remarks>See game logic in <see cref="ForgeMenu.update"/>.</remarks>
    internal class ForgingHandler : BaseAnimationHandler
    {
        private readonly IModHelper Helper;

        /*********
         ** Public methods
         *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="multiplier">The animation speed multiplier to apply.</param>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public ForgingHandler(float multiplier, IModHelper helper)
            : base(multiplier)
        {
            this.Helper = helper;
        }

        /// <summary>Get whether the animation is currently active.</summary>
        /// <param name="playerAnimationID">The player's current animation ID.</param>
        public override bool IsEnabled(int playerAnimationID)
        {
            if (Game1.activeClickableMenu is ForgeMenu menu)
            {
                return this.Helper.Reflection.GetField<int>(menu, "_timeUntilCraft").GetValue() > 0 ||
                       this.Helper.Reflection.GetField<int>(menu, "_sparklingTimer").GetValue() > 0;
            }
            return false;
        }

        /// <summary>Perform any logic needed on update while the animation is active.</summary>
        /// <param name="playerAnimationID">The player's current animation ID.</param>
        public override void Update(int playerAnimationID)
        {
            ForgeMenu menu = (ForgeMenu)Game1.activeClickableMenu;
            this.ApplySkips(
                () => menu.update(Game1.currentGameTime)
            );
        }
    }
}
