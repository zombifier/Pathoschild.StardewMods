using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.FastAnimations.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace Pathoschild.Stardew.FastAnimations.Handlers;

/// <summary>Handles the use-totem animation.</summary>
/// <remarks>See game logic in <see cref="SObject.performUseAction"/>.</remarks>
internal class UseTotemHandler : BaseAnimationHandler
{
    /*********
     ** Fields
     *********/
    /// <summary>The last totem used.</summary>
    private Item? LastTotem;

    /*********
     ** Public methods
     *********/
    /// <summary>Construct an instance.</summary>
    /// <param name="multiplier">The animation speed multiplier to apply.</param>
    public UseTotemHandler(float multiplier) : base(multiplier)
    {
    }

    /// <summary>Get whether the animation is currently active.</summary>
    /// <param name="playerAnimationID">The player's current animation ID.</param>
    public override bool IsEnabled(int playerAnimationID)
    {
        var player = Game1.player;

        if (player.CurrentItem?.Name.Contains("Totem") == true)
            this.LastTotem = player.CurrentItem;

        return this.IsUsingTotem(player);
    }

    /// <summary>Perform any logic needed on update while the animation is active.</summary>
    /// <param name="playerAnimationID">The player's current animation ID.</param>
    public override void Update(int playerAnimationID)
    {
        var player = Game1.player;
        var location = player.currentLocation;

        this.ApplySkips(
            () =>
            {
                // player animation
                player.Update(Game1.currentGameTime, location);

                // animation of item thrown in the air
                foreach (TemporaryAnimatedSprite sprite in this.GetTemporarySprites(player).ToArray())
                {
                    bool done = sprite.update(Game1.currentGameTime);
                    if (done)
                        location.TemporarySprites.Remove(sprite);
                }

                // Rain Totem
                if (this.LastTotem?.ItemId == "681")
                    Game1.updatePause(Game1.currentGameTime);
            },
            () => !this.IsUsingTotem(player)
        );
    }

    /*********
     ** Private methods
     *********/
    /// <summary>Get the temporary animated sprites added as part of the use-totem animation.</summary>
    /// <param name="player">The player being animated.</param>
    /// <remarks>Derived from <see cref="SObject.performUseAction"/>.</remarks>
    private IEnumerable<TemporaryAnimatedSprite> GetTemporarySprites(Farmer player)
    {
        if (this.LastTotem == null) yield break;

        foreach (TemporaryAnimatedSprite sprite in player.currentLocation.TemporarySprites)
        {
            var data = ItemRegistry.GetDataOrErrorItem(this.LastTotem.QualifiedItemId);

            // Item sprite
            if (sprite.textureName == data.TextureName && sprite.sourceRect == data.GetSourceRect())
                yield return sprite;

            // Sprinkles sprite
            if (sprite.textureName == "TileSheets\\animations" && sprite.sourceRect.Intersects(new Rectangle(0, 10 * 64, 64 * 8, 128)))
                yield return sprite;

            // Rain totem sprite
            if (sprite.initialParentTileIndex == 0)
                yield return sprite;
        }
    }

    /// <summary>Check whether the current player's animation is the animation of using totem.</summary>
    /// <remarks>Derived from <see cref="SObject.performUseAction"/>.</remarks>
    private bool IsUsingTotem(Farmer player)
    {
        var currentAnimation = player.FarmerSprite.currentAnimation;

        return currentAnimation is [{ frame: 57, milliseconds: 2000 }, ..];
    }
}
