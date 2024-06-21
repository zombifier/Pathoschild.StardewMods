using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pathoschild.Stardew.Common.Integrations.CustomBush;
using Pathoschild.Stardew.Common.Integrations.BushBloomMod;
using Pathoschild.Stardew.LookupAnything.Framework.DebugFields;
using Pathoschild.Stardew.LookupAnything.Framework.Fields;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.TokenizableStrings;
using StardewModdingAPI;
using System.Threading;
using System.Linq;

namespace Pathoschild.Stardew.LookupAnything.Framework.Lookups.TerrainFeatures
{
    /// <summary>Describes a bush.</summary>
    internal class BushSubject : BaseSubject
    {
        /*********
        ** Fields
        *********/
        /// <summary>The underlying target.</summary>
        private readonly Bush Target;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="gameHelper">Provides utility methods for interacting with the game code.</param>
        /// <param name="bush">The lookup target.</param>
        public BushSubject(GameHelper gameHelper, Bush bush)
            : base(gameHelper)
        {
            this.Target = bush;

            if (this.TryGetCustomBush(bush, out ICustomBush? customBush))
                this.Initialize(TokenParser.ParseText(customBush.DisplayName), TokenParser.ParseText(customBush.Description), I18n.Type_Bush());
            else if (this.IsBerryBush(bush))
                this.Initialize(I18n.Bush_Name_Berry(), I18n.Bush_Description_Berry(), I18n.Type_Bush());
            else if (this.IsTeaBush(bush))
                this.Initialize(I18n.Bush_Name_Tea(), I18n.Bush_Description_Tea(), I18n.Type_Bush());
            else
                this.Initialize(I18n.Bush_Name_Plain(), I18n.Bush_Description_Plain(), I18n.Type_Bush());
        }

        /// <summary>Get the data to display for this subject.</summary>
        public override IEnumerable<ICustomField> GetData()
        {
            // get basic info
            Bush bush = this.Target;
            bool isBerryBush = this.IsBerryBush(bush);
            bool isTeaBush = this.IsTeaBush(bush);
            SDate today = SDate.Now();

            if (isBerryBush && this.TryBushBloomGetAllSchedules(out (string, WorldDate, WorldDate)[]? bushBloomSchedule))
            {
                Array.Sort(bushBloomSchedule, ((string, WorldDate, WorldDate) entryX, (string, WorldDate, WorldDate) entryY) =>
                {
                    if (entryX.Item2 < entryY.Item2)
                        return -1;
                    if (entryX.Item2 > entryY.Item2)
                        return 1;
                    if (entryX.Item3 < entryY.Item3)
                        return -1;
                    if (entryX.Item3 > entryY.Item3)
                        return 1;
                    return 0;
                });
                List<Item> itemList = [];
                Dictionary<string, string> displayText = [];
                foreach ((string, WorldDate, WorldDate) entry in bushBloomSchedule)
                {
                    // Item1: ItemId (not qualified, needs (O) prefix)
                    // Item2: start day
                    // Item3: end day inclusive
                    SDate lastDay = SDate.From(entry.Item3);
                    if (today > lastDay)
                        continue;
                    SDate firstDay = SDate.From(entry.Item2);
                    if (today < firstDay)
                        continue;
                    Item item = ItemRegistry.Create(entry.Item1);
                    itemList.Add(item);
                    if (firstDay == lastDay)
                        displayText[item.QualifiedItemId] = $"{item.DisplayName}: {this.Stringify(firstDay)}";
                    else
                        displayText[item.QualifiedItemId] = $"{item.DisplayName}: {this.Stringify(firstDay)} - {this.Stringify(lastDay)}";
                }
                yield return new ItemIconListField(this.GameHelper, I18n.Bush_NextHarvest(), itemList, false, displayText);
            }
            else
            {
                if (isBerryBush || isTeaBush)
                {
                    SDate nextHarvest = this.GetNextHarvestDate(bush);
                    string nextHarvestStr = nextHarvest == today
                        ? I18n.Generic_Now()
                        : $"{this.Stringify(nextHarvest)} ({this.GetRelativeDateStr(nextHarvest)})";
                    if (this.TryGetCustomBushDrops(bush, out IList<ICustomBushDrop>? drops))
                    {
                        yield return new CustomBushDropsField(this.GameHelper, I18n.Bush_NextHarvest(), drops, preface: nextHarvestStr);
                    }
                    else
                    {
                        string harvestSchedule = isTeaBush ? I18n.Bush_Schedule_Tea() : I18n.Bush_Schedule_Berry();
                        yield return new GenericField(I18n.Bush_NextHarvest(), $"{nextHarvestStr}{Environment.NewLine}{harvestSchedule}");
                    }
                }

                // date planted + grown
                if (isTeaBush)
                {
                    SDate datePlanted = this.GetDatePlanted(bush);
                    int daysOld = SDate.Now().DaysSinceStart - datePlanted.DaysSinceStart; // bush.getAge() not reliable, e.g. for Caroline's tea bush
                    SDate dateGrown = this.GetDateFullyGrown(bush);

                    yield return new GenericField(I18n.Bush_DatePlanted(), $"{this.Stringify(datePlanted)} ({this.GetRelativeDateStr(-daysOld)})");
                    if (dateGrown > today)
                    {
                        string grownOnDateText = I18n.Bush_Growth_Summary(date: this.Stringify(dateGrown));
                        yield return new GenericField(I18n.Bush_Growth(), $"{grownOnDateText} ({this.GetRelativeDateStr(dateGrown)})");
                    }
                }
            }
        }

        /// <summary>Get the data to display for this subject.</summary>
        public override IEnumerable<IDebugField> GetDebugFields()
        {
            Bush target = this.Target;

            // pinned fields
            yield return new GenericDebugField("health", target.health, pinned: true);
            yield return new GenericDebugField("is town bush", this.Stringify(target.townBush.Value), pinned: true);
            yield return new GenericDebugField("is in bloom", this.Stringify(target.inBloom()), pinned: true);

            // raw fields
            foreach (IDebugField field in this.GetDebugFieldsFrom(target))
                yield return field;
        }

        /// <summary>Draw the subject portrait (if available).</summary>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        /// <param name="position">The position at which to draw.</param>
        /// <param name="size">The size of the portrait to draw.</param>
        /// <returns>Returns <c>true</c> if a portrait was drawn, else <c>false</c>.</returns>
        public override bool DrawPortrait(SpriteBatch spriteBatch, Vector2 position, Vector2 size)
        {
            Bush bush = this.Target;

            // get source info
            Rectangle sourceArea = bush.sourceRect.Value;
            Point spriteSize = new(sourceArea.Width * Game1.pixelZoom, sourceArea.Height * Game1.pixelZoom);
            SpriteEffects spriteEffects = bush.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // calculate target area
            float scale = Math.Min(size.X / spriteSize.X, size.Y / spriteSize.Y);
            Point targetSize = new((int)(spriteSize.X * scale), (int)(spriteSize.Y * scale));
            Vector2 offset = new Vector2(size.X - targetSize.X, size.Y - targetSize.Y) / 2;

            // get texture
            Texture2D texture;
            if (this.TryGetCustomBush(bush, out ICustomBush? customBush))
            {
                texture = bush.IsSheltered()
                    ? Game1.content.Load<Texture2D>(customBush.IndoorTexture)
                    : Game1.content.Load<Texture2D>(customBush.Texture);
            }
            else
                texture = Bush.texture.Value;

            // draw portrait
            spriteBatch.Draw(
                texture: texture,
                destinationRectangle: new((int)(position.X + offset.X), (int)(position.Y + offset.Y), targetSize.X, targetSize.Y),
                sourceRectangle: sourceArea,
                color: Color.White,
                rotation: 0,
                origin: Vector2.Zero,
                effects: spriteEffects,
                layerDepth: 0
            );
            return true;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get whether the given bush produces berries.</summary>
        /// <param name="bush">The berry busy.</param>
        private bool IsBerryBush(Bush bush)
        {
            return bush.size.Value == Bush.mediumBush && !bush.townBush.Value && !bush.Location.InIslandContext();
        }

        /// <summary>Get whether a given bush produces tea.</summary>
        /// <param name="bush">The bush to check.</param>
        private bool IsTeaBush(Bush bush)
        {
            return bush.size.Value == Bush.greenTeaBush;
        }

        /// <summary>Get bush model from the Custom Bush mod if applicable.</summary>
        /// <param name="bush">The bush to check.</param>
        /// <param name="customBush">The resulting custom bush, if applicable.</param>
        /// <returns>Returns whether a custom bush was found.</returns>
        private bool TryGetCustomBush(Bush bush, [NotNullWhen(true)] out ICustomBush? customBush)
        {
            customBush = null;
            return
                this.GameHelper.CustomBush.IsLoaded
                && this.GameHelper.CustomBush.ModApi.TryGetCustomBush(bush, out customBush);
        }

        /// <summary>Get bush drops from custom bush.</summary>
        /// <param name="bush">The bush to check.</param>
        /// <param name="drops">When this method returns, contains the items produced by the custom bush.</param>
        /// <returns>Returns whether a custom bush with drops was found.</returns>
        private bool TryGetCustomBushDrops(Bush bush, [NotNullWhen(true)] out IList<ICustomBushDrop>? drops)
        {
            drops = null;
            return
                this.GameHelper.CustomBush.IsLoaded
                && this.GameHelper.CustomBush.ModApi.TryGetCustomBush(bush, out ICustomBush? _customBush, out string? id)
                && this.GameHelper.CustomBush.ModApi.TryGetDrops(id, out drops);
        }

        /// <summary>
        /// Get custom bush bloom schedule for today at this locaiton
        /// </summary>
        /// <returns>Returns BushBloomMod loaded</returns>
        private bool TryBushBloomGetActiveSchedules(Bush bush, [NotNullWhen(true)] out (string, WorldDate, WorldDate)[]? bushBloomSchedule)
        {
            SDate today = SDate.Now();
            bushBloomSchedule = null;
            if (this.GameHelper.BushBloomMod.IsLoaded && this.GameHelper.BushBloomMod.ModApi.IsReady())
            {
                bushBloomSchedule = this.GameHelper.BushBloomMod.ModApi.GetActiveSchedules(
                    today.Season.ToString(), today.Day, today.Year,
                    bush.Location, bush.Tile
                );
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get custom bush bloom schedule for today
        /// </summary>
        /// <returns>Returns BushBloomMod loaded</returns>
        private bool TryBushBloomGetAllSchedules([NotNullWhen(true)] out (string, WorldDate, WorldDate)[]? bushBloomSchedule)
        {
            bushBloomSchedule = null;
            if (this.GameHelper.BushBloomMod.IsLoaded && this.GameHelper.BushBloomMod.ModApi.IsReady())
            {
                bushBloomSchedule = this.GameHelper.BushBloomMod.ModApi.GetAllSchedules();
                return true;
            }
            return false;
        }

        /// <summary>Get the date when the bush was planted.</summary>
        /// <param name="bush">The bush to check.</param>
        private SDate GetDatePlanted(Bush bush)
        {
            SDate date = new(1, Season.Spring, 1);
            if (this.IsTeaBush(bush) && bush.datePlanted.Value > 0) // Caroline's sun room bush has datePlanted = -999
                date = date.AddDays(bush.datePlanted.Value);
            return date;
        }

        /// <summary>Get the date when the bush will be fully grown.</summary>
        /// <param name="bush">The bush to check.</param>
        private SDate GetDateFullyGrown(Bush bush)
        {
            SDate date = this.GetDatePlanted(bush);
            if (this.TryGetCustomBush(bush, out ICustomBush? customBush))
                date = date.AddDays(customBush.AgeToProduce);
            else if (this.IsTeaBush(bush))
                date = date.AddDays(Bush.daysToMatureGreenTeaBush);
            return date;
        }

        /// <summary>Get the day of season when this push start to produce.</summary>
        /// <param name="bush">The bush to check.</param>
        private int GetDayToBeginProducing(Bush bush)
        {
            if (this.TryGetCustomBush(bush, out ICustomBush? customBush))
                return customBush.DayToBeginProducing;
            else if (this.IsTeaBush(bush))
                // tea bushes produce on day 22+ of season
                return 22;
            return -1;
        }

        /// <summary>Get seasons during which this bush will produce (if not sheltered).</summary>
        /// <param name="bush">The bush to check.</param>
        private List<Season> GetProducingSeasons(Bush bush)
        {
            if (this.TryGetCustomBush(bush, out ICustomBush? customBush))
                return customBush.Seasons;
            else if (this.IsTeaBush(bush))
                return [Season.Spring, Season.Summer, Season.Fall];
            else
                return [Season.Spring, Season.Fall];
        }

        /// <summary>Get the next date when the bush will produce forage.</summary>
        /// <param name="bush">The bush to check.</param>
        /// <remarks>Derived from <see cref="Bush.inBloom"/>.</remarks>
        private SDate GetNextHarvestDate(Bush bush)
        {
            SDate today = SDate.Now();
            var tomorrow = today.AddDays(1);

            // currently has produce
            if (bush.tileSheetOffset.Value == 1)
                return today;

            // tea bush and custom bush
            int dayToBegin = this.GetDayToBeginProducing(bush);
            if (dayToBegin >= 0)
            {
                SDate readyDate = this.GetDateFullyGrown(bush);
                if (readyDate < tomorrow)
                    readyDate = tomorrow;
                if (!bush.IsSheltered())
                {
                    // bush not sheltered, must check producing seasons
                    List<Season> producingSeasons = this.GetProducingSeasons(bush);
                    SDate seasonDate = new(Math.Max(1, dayToBegin), readyDate.Season, readyDate.Year);
                    while (!producingSeasons.Contains(seasonDate.Season))
                        seasonDate = seasonDate.AddDays(28);
                    if (readyDate < seasonDate)
                        return seasonDate;
                }
                if (readyDate.Day < dayToBegin)
                    readyDate = new(dayToBegin, readyDate.Season, readyDate.Year);
                return readyDate;
            }

            // wild bushes produce salmonberries in spring 15-18, and blackberries in fall 8-11
            SDate springStart = new(15, Season.Spring);
            SDate springEnd = new(18, Season.Spring);
            SDate fallStart = new(8, Season.Fall);
            SDate fallEnd = new(11, Season.Fall);

            if (tomorrow < springStart)
                return springStart;
            if (tomorrow > springEnd && tomorrow < fallStart)
                return fallStart;
            if (tomorrow > fallEnd)
                return new(springStart.Day, springStart.Season, springStart.Year + 1);
            return tomorrow;
        }
    }
}
