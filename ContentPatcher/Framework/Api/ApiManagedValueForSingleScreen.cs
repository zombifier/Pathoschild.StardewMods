using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ContentPatcher.Framework.Commands;
using ContentPatcher.Framework.Conditions;
using ContentPatcher.Framework.Tokens;

namespace ContentPatcher.Framework.Api
{
    /// <summary>A parsed string linked to the Content Patcher context for an API consumer, which assume they're always run on the same screen.</summary>
    internal class ApiManagedValueForSingleScreen : IManagedValue
    {
        /*********
        ** Fields
        *********/
        /// <summary>The managed token string corresponding to this managed value.</summary>
        private readonly IManagedTokenString TokenString;

        /// <summary>The context with which to update conditions.</summary>
        private readonly IContext Context;

        /// <summary>The context update tick when the conditions were last updated.</summary>
        private int LastUpdateTick = -1;


        /*********
        ** Accessors
        *********/
        /// <inheritdoc />
        [MemberNotNullWhen(false, nameof(ApiManagedValue.ValidationError))]
        public bool IsValid { get; }

        /// <inheritdoc />
        public string? ValidationError { get; }

        /// <inheritdoc />
        public bool IsReady => this.Contextuals.IsReady;

        /// <inheritdoc />
        public string? Value { get; private set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="tokenString">The underlying token string.</param>
        /// <param name="context">The context with which to update conditions.</param>
        /// <param name="isValid">Whether the conditions were parsed successfully (regardless of whether they're in scope currently).</param>
        /// <param name="validationError">If <paramref name="isValid"/> is false, an error phrase indicating why the conditions failed to parse.</param>
        public ApiManagedValueForSingleScreen(IManagedTokenString tokenString, IContext context, bool isValid = true, string? validationError = null)
        {
            this.TokenString = tokenString;
            this.Context = context;
            this.IsValid = isValid;
            this.ValidationError = validationError;
        }

        /// <inheritdoc />
        public IEnumerable<int> UpdateContext()
        {
            // skip unneeded updates
            if (!this.ShouldUpdate())
                return Enumerable.Empty<int>();
            this.LastUpdateTick = this.Context.UpdateTick;

            // update context
            string? oldValue = this.Value;
            if (this.IsValid)
            {
                this.TokenString.UpdateContext(this.Context);
                this.Value = this.IsReady ? this.TokenString.Value : null;
            }

            // return screen ID if it changed
            return this.Value != oldValue
                ? new[] { StardewModdingAPI.Context.ScreenId }
                : Enumerable.Empty<int>();
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get whether the conditions need to be updated for the current context.</summary>
        private bool ShouldUpdate()
        {
            // update once if immutable
            if (!this.TokenString.IsMutable)
                return false;

            // else update if context changed
            return this.LastUpdateTick < this.Context.UpdateTick;
        }
    }
}
