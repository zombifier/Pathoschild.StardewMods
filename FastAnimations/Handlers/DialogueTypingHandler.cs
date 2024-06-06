using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using StardewValley.Menus;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the dialogue-typing animation.</summary>
    /// <remarks>See game logic in <see cref="DialogueBox.update"/>.</remarks>
    internal class DialogueTypingHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public DialogueTypingHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            return
                Game1.activeClickableMenu is DialogueBox { transitioning: false } menu
                && menu.characterIndexInDialogue < menu.getCurrentString().Length;
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
