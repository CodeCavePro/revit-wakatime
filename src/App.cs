#region Namespaces

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

#if REVIT2017
using System.Globalization;
using System.Linq;
using System.Threading;
#endif

#endregion

namespace CodeCave.WakaTime.Revit
{
    /// <summary>
    /// The main application defined in this add-in
    /// </summary>
    /// <seealso cref="T:Autodesk.Revit.UI.IExternalApplication" />
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class App : IExternalApplication
    {
        protected UIControlledApplication uiControlledApplication;

        /// <summary>
        /// Initializes the <see cref="App"/> class.
        /// </summary>
        static App()
        {
#if WINFORMS
            global::System.Windows.Forms.Application.EnableVisualStyles();
            global::System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
#endif
        }

        /// <summary>
        /// Called when [startup].
        /// </summary>
        /// <param name="uiControlledApplication">The UI control application.</param>
        /// <returns></returns>
        /// ReSharper disable once ParameterHidesMember
        public Result OnStartup(UIControlledApplication uiControlledApplication)
        {
            this.uiControlledApplication = uiControlledApplication;

#if REVIT2017
            // A workaround for a bug with UI culture in Revit 2017.1.1
            // More info here: https://forums.autodesk.com/t5/revit-api-forum/why-the-language-key-switches-currentculture-instead-of/m-p/6843557/highlight/true#M20779
            var language = uiControlledApplication.ControlledApplication.Language.ToString();
            Thread.CurrentThread.CurrentUICulture = CultureInfo
                                                        .GetCultures(CultureTypes.SpecificCultures)
                                                        .FirstOrDefault(c => language.Contains(c.EnglishName)) ?? Thread.CurrentThread.CurrentUICulture;
#endif

            InitializeRibbon();

            try
            {
                // TODO: add you code here
            }
            catch (Exception ex)
            {
                TaskDialog.Show($"Error in {nameof(OnStartup)} method", ex.ToString());
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// Called when [shutdown].
        /// </summary>
        /// <param name="uiCtrlApp">The application.</param>
        /// <returns></returns>
        public Result OnShutdown(UIControlledApplication uiCtrlApp)
        {
            try
            {
                // TODO: add you code here
            }
            catch (Exception ex)
            {
                TaskDialog.Show($"Error in {nameof(OnShutdown)} method", ex.ToString());
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        private void InitializeRibbon()
        {
            // TODO declare your ribbon items here
            var ribbonItems = new List<RibbonHelper.RibbonButton>
            {
                new RibbonHelper.RibbonButton<SettingsCommand>                              // One can reference commands defined in other assemblies
                {
                    Text = StringLocalizer.CallingAssembly["Settings"],                     // Text displayed on the command, can be stored in the resources
                    Tooltip = StringLocalizer.CallingAssembly["Edit WakaTime settings"],    // Tooltip and long description
                    IconName = "Resources.settings.png",                                    // Path to the image, it's relative to the assembly where the command above is defined
                    AvailabilityClassName = typeof(ZeroDocStateAvailability).FullName,
                },

                new RibbonHelper.RibbonButton<DashboardCommand>                             // One can reference commands defined in other assemblies
                {
                    // You could make your ribbon buttons active with no documenent open/active
                    // Try to create your own class with complex rules on when the given button is active and when it's not
                    Text = StringLocalizer.CallingAssembly["Dashboard"],                    // Text displayed on the command, can be stored in the resources
                    Tooltip = StringLocalizer.CallingAssembly["Go to WakaTime dashboard"],  // Tooltip and long description
                    IconName = "Resources.wakatime.png",                                    // Path to the image, it's relative to the assembly where the command above is defined
                    AvailabilityClassName = typeof(ZeroDocStateAvailability).FullName,
                },
            };

            RibbonHelper.AddButtons(
                uiControlledApplication,
                ribbonItems,
                ribbonPanelName: StringLocalizer.CallingAssembly["WakaTime time tracking"], // The title of the ribbot panel
                ribbonTabName: StringLocalizer.CallingAssembly[nameof(WakaTime)]            // The title of the ribbon tab
            );
        }
    }
}
