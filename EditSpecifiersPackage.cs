//------------------------------------------------------------------------------
// <copyright file="EditSpecifiersPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace SpecifierTool
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
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[InstalledProductRegistration("#110", "#112", "1.4", IconResourceID = 400)] // Info on this package for Help/About
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[Guid(EditSpecifiersPackage.PackageGuidString)]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
	public sealed class EditSpecifiersPackage : AsyncPackage
	{
		/// <summary>
		/// EditSpecifiersPackage GUID string.
		/// </summary>
		public const string PackageGuidString = "72efd7e7-d367-41cc-b492-4f3d1fad7849";

		/// <summary>
		/// Initializes a new instance of the <see cref="EditSpecifiers"/> class.
		/// </summary>
		public EditSpecifiersPackage()
		{
			// Inside this method you can place any initialization code that does not require
			// any Visual Studio service because at this point the package object is created but
			// not sited yet inside Visual Studio environment. The place to do all the other
			// initialization is the Initialize method.
		}

        private static EditSpecifiersPackage _instance = null;
        public static EditSpecifiersPackage Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EditSpecifiersPackage();
                return _instance;
            }
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
			EditSpecifiers.Initialize(this);
            _instance = this;
			return base.InitializeAsync(cancellationToken, progress);
		}

        public System.Collections.Specialized.StringCollection PropertySpecifierList
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.UPROPERTYList;
            }
        }

        public System.Collections.Specialized.StringCollection ClassSpecifierList
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.UCLASSList;
            }
        }

        public System.Collections.Specialized.StringCollection EnumSpecifierList
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.UENUMList;
            }
        }

        public System.Collections.Specialized.StringCollection StructSpecifierList
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.USTRUCTList;
            }
        }

        public System.Collections.Specialized.StringCollection FunctionSpecifierList
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.UFUNCTIONList;
            }
        }

        public System.Collections.Specialized.StringCollection InterfaceSpecifierList
        {
            get
            {
                OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
                return page.UINTERFACEList;
            }
        }

        #endregion
    }
}
