using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pathoschild.Stardew.Common;
using Pathoschild.Stardew.Common.Integrations.CustomBush;
using StardewValley;

namespace Pathoschild.Stardew.LookupAnything.Framework.Fields
{
    /// <summary>A metadata field which shows a list of bush product drops, for custom bush mod.</summary>
    internal class CustomBushDropsField : GenericField
    {
        /*********
        ** Fields
        *********/
        /// <summary>Provides utility methods for interacting with the game code.</summary>
        protected GameHelper GameHelper;

        /// <summary>The possible drops.</summary>
        private readonly Tuple<ICustomBushDrop, Item, SpriteInfo?>[] Drops;

        /// <summary>The text to display before the list, if any.</summary>
        private readonly string? Preface;

        /// <summary>The text to display if there are no items.</summary>
        private readonly string? DefaultText;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="gameHelper">Provides utility methods for interacting with the game code.</param>
        /// <param name="label">A short field label.</param>
        /// <param name="drops">The possible drops.</param>
        /// <param name="sort">Whether to sort the resulting list by probability and name.</param>
        /// <param name="defaultText">The text to display if there are no items (or <c>null</c> to hide the field).</param>
        /// <param name="preface">The text to display before the list, if any.</param>
        public CustomBushDropsField(GameHelper gameHelper, string label, IEnumerable<ICustomBushDrop> drops, bool sort = true, string? defaultText = null, string? preface = null)
            : base(label)
        {
            this.GameHelper = gameHelper;
            this.Drops = this.GetEntries(drops, gameHelper).ToArray();
            if (sort)
                this.Drops = this.Drops.OrderByDescending(p => p.Item1.Season).ThenBy(p => p.Item1.Chance).ToArray();

            this.HasValue = defaultText != null || this.Drops.Any();
            this.Preface = preface;
            this.DefaultText = defaultText;
        }

        /// <summary>Draw the value (or return <c>null</c> to render the <see cref="GenericField.Value"/> using the default format).</summary>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        /// <param name="font">The recommended font.</param>
        /// <param name="position">The position at which to draw.</param>
        /// <param name="wrapWidth">The maximum width before which content should be wrapped.</param>
        /// <returns>Returns the drawn dimensions, or <c>null</c> to draw the <see cref="GenericField.Value"/> using the default format.</returns>
        public override Vector2? DrawValue(SpriteBatch spriteBatch, SpriteFont font, Vector2 position, float wrapWidth)
        {
            if (!this.Drops.Any())
                return spriteBatch.DrawTextBlock(font, this.DefaultText, position, wrapWidth);

            float height = 0;

            // draw preface
            if (!string.IsNullOrWhiteSpace(this.Preface))
            {
                Vector2 prefaceSize = spriteBatch.DrawTextBlock(font, this.Preface, position, wrapWidth);
                height += (int)prefaceSize.Y;
            }

            // list drops
            Vector2 iconSize = new(font.MeasureString("ABC").Y);
            foreach ((ICustomBushDrop drop, Item item, SpriteInfo? sprite) in this.Drops)
            {
                // get data
                bool isGuaranteed = drop.Chance > .99f;

                // draw icon
                spriteBatch.DrawSpriteWithin(sprite, position.X, position.Y + height, iconSize);

                // draw text
                string text = isGuaranteed ? item.DisplayName : I18n.Generic_PercentChanceOf(percent: (decimal)(Math.Round(drop.Chance, 4) * 100), label: item.DisplayName);
                if (drop.MinStack < drop.MaxStack)
                    text += $" ({I18n.Generic_Range(min: drop.MinStack, max: drop.MaxStack)})";
                else if (drop.MinStack > 1)
                    text += $" ({drop.MinStack})";
                Vector2 textSize = spriteBatch.DrawTextBlock(font, text, position + new Vector2(iconSize.X + 5, height + 5), wrapWidth);
                if (drop.Condition != null)
                {
                    string conditionText = I18n.Item_RecipesForMachine_Conditions(conditions: HumanReadableConditionParser.Format(drop.Condition));
                    height += textSize.Y + 5;
                    textSize = spriteBatch.DrawTextBlock(font, conditionText, position + new Vector2(iconSize.X + 5, height + 5), wrapWidth);
                }
                height += textSize.Y + 5;
            }

            // return size
            return new Vector2(wrapWidth, height);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the internal drop list entries.</summary>
        /// <param name="drops">The possible drops.</param>
        /// <param name="gameHelper">Provides utility methods for interacting with the game code.</param>
        private IEnumerable<Tuple<ICustomBushDrop, Item, SpriteInfo?>> GetEntries(IEnumerable<ICustomBushDrop> drops, GameHelper gameHelper)
        {
            foreach (ICustomBushDrop drop in drops)
            {
                Item item = ItemRegistry.Create(drop.ItemId);
                SpriteInfo? sprite = gameHelper.GetSprite(item);
                yield return Tuple.Create(drop, item, sprite);
            }
        }
    }
}
