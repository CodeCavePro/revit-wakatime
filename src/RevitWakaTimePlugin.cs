using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using WakaTime;
using CodeCave.WakaTime.Revit;

namespace CodeCave.WakaTime.Revit
{
    public class RevitWakaTimePlugin : WakaTimeIdePlugin<UIApplication>, IDisposable, IWin32Window
    {
        protected bool disposed;

        private SettingsForm _settingsForm;
        private ApiKeyForm _apiKeyForm;
        private DownloadProgressForm _downloadProgressForm;

        public IntPtr Handle =>
#if REVIT2020 || REVIT2019
            editorObj.MainWindowHandle;
#else
            System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
#endif

        #region Constructor / Destructor

        public RevitWakaTimePlugin(UIApplication editor)
            : base(editor)
        {

        }

        ~RevitWakaTimePlugin() // the finalizer
        {
            Dispose(false);
        }

        public override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (_settingsForm != null && !_settingsForm.IsDisposed)
                    _settingsForm.Dispose();

                if (_apiKeyForm != null && !_apiKeyForm.IsDisposed)
                    _apiKeyForm.Dispose();

                if (_downloadProgressForm != null && !_downloadProgressForm.IsDisposed)
                    _downloadProgressForm.Dispose();
            }

            // Idling and initialization
            editorObj.Idling -= OnIdling;
            editorObj.Application.ApplicationInitialized -= OnApplicationInitialized;
            // Open / change
            editorObj.Application.DocumentOpened -= OnDocumentOpened;
            editorObj.Application.DocumentChanged -= OnDocumentChanged;
            // Save / SaveAs
            editorObj.Application.DocumentSaved -= OnDocumentSaved;
            editorObj.Application.DocumentSavedAs -= OnDocumentSavedAs;
            // Progress & Failure
            editorObj.Application.ProgressChanged -= OnProgressChanged;
            editorObj.Application.FailuresProcessing -= OnFailuresProcessing;
            // Closing
            editorObj.Application.DocumentClosing -= OnDocumentClosing;
            editorObj.Application.DocumentClosed -= OnDocumentClosed;
            // Views
            editorObj.ViewActivated -= OnViewActivated;

            // Release unmanaged resources.
            // Set large fields to null.
            disposed = true;
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        #endregion

        #region Logger & Editor info

        /// <summary>
        /// Gets the active solution path.
        /// </summary>
        /// <returns></returns>
        public override string GetActiveSolutionPath()
        {
            var activeDocumentPath = editorObj?.ActiveUIDocument?.Document?.PathName;
            return (string.IsNullOrWhiteSpace(activeDocumentPath))
                ? null
                : Directory.GetParent(activeDocumentPath)?.FullName;
        }

        /// <summary>
        /// Gets the editor information.
        /// </summary>
        /// <returns></returns>
        public override EditorInfo GetEditorInfo() {

            return editorInfo ?? (editorInfo = new EditorInfo
            {
                Name = "Autodesk Revit",
                Version = new Version(int.Parse(editorObj.Application.VersionNumber), int.Parse(editorObj.Application.VersionBuild)),
                PluginKey = "autodesk-revit",
                PluginName = "WakaTime",
                PluginVersion = GetType().Assembly.GetName().Version
            });
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <returns></returns>
        public override ILogService GetLogger()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Forms

        /// <summary>
        /// Gets download progress reporter.
        /// </summary>
        /// <returns></returns>
        public override IDownloadProgressReporter GetReporter() => new DownloadProgressForm(this);

        public override void PromptApiKey()
        {
            if (_apiKeyForm == null)
                _apiKeyForm = new ApiKeyForm();

            _apiKeyForm.ShowDialog();
        }

        /// <summary>
        /// Handle settingses popup.
        /// </summary>
        public override void SettingsPopup()
        {
            if (_settingsForm == null)
                _settingsForm = new SettingsForm();

            _settingsForm.ShowDialog();
        }

        #endregion

        #region IDE events

        /// <summary>
        /// Binds the editor events.
        /// </summary>
        public override void BindEditorEvents()
        {
            // Idling and initialization
            editorObj.Idling += OnIdling;
            editorObj.Application.ApplicationInitialized += OnApplicationInitialized;
            // Open / change
            editorObj.Application.DocumentOpened += OnDocumentOpened;
            editorObj.Application.DocumentChanged += OnDocumentChanged;
            // Save / SaveAs
            editorObj.Application.DocumentSaved += OnDocumentSaved;
            editorObj.Application.DocumentSavedAs += OnDocumentSavedAs;
            // Progress & Failure
            editorObj.Application.ProgressChanged += OnProgressChanged;
            editorObj.Application.FailuresProcessing += OnFailuresProcessing;
            // Closing
            editorObj.Application.DocumentClosing += OnDocumentClosing;
            editorObj.Application.DocumentClosed += OnDocumentClosed;
            // Views
            editorObj.ViewActivated += OnViewActivated;
        }

        /// <summary>
        /// Called when [idling].
        /// This event is raised only when the Revit UI is in a state
        /// when there is no active document.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="T:Autodesk.Revit.UI.Events.IdlingEventArgs" /> instance containing the event data.</param>
        /// ReSharper disable once MemberCanBeMadeStatic.Local
        private void OnIdling(object sender, IdlingEventArgs args)
        {
            // TODO: add you code here
        }

        /// <summary>
        /// Called when [application initialized].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="T:Autodesk.Revit.DB.Events.ApplicationInitializedEventArgs" /> instance containing the event data.</param>
        /// ReSharper disable once MemberCanBeMadeStatic.Local
        private void OnApplicationInitialized(object sender, ApplicationInitializedEventArgs args)
        {
            // TODO: add you code here
        }

        /// <summary>
        /// Called when [document opened].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DocumentOpenedEventArgs" /> instance containing the event data.</param>
        private void OnDocumentOpened(object sender, DocumentOpenedEventArgs args)
        {
            // TODO: this is just an example, remove or change code below
            var doc = args.Document;
            Debug.Assert(null != doc, $"Expected a valid Revit {nameof(Document)} instance");

            // TODO: this is just an example, remove or change code below
            var app = args.Document?.Application;
            var uiapp = new UIApplication(app);
            Debug.Assert(null != uiapp, $"Expected a valid Revit {nameof(UIApplication)} instance");
        }

        /// <summary>
        /// Called when [document changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:Autodesk.Revit.DB.Events.DocumentChangedEventArgs" /> instance containing the event data.</param>
        /// ReSharper disable once MemberCanBeMadeStatic.Local
        private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            // TODO: add you code here
        }

        /// <summary>
        /// Called when [document saved].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DocumentSavedEventArgs" /> instance containing the event data.</param>
        /// ReSharper disable once MemberCanBeMadeStatic.Local
        private void OnDocumentSaved(object sender, DocumentSavedEventArgs args)
        {
            // TODO: add you code here
        }

        /// <summary>
        /// Called when [document saved as].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DocumentSavedAsEventArgs" /> instance containing the event data.</param>
        /// ReSharper disable once MemberCanBeMadeStatic.Local
        private void OnDocumentSavedAs(object sender, DocumentSavedAsEventArgs args)
        {
            // TODO: add you code here
        }

        /// <summary>
        /// Called when [failures processing].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FailuresProcessingEventArgs" /> instance containing the event data.</param>
        /// ReSharper disable once MemberCanBeMadeStatic.Local
        private void OnFailuresProcessing(object sender, FailuresProcessingEventArgs e)
        {
            // TODO: add you code here
        }

        /// <summary>
        /// Called when [progress changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="progressChangedEventArgs">The <see cref="ProgressChangedEventArgs"/> instance containing the event data.</param>
        /// ReSharper disable once MemberCanBeMadeStatic.Local
        private void OnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            // TODO: add you code here
        }

        /// <summary>
        /// Called when [document closing].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DocumentClosingEventArgs" /> instance containing the event data.</param>
        /// ReSharper disable once MemberCanBeMadeStatic.Local
        private void OnDocumentClosing(object sender, DocumentClosingEventArgs args)
        {
            // TODO: add you code here
        }

        /// <summary>
        /// Called when [document closed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DocumentClosedEventArgs" /> instance containing the event data.</param>
        /// ReSharper disable once MemberCanBeMadeStatic.Local
        private void OnDocumentClosed(object sender, DocumentClosedEventArgs args)
        {
            // TODO: add you code here
        }

        /// <summary>
        /// Called when [view activated].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ViewActivatedEventArgs" /> instance containing the event data.</param>
        private void OnViewActivated(object sender, ViewActivatedEventArgs e)
        {
            // TODO: add you code here
        }

        #endregion
    }
}
