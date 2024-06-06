using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using StardewValley.Menus;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the dialogue-box-open animation.</summary>
    /// <remarks>See game logic in <see cref="DialogueBox.update"/>.</remarks>
    internal class OpenDialogueBoxHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public OpenDialogueBoxHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            return Game1.activeClickableMenu is DialogueBox { transitioning: true };
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            DialogueBox menu = (DialogueBox)Game1.activeClickableMenu;

            this.ApplySkips(
                () => menu.update(Game1.currentGameTime),
                () => !this.IsEnabled(playerAnimationID)
            );
        }
    }
}
