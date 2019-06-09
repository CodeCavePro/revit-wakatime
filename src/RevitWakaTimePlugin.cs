using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.IO;
using System.Windows.Forms;
using WakaTime;

namespace CodeCave.WakaTime.Revit
{
    public class RevitWakaTimePlugin : WakaTimeIdePlugin<UIApplication>, IDisposable, IWin32Window
    {
        protected bool disposed;

        protected ILogService logger;

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

                // Open / change
                editorObj.Application.DocumentOpened -= OnDocumentOpened;
                editorObj.Application.DocumentChanged -= OnDocumentChanged;
                // Save / SaveAs
                editorObj.Application.DocumentSaved -= OnDocumentSaved;
                editorObj.Application.DocumentSavedAs -= OnDocumentSavedAs;
                // Closing
                editorObj.Application.DocumentClosing -= OnDocumentClosing;
                // Views
                editorObj.ViewActivated -= OnViewActivated;
            }

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
            var activeDocumentPath = editorObj?.ActiveUIDocument.Document?.PathName;
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
                Name = "Revit",
                Version = new Version(editorObj.Application.SubVersionNumber),
                PluginKey = "revit-wakatime",
                PluginName = nameof(WakaTime),
                PluginVersion = GetType().Assembly.GetName().Version
            });
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <returns></returns>
        public override ILogService GetLogger()
        {
            var roamingFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appData = Path.Combine(roamingFolder, "Autodesk", "Revit", $"Autodesk Revit {editorObj.Application.VersionNumber}");
            return logger ?? (logger = new FileLogger(appData));
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
            // Open / change
            editorObj.Application.DocumentOpened += OnDocumentOpened;
            editorObj.Application.DocumentChanged += OnDocumentChanged;
            // Save / SaveAs
            editorObj.Application.DocumentSaved += OnDocumentSaved;
            editorObj.Application.DocumentSavedAs += OnDocumentSavedAs;
            // Closing
            editorObj.Application.DocumentClosing += OnDocumentClosing;
            // Views
            editorObj.ViewActivated += OnViewActivated;
        }

        /// <summary>
        /// Called when [document opened].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DocumentOpenedEventArgs" /> instance containing the event data.</param>
        private void OnDocumentOpened(object sender, DocumentOpenedEventArgs args)
        {
            if (args.Document == null)
                return;

            var solutionPath = (args.Document.IsFamilyDocument)
                ? Directory.GetParent(args.Document.PathName).FullName
                : args.Document.PathName;

            OnSolutionOpened(solutionPath);
            OnDocumentOpened(args.Document.PathName);
        }

        /// <summary>
        /// Called when [document changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DocumentChangedEventArgs"/> instance containing the event data.</param>
        /// ReSharper disable once MemberCanBeMadeStatic.Local
        private void OnDocumentChanged(object sender, DocumentChangedEventArgs args)
        {
            ProcessActivity(args.GetDocument());
        }

        /// <summary>
        /// Called when [document saved].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DocumentSavedEventArgs" /> instance containing the event data.</param>
        /// ReSharper disable once MemberCanBeMadeStatic.Local
        private void OnDocumentSaved(object sender, DocumentSavedEventArgs args)
        {
            ProcessActivity(args.Document);
        }

        /// <summary>
        /// Called when [document saved as].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DocumentSavedAsEventArgs" /> instance containing the event data.</param>
        /// ReSharper disable once MemberCanBeMadeStatic.Local
        private void OnDocumentSavedAs(object sender, DocumentSavedAsEventArgs args)
        {
            ProcessActivity(args.Document);
        }

        /// <summary>
        /// Called when [document closing].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DocumentClosingEventArgs" /> instance containing the event data.</param>
        /// ReSharper disable once MemberCanBeMadeStatic.Local
        private void OnDocumentClosing(object sender, DocumentClosingEventArgs args)
        {
            ProcessActivity(args.Document);
        }

        /// <summary>
        /// Called when [view activated].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ViewActivatedEventArgs"/> instance containing the event data.</param>
        private void OnViewActivated(object sender, ViewActivatedEventArgs args)
        {
            ProcessActivity(args.Document);
        }

        private void ProcessActivity(Document doc)
        {
            if (doc == null)
                return;

            var solutionPath = (doc.IsFamilyDocument)
                ? Directory.GetParent(doc.PathName).FullName
                : doc.PathName;

            OnSolutionOpened(solutionPath);
            OnDocumentChanged(doc.PathName);
        }

        #endregion
    }
}
