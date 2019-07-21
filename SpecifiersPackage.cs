//------------------------------------------------------------------------------
// <copyright file="SpecifiersPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System.ComponentModel;

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
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#1110", "#1112", "1.0", IconResourceID = 1400)] // Info on this package for Help/About
	[Guid(SpecifiersPackage.PackageGuidString)]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string)]
	[ProvideOptionPage(typeof(OptionPageGrid),
	"Specifiers Editor", "Specifier Editor Settings", 0, 0, true)]
	public sealed class SpecifiersPackage : Package
	{
		private static SpecifiersPackage _instance;
		public static SpecifiersPackage Instance
		{
			get
			{
				if(_instance == null)
					_instance = new SpecifiersPackage();
				return _instance;
			}
		}

		/// <summary>
		/// SpecifiersPackage GUID string.
		/// </summary>
		public const string PackageGuidString = "ed70a1f8-1d00-472c-b7ac-80d53bb706d1";

		/// <summary>
		/// Initializes a new instance of the <see cref="SpecifiersPackage"/> class.
		/// </summary>
		public SpecifiersPackage()
		{
			// Inside this method you can place any initialization code that does not require
			// any Visual Studio service because at this point the package object is created but
			// not sited yet inside Visual Studio environment. The place to do all the other
			// initialization is the Initialize method.
		}

		#region Package Members

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();
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

		#endregion
	}

	public class OptionPageGrid : DialogPage
	{
		public OptionPageGrid()
		{
			UPROPERTY_Strings = Properties.Settings.Default.UPROPERTY_Specifiers;
			UCLASS_Strings = Properties.Settings.Default.UCLASS_Specifiers;
			UENUM_Strings = Properties.Settings.Default.UENUM_Specifiers;
			USTRUCT_Strings = Properties.Settings.Default.USTRUCT_Specifiers;
			UFUNCTION_Strings = Properties.Settings.Default.UFUNCTION_Specifiers;
		}

		private System.Collections.Specialized.StringCollection UPROPERTY_Strings;
		private System.Collections.Specialized.StringCollection UCLASS_Strings;
		private System.Collections.Specialized.StringCollection UENUM_Strings;
		private System.Collections.Specialized.StringCollection USTRUCT_Strings;
		private System.Collections.Specialized.StringCollection UFUNCTION_Strings;

		[Category("Specifiers")]
		[DisplayName("UPROPERTY Specifiers")]
		[Description("List of specifiers that appear for UPROPERTY")]
		public System.Collections.Specialized.StringCollection UPROPERTYList
		{
			get { return UPROPERTY_Strings; }
			set { UPROPERTY_Strings = value; }
		}

		[Category("Specifiers")]
		[DisplayName("UCLASS Specifiers")]
		[Description("List of specifiers that appear for UCLASS")]
		public System.Collections.Specialized.StringCollection UCLASSList
		{
			get { return UCLASS_Strings; }
			set { UCLASS_Strings = value; }
		}

		[Category("Specifiers")]
		[DisplayName("UENUM Specifiers")]
		[Description("List of specifiers that appear for UENUM")]
		public System.Collections.Specialized.StringCollection UENUMList
		{
			get { return UENUM_Strings; }
			set { UENUM_Strings = value; }
		}

		[Category("Specifiers")]
		[DisplayName("USTRUCT Specifiers")]
		[Description("List of specifiers that appear for USTRUCT")]
		public System.Collections.Specialized.StringCollection USTRUCTList
		{
			get { return USTRUCT_Strings; }
			set { USTRUCT_Strings = value; }
		}

		[Category("Specifiers")]
		[DisplayName("UFUNCTION Specifiers")]
		[Description("List of specifiers that appear for UFUNCTION")]
		public System.Collections.Specialized.StringCollection UFUNCTIONList
		{
			get { return UFUNCTION_Strings; }
			set { UFUNCTION_Strings = value; }
		}
	}
}
