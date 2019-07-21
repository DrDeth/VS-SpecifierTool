using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.PlatformUI;

namespace SpecifierTool
{
	[Guid(GeneratorDialog.GeneratorDialogGuidString)]
	public partial class GeneratorDialog : DialogWindow
	{
		public string ChosenSpecifier = "";
		public EReturnCode ReturnCode = EReturnCode.RETURN_CANCELLED;

		public const string GeneratorDialogGuidString = "410950cf-9043-4700-a046-317e473acc20";

		public GeneratorDialog(string helpTopic) : base(helpTopic)
		{
			this.InitializeComponent();
			specList.SelectedIndex = 0;
		}

		public GeneratorDialog() : base()
		{
			this.InitializeComponent();
			specList.SelectedIndex = 0;
			Loaded += (sender, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
			//FocusManager.SetFocusedElement(specList.SelectedItem, 
		}

		[SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
		private void Cancel_Button_Click(object sender, RoutedEventArgs e)
		{
			ReturnCode = EReturnCode.RETURN_CANCELLED;
			Close();
		}

		private void Ok_Button_Click(object sender, RoutedEventArgs e)
		{
			ListBoxItem li = specList.SelectedItem as ListBoxItem;
			ChosenSpecifier = li.Content.ToString();
			ReturnCode = EReturnCode.RETURN_OK;
			Close();
		}
	}
}