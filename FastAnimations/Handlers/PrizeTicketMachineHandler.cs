using Pathoschild.Stardew.FastAnimations.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the ticket prize machine animation.</summary>
    /// <remarks>See game logic in <see cref="PrizeTicketMenu.update"/>.</remarks>
    internal class PrizeTicketMachineHandler : BaseAnimationHandler
    {
        /*********
        ** Fields
        *********/
        /// <summary>An API for accessing inaccessible code.</summary>
        private readonly IReflectionHelper Reflection;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="multiplier">The animation speed multiplier to apply.</param>
        /// <param name="reflection">An API for accessing inaccessible code.</param>
        public PrizeTicketMachineHandler(float multiplier, IReflectionHelper reflection)
            : base(multiplier)
        {
            this.Reflection = reflection;
        }

        /// <summary>Get whether the animation is currently active.</summary>
        /// <param name="playerAnimationID">The player's current animation ID.</param>
        public override bool IsEnabled(int playerAnimationID)
        {
            return
                Game1.activeClickableMenu is PrizeTicketMenu menu
                && (
                    this.Reflection.GetField<bool>(menu, "gettingReward").GetValue()
                    || this.Reflection.GetField<bool>(menu, "movingRewardTrack").GetValue()
                );
        }

        /// <summary>Perform any logic needed on update while the animation is active.</summary>
        /// <param name="playerAnimationID">The player's current animation ID.</param>
        public override void Update(int playerAnimationID)
        {
            PrizeTicketMenu menu = (PrizeTicketMenu)Game1.activeClickableMenu;

            this.ApplySkips(
                () => menu.update(Game1.currentGameTime)
            );
        }
    }
}
