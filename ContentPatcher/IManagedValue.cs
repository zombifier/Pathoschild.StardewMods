using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ContentPatcher
{
    /// <summary>A set of parsed values linked to the Content Patcher context. These values are <strong>per-screen</strong>, so the result depends on the screen that's active when calling the members.</summary>
    public interface IManagedValue
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Whether the conditions were parsed successfully (regardless of whether they're in scope currently).</summary>
        [MemberNotNullWhen(false, nameof(IManagedValue.ValidationError))]
        bool IsValid { get; }

        /// <summary>If <see cref="IsValid"/> is false, an error phrase indicating why the conditions failed to parse, formatted like this: <c>'seasonz' isn't a valid token name; must be one of &lt;token list&gt;</c>. If the conditions are valid, this is <c>null</c>.</summary>
        string? ValidationError { get; }

        /// <summary>Whether the conditions' tokens are all valid in the current context. For example, this would be false if the conditions use <c>Season</c> and a save isn't loaded yet.</summary>
        bool IsReady { get; }

        /// <summary>The value(s) provided at the current context.</summary>
        string? Value { get; }


        /*********
        ** Methods
        *********/
        /// <summary>Update the conditions based on Content Patcher's current context for every active screen. It's safe to call this as often as you want, but it has no effect if the Content Patcher context hasn't changed since you last called it.</summary>
        /// <returns>Returns the screens for which <see cref="Values"/> changed value. To check if the current screen changed, you can check <c>UpdateContext()</c></returns>
        IEnumerable<int> UpdateContext();
    }
}
