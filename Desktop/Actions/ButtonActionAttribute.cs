using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Declares a button action with the specifed action identifier and path hint.
    /// </summary>
    public class ButtonActionAttribute : ClickActionAttribute
    {
        /// <summary>
        /// Attribute constructor
        /// </summary>
        /// <param name="actionID">The logical action identifier to associate with this action</param>
        /// <param name="pathHint">The suggested location of this action in the toolbar model</param>
        public ButtonActionAttribute(string actionID, string pathHint)
            : base(actionID, pathHint)
        {
        }

        internal override void Apply(IActionBuilder builder)
        {
            builder.Apply(this);
        }
    }
}
