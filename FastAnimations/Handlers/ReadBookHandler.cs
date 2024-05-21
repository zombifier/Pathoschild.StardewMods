using System;
using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace Pathoschild.Stardew.FastAnimations.Handlers;

/// <summary>Handles the read-book animation.</summary>
/// <remarks>See game logic in <see cref="SObject.readBook"/>.</remarks>
internal class ReadBookHandler : BaseAnimationHandler
{
    /*********
     ** Public methods
     *********/
    /// <summary>Construct an instance.</summary>
    /// <param name="multiplier">The animation speed multiplier to apply.</param>
    public ReadBookHandler(float multiplier) : base(multiplier)
    {
    }

    /// <summary>Get whether the animation is currently active.</summary>
    /// <param name="playerAnimationID">The player's current animation ID.</param>
    public override bool IsEnabled(int playerAnimationID)
    {
        return Game1.player.FarmerSprite.currentAnimation is [{ frame: 57, milliseconds: 1000 }, ..];
    }

    /// <summary>Perform any logic needed on update while the animation is active.</summary>
    /// <param name="playerAnimationID">The player's current animation ID.</param>
    public override void Update(int playerAnimationID)
    {
        this.SpeedUpPlayer(() => !this.IsEnabled(playerAnimationID));

        // reduce freeze time
        int reduceTimersBy = (int)(BaseAnimationHandler.MillisecondsPerFrame * this.Multiplier);
        Game1.player.freezePause = Math.Max(0, Game1.player.freezePause - reduceTimersBy);
    }
}
