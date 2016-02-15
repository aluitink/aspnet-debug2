//------------------------------------------------------------------------------
// <copyright file="AttachDebugger.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Windows;
using aspnet_debug.Extension.Views;
using aspnet_debug.Shared.Server;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace aspnet_debug.Extension
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AttachDebugger
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("e506acf1-4db7-4bac-a73f-a29cac7b9dbf");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        private MonoVisualStudioExtension _monoExtension;
        private MonoDebugServer _server = new MonoDebugServer();

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachDebugger"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner _package, not null.</param>
        private AttachDebugger(Package package, DTE dte)
        {
            if (package == null)
                throw new ArgumentNullException("package");

            _package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
            
            _monoExtension = new MonoVisualStudioExtension(dte);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static AttachDebugger Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner _package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this._package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner _package, not null.</param>
        public static void Initialize(Package package, DTE dte)
        {
            Instance = new AttachDebugger(package, dte);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            string title = "AttachDebugger";

            // Show a message box to prove we were here
            //VsShellUtilities.ShowMessageBox(
            //    this.ServiceProvider,
            //    message,
            //    title,
            //    OLEMSGICON.OLEMSGICON_INFO,
            //    OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            StartSearching();
        }

        private async void StartSearching()
        {
            var dlg = new ServersFound();

            if (dlg.ShowDialog().GetValueOrDefault())
            {
                try
                {
                    _monoExtension.BuildSolution();
                    if (dlg.ViewModel.SelectedServer != null)
                        await _monoExtension.AttachDebugger(dlg.ViewModel.SelectedServer.IpAddress.ToString());
                    else if (!string.IsNullOrWhiteSpace(dlg.ViewModel.ManualIp))
                        await _monoExtension.AttachDebugger(dlg.ViewModel.ManualIp);
                }
                catch (Exception ex)
                {
                    //logger.Error(ex);
                    MessageBox.Show(ex.Message, "MonoRemoteDebugger", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
