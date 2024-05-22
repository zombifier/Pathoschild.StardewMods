using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using StardewValley.Tools;

namespace Pathoschild.Stardew.FastAnimations.Handlers
{
    /// <summary>Handles the tool swinging animation.</summary>
    internal class ToolSwingHandler : BaseAnimationHandler
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public ToolSwingHandler(float multiplier)
            : base(multiplier) { }

        /// <inheritdoc />
        public override bool IsEnabled(int playerAnimationID)
        {
            Tool tool = Game1.player.CurrentTool;

            return
                Game1.player.UsingTool
                && !Game1.player.canStrafeForToolUse()
                && tool != null
                && (
                    (tool as MeleeWeapon)?.isScythe() == true
                    || tool is not (FishingRod or MeleeWeapon)
                );
        }

        /// <inheritdoc />
        public override void Update(int playerAnimationID)
        {
            this.SpeedUpPlayer(until: () => !Game1.player.UsingTool);
        }
    }
}
