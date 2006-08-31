using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// A subclass of <see cref="Path"/> that is used by <see cref="IAction"/> to represent an action path.
    /// </summary>
    public class ActionPath : Path
    {
        public const string GlobalMenus = "global-menus";
        public const string GlobalToolbars = "global-toolbars";

        /// <summary>
        /// Constructs an action path from the specified path string, using the specified resource resolver.
        /// If the resource resolver is null, the path segments will be treated as localized text.
        /// </summary>
        /// <param name="pathString">A string respresenting the path</param>
        /// <param name="resolver">A resource resolver used to localize each path segment. May be null.</param>
        public ActionPath(string pathString, IResourceResolver resolver)
            :base(pathString, resolver)
        {
        }

        /// <summary>
        /// The action "site", which is the first segment of the action path.
        /// </summary>
        public string Site
        {
            get { return this.Segments.Length > 0 ? this.Segments[0].ResourceKey : null; }
        }
    }
}
