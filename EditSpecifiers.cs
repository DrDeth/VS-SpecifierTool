//------------------------------------------------------------------------------
// <copyright file="EditSpecifiers.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.Windows.Media;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Editor;
//using Microsoft.VisualStudio.Editor.DefGuidList;

namespace SpecifierTool
{
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class EditSpecifiers
	{
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 0x0100;
		public const int CommandId2 = 0x0099;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("d2c4f4b0-8ecb-4ed6-b7c6-bbf86c09df13");

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private readonly Package package;

		/// <summary>
		/// Initializes a new instance of the <see cref="EditSpecifiers"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		private EditSpecifiers(Package package)
		{
			if(package == null)
			{
				throw new ArgumentNullException("package");
			}

			this.package = package;

			OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if(commandService != null)
			{
				var menuCommandID = new CommandID(CommandSet, CommandId);
				var menuCommandID2 = new CommandID(CommandSet, CommandId2);
				var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
				var menuItem2 = new MenuCommand(this.GenerateSpecifierCallback, menuCommandID2);
				commandService.AddCommand(menuItem2);
				commandService.AddCommand(menuItem);
			}
		}

		/// <summary>
		/// Gets the instance of the command.
		/// </summary>
		public static EditSpecifiers Instance
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the service provider from the owner package.
		/// </summary>
		private IServiceProvider ServiceProvider
		{
			get
			{
				return this.package;
			}
		}

		/// <summary>
		/// Initializes the singleton instance of the command.
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		public static void Initialize(Package package)
		{
			Instance = new EditSpecifiers(package);
		}

		IWpfTextViewHost GetCurrentViewHost()
		{
			// code to get access to the editor's currently selected text cribbed from
			// http://msdn.microsoft.com/en-us/library/dd884850.aspx
			IVsTextManager txtMgr = (IVsTextManager)this.ServiceProvider.GetService(typeof(SVsTextManager));
			IVsTextView vTextView = null;
			int mustHaveFocus = 1;
			txtMgr.GetActiveView(mustHaveFocus, null, out vTextView);
			IVsUserData userData = vTextView as IVsUserData;
			if(userData == null)
			{
				return null;
			}
			else
			{
				IWpfTextViewHost viewHost;
				object holder;
				Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
				userData.GetData(ref guidViewHost, out holder);
				viewHost = (IWpfTextViewHost)holder;
				return viewHost;
			}
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
			string title = "EditSpecifiers";

			// Show a message box to prove we were here
			/*
			VsShellUtilities.ShowMessageBox(
				this.ServiceProvider,
				message,
				title,
				OLEMSGICON.OLEMSGICON_INFO,
				OLEMSGBUTTON.OLEMSGBUTTON_OK,
				OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
				*/

			DTE dte = (DTE)this.ServiceProvider.GetService(typeof(DTE));
			string text = "";
			string parameters = "";

			//IWpfTextViewHost host = GetCurrentViewHost();
			//text = host.TextView.Selection.SelectedSpans[0].GetText();

			if(dte.ActiveDocument != null)
			{
				var selection = (TextSelection)dte.ActiveDocument.Selection;

				if(text.Length == 0)
				{
					selection.WordLeft();
					selection.WordRight(true);
					text = selection.Text;

					// See if we have an existing specifier definition to parse through
					selection.WordRight(true);
					if(selection.Text.Contains("("))
					{
						//selection.WordRight(false, -1);
						selection.EndOfLine(true);
						parameters = selection.Text;
						int depth = 0;
						bool entered = false;
						int splitpos = 0;
						for(int i = 0; i < parameters.Length; ++i)
						{
							if(parameters[i] == '(')
							{
								depth++;
							}
							else if(parameters[i] == ')')
							{
								depth--;
							}
							if(depth >= 1 && !entered)
								entered = true;
							if(entered && depth <= 0)
							{
								splitpos = i + 1;
								break;
							}
						}
						if(depth != 0)
						{
							VsShellUtilities.ShowMessageBox(
								this.ServiceProvider,
								"Edit Specifiers found parsing errors in an existing specifier\nPlease correct any mismatching parenthesis",
								"",
								OLEMSGICON.OLEMSGICON_INFO,
								OLEMSGBUTTON.OLEMSGBUTTON_OK,
								OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
							return;
						}
						selection.CharRight(true, (-1 * (parameters.Length - splitpos)));
						if(splitpos < parameters.Length)
							parameters = parameters.Remove(splitpos);
					}
					else if(!selection.ActivePoint.AtEndOfDocument)
					{
					//	selection.WordRight(true, -1);
						selection.CharRight(true, -1);
					}
				}					
			}
			else return;

#if !DEBUG
			if(text.Length == 0) return;
#endif
			ESpecifierMode editMode = 0;
			if(text.Contains("UPROPERTY"))
				editMode = ESpecifierMode.MODE_UPROPERTY;
			else if(text.Contains("UFUNCTION"))
				editMode = ESpecifierMode.MODE_UFUNCTION;
			else if(text.Contains("UCLASS"))
				editMode = ESpecifierMode.MODE_UCLASS;
			else if(text.Contains("UENUM"))
				editMode = ESpecifierMode.MODE_UENUM;
			else if(text.Contains("USTRUCT"))
				editMode = ESpecifierMode.MODE_USTRUCT;
			else
			{
				VsShellUtilities.ShowMessageBox(
					this.ServiceProvider,
					"Edit Specifiers did not find a correct UE4 specifier.\nValid Specifiers are as follows:\n\nUPROPERTY\nUFUNCTION\nUENUM\nUCLASS\nUSTRUCT",
					"",
					OLEMSGICON.OLEMSGICON_INFO,
					OLEMSGBUTTON.OLEMSGBUTTON_OK,
					OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
				return;
			}

			SpecifierDialog dialog = new SpecifierDialog(editMode, text, parameters);
			dialog.ShowModal();
			if(dialog.ReturnCode == EReturnCode.RETURN_OK)
			{
				if(!string.IsNullOrWhiteSpace(dialog.OutputResult))
				{
					var txt = (TextSelection)dte.ActiveDocument.Selection;
					txt.Text = dialog.OutputResult;
				}
			}
		}

		private void GenerateSpecifierCallback(object sender, EventArgs e)
		{
			GeneratorDialog dialog = new GeneratorDialog();
			dialog.ShowModal();

			if(dialog.ReturnCode == EReturnCode.RETURN_OK)
			{
				string text = dialog.ChosenSpecifier;

				ESpecifierMode editMode = 0;
				if(text.Contains("UPROPERTY"))
					editMode = ESpecifierMode.MODE_UPROPERTY;
				else if(text.Contains("UFUNCTION"))
					editMode = ESpecifierMode.MODE_UFUNCTION;
				else if(text.Contains("UCLASS"))
					editMode = ESpecifierMode.MODE_UCLASS;
				else if(text.Contains("UENUM"))
					editMode = ESpecifierMode.MODE_UENUM;
				else if(text.Contains("USTRUCT"))
					editMode = ESpecifierMode.MODE_USTRUCT;

				SpecifierDialog dialog2 = new SpecifierDialog(editMode, text + " (Generated)", string.Empty);
				dialog2.ShowModal();

				if(dialog2.ReturnCode == EReturnCode.RETURN_OK)
				{
					if(!string.IsNullOrWhiteSpace(dialog2.OutputResult))
					{
						DTE dte = (DTE)this.ServiceProvider.GetService(typeof(DTE));
						var txt = (TextSelection)dte.ActiveDocument.Selection;
						txt.Text = dialog2.OutputResult;
					}
				}
			}

		}
	}
}
