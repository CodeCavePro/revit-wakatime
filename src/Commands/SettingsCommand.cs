using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodeCave.WakaTime.Revit;

namespace CodeCave.WakaTime.Revit
{
    /// <summary>
    /// WakaTime settings command
    /// </summary>
    /// <seealso cref="Autodesk.Revit.UI.IExternalCommand" />
    /// <seealso cref="T:Autodesk.Revit.UI.IExternalCommand" />
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SettingsCommand : IExternalCommand
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
            using (var settingsForm = new SettingsForm())
            {
                settingsForm.ShowDialog();
            }

            return Result.Succeeded;
        }
    }
}
