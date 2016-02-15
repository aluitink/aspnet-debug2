//------------------------------------------------------------------------------
// <copyright file="AttachDebuggerPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using aspnet_debug.Debugger.VisualStudio;
using aspnet_debug.Extension.Settings;
using aspnet_debug.Shared.Server;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.Win32;
using Process = System.Diagnostics.Process;

namespace aspnet_debug.Extension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(AttachDebuggerPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class AttachDebuggerPackage : Package
    {
        /// <summary>
        /// AttachDebuggerPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "e3003c59-ca60-4918-9577-1f0989a1d128";
        
        
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            var settingsManager = new ShellSettingsManager(this);
            var configurationSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            UserSettingsManager.Initialize(configurationSettingsStore);
            
            TryRegisterAssembly();


            //Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            //{
            //    Source = new Uri("/aspnet_debug.VSExtension;component/Resources/Resources.xaml", UriKind.Relative)
            //});

            var dte = (DTE) GetService(typeof (DTE));
            AttachDebugger.Initialize(this, dte);
            base.Initialize();
        }

        private void TryRegisterAssembly()
        {
            try
            {
                RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(@"CLSID\{8BF3AB9F-3864-449A-93AB-E7B0935FC8F5}");

                if (regKey != null)
                    return;

                string location = typeof(DebuggedProcess).Assembly.Location;

                string regasm = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe";
                if (!Environment.Is64BitOperatingSystem)
                    regasm = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe";

                var p = new ProcessStartInfo(regasm, location);
                p.Verb = "runas";
                p.RedirectStandardOutput = true;
                p.UseShellExecute = false;
                p.CreateNoWindow = true;

                Process proc = Process.Start(p);
                while (!proc.HasExited)
                {
                    string txt = proc.StandardOutput.ReadToEnd();
                }

                using (RegistryKey config = VSRegistry.RegistryRoot(__VsLocalRegistryType.RegType_Configuration))
                {
                    MonoDebuggerInstaller.RegisterDebugEngine(location, config);
                }
            }
            catch (UnauthorizedAccessException)
            {
                //MessageBox.Show(
                //    "Failed finish installation of MonoRemoteDebugger - Please run Visual Studio once as Administrator...",
                //    "MonoRemoteDebugger", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
            }
        }

        #endregion
    }
}
