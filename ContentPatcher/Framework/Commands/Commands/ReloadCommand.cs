using System;
using System.Linq;
using ContentPatcher.Framework.Patches;
using Pathoschild.Stardew.Common.Commands;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace ContentPatcher.Framework.Commands.Commands
{
    /// <summary>A console command which reloads the patches from a given content pack.</summary>
    internal class ReloadCommand : BaseCommand
    {
        /*********
        ** Fields
        *********/
        /// <summary>Manages loading and unloading patches.</summary>
        private readonly Func<PatchLoader> GetPatchLoader;

        /// <summary>Manages loaded patches.</summary>
        private readonly Func<PatchManager> GetPatchManager;

        /// <summary>The loaded content packs.</summary>
        private readonly LoadedContentPack[] ContentPacks;

        /// <summary>A callback which immediately updates the current condition context.</summary>
        private readonly Action UpdateContext;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        /// <param name="getPatchLoader">Manages loading and unloading patches.</param>
        /// <param name="contentPacks">The loaded content packs.</param>
        /// <param name="updateContext">A callback which immediately updates the current condition context.</param>
        public ReloadCommand(IMonitor monitor, Func<PatchLoader> getPatchLoader, Func<PatchManager> getPatchManager, LoadedContentPack[] contentPacks, Action updateContext)
            : base(monitor, "reload")
        {
            this.GetPatchLoader = getPatchLoader;
            this.GetPatchManager = getPatchManager;
            this.ContentPacks = contentPacks;
            this.UpdateContext = updateContext;
        }

        /// <inheritdoc />
        public override string GetDescription()
        {
            return
                """
                patch reload
                   Usage: patch reload "<content pack ID>" "[optional specific included file to reload]"
                   Reloads the patches of the content.json (or a specified json loaded by an Include patch) of a content pack. Config schema changes and dynamic token changes are unsupported.
                """;
        }

        /// <inheritdoc />
        public override void Handle(string[] args)
        {
            var patchLoader = this.GetPatchLoader();
            var patchManager = this.GetPatchManager();

            // get pack ID
            if (args.Length < 1 || args.Length > 2)
            {
                this.Monitor.Log("The 'patch reload' command expects a single argument containing the target content pack ID, with an optional additional argument for a specific file. See 'patch help' for more info.", LogLevel.Error);
                return;
            }
            string packId = args[0];

            // get pack
            RawContentPack? pack = this.ContentPacks.SingleOrDefault(p => p.Manifest.UniqueID == packId);
            if (pack == null)
            {
                this.Monitor.Log($"No Content Patcher content pack with the unique ID \"{packId}\".", LogLevel.Error);
                return;
            }

            if (args.Length >= 2)
            {
                string specificFilePath = args[1];

                IPatch? patch = patchManager.GetPatches().FirstOrDefault(p => p.FromAsset == PathUtilities.NormalizePath(specificFilePath));
                if (patch == null || patch is not IncludePatch include)
                {
                    this.Monitor.Log($"There was no patch including the path \"{specificFilePath}\" (or it was not an Include type patch).", LogLevel.Error);
                    return;
                }
                else if (!include.IsReady || !patch.Conditions.All(p => p.IsMatch))
                {
                    this.Monitor.Log($"The specified patch is not currently active.");
                    return;
                }

                patchLoader.UnloadPatchesLoadedBy(include);

                include.AttemptLoad();
            }
            else
            {
                // unload patches
                patchLoader.UnloadPatchesLoadedBy(pack);

                // load pack patches
                if (!pack.TryReloadContent(out string? loadContentError))
                {
                    this.Monitor.Log($"Failed to reload content pack '{pack.Manifest.Name}' for configuration changes: {loadContentError}. The content pack may not be in a valid state.", LogLevel.Error); // should never happen
                    return;
                }

                // reload patches
                patchLoader.LoadPatches(
                    contentPack: pack,
                    rawPatches: pack.Content.Changes,
                    rootIndexPath: [pack.Index],
                    path: new LogPathBuilder(pack.Manifest.Name),
                    parentPatch: null
                );
            }

            // make the changes apply
            this.UpdateContext();

            this.Monitor.Log("Content pack reloaded.", LogLevel.Info);
        }
    }
}
