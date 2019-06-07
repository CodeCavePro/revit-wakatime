using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;

namespace CodeCave.WakaTime.Revit
{
    /// <summary>
    /// Command which redirects users to WakaTime dashboard (website)
    /// </summary>
    /// <seealso cref="Autodesk.Revit.UI.IExternalCommand" />
    /// <seealso cref="T:Autodesk.Revit.UI.IExternalCommand" />
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DashboardCommand : IExternalCommand
    {
        /// <summary>
        /// Executes the specified Revit command <see cref="ExternalCommand"/>.
        /// The main Execute method (inherited from IExternalCommand) must be public.
        /// </summary>
        /// <param name="commandData">The command data / context.</param>
        /// <param name="message">The message.</param>
        /// <param name="elements">The elements.</param>
        /// <returns>The result of command execution.</returns>
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements
        )
        {
            const string wakaTimeUrl = "https://wakatime.com/dashboard";

            try
            {
                Process.Start(wakaTimeUrl);
                return Result.Succeeded;
            }
            catch
            {
                return Result.Cancelled;
            }
        }
    }
}
