using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using StardewValley.Tools;

namespace Pathoschild.Stardew.FastAnimations.Handlers;

/// <summary>Handles the fishing-treasure-open animation.</summary>
/// <remarks>See game logic in <see cref="StardewValley.Tools.FishingRod.openChestEndFunction"/>.</remarks>
internal class FishingTreasureHandler : BaseAnimationHandler
{
    /*********
     ** Public methods
     *********/
    /// <summary>Construct an instance.</summary>
    /// <param name="multiplier">The animation speed multiplier to apply.</param>
    public FishingTreasureHandler(float multiplier) : base(multiplier)
    {
    }

    /// <summary>Get whether the animation is currently active.</summary>
    /// <param name="playerAnimationID">The player's current animation ID.</param>
    public override bool IsEnabled(int playerAnimationID)
    {
        var player = Game1.player;
        return this.GetTemporarySprites(player).Any();
    }

    /// <summary>Perform any logic needed on update while the animation is active.</summary>
    /// <param name="playerAnimationID">The player's current animation ID.</param>
    public override void Update(int playerAnimationID)
    {
        var player = Game1.player;

        this.ApplySkips(
            () =>
            {
                foreach (var sprite in this.GetTemporarySprites(player).ToArray())
                {
                    bool done = sprite.update(Game1.currentGameTime);
                    if (done)
                        (player.CurrentTool as FishingRod)?.animations.Remove(sprite);
                }
            },
            () => !this.IsEnabled(playerAnimationID)
        );
    }

    /*********
     ** Private methods
     *********/
    /// <summary>Get the temporary animated sprites added as part of the fishing treasure open animation.</summary>
    /// <param name="player">The player being animated.</param>
    private IEnumerable<TemporaryAnimatedSprite> GetTemporarySprites(Farmer player)
    {
        var fishingRod = player.CurrentTool as FishingRod;
        if (fishingRod is null) yield break;

        foreach (TemporaryAnimatedSprite sprite in fishingRod.animations)
        {
            if (sprite.textureName == "LooseSprites\\Cursors_1_6" && sprite.sourceRect.Intersects(new Rectangle(256, 75, 32 * 4, 32)))
                yield return sprite;

            if (sprite.textureName == "LooseSprites\\Cursors" && sprite.sourceRect.Intersects(new Rectangle(64, 1920, 32 * 4, 32)))
                yield return sprite;
        }
    }
}
