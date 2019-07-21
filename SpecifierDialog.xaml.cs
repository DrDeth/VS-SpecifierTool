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
using Microsoft.Practices.ServiceLocation;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Globalization;
using EnvDTE;

namespace SpecifierTool
{
	public enum ESpecifierMode
	{
		MODE_UPROPERTY = 0,
		MODE_UCLASS,
		MODE_USTRUCT,
		MODE_UENUM,
		MODE_UFUNCTION,
	};

	public enum ESpecifierType
	{
		TYPE_STRING = 0,
		TYPE_BOOL,
	};

	public enum EReturnCode
	{
		RETURN_CANCELLED = 0,
		RETURN_OK,
	};

	[Guid(SpecifierDialog.SpecifierDialogGuidString)]
	public partial class SpecifierDialog : DialogWindow
	{
		public const string SpecifierDialogGuidString = "44d968db-133c-4689-9049-f77cc4ffc512";
		public ObservableCollection<BoolStringClass> TheList { get; set; }

		public EReturnCode ReturnCode;
		public ESpecifierMode currentMode;

		public class BoolStringClass
		{
			public string TheText { get; set; }
			public int TheValue { get; set; }
			public bool? TheCheck { get; set; }
		}

		public SpecifierDialog(ESpecifierMode mode, string property, string parameters) : base()
		{
			this.InitializeComponent();

			SpecifierListBox.SelectionMode = SelectionMode.Multiple;

			OutputResult = "";
			ReturnCode = EReturnCode.RETURN_CANCELLED;
			m_Data = new SortedDictionary<string, SpecifierData>();

			Title += " - " + property;

			MetaScroller.Content = metaPanel;

			CreateCheckBoxList(mode);
			currentMode = mode;
			ParseExistingSpecifier(parameters);
		}

		public SpecifierDialog(string helpTopic) : base(helpTopic)
		{
			this.InitializeComponent();
			//CreateCheckBoxList(0);
		}

		[SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
		private void Ok_Button_Click(object sender, RoutedEventArgs e)
		{
			//MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()), "SpecifierDialog");
			ReturnCode = EReturnCode.RETURN_OK;
			OutputResult = CompileSpecifier();
			Close();
		}
		private void Cancel_Button_Click(object sender, RoutedEventArgs e)
		{
			//MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()), "SpecifierDialog");
			Close();
		}

		public void CreateCheckBoxList(ESpecifierMode mode)
		{
			//ESpecifierMode md = (ESpecifierMode)mode;
			System.Collections.Specialized.StringCollection specifierList = new System.Collections.Specialized.StringCollection();
			switch(mode)
			{
			case ESpecifierMode.MODE_UPROPERTY:
				specifierList = SpecifiersPackage.Instance.PropertySpecifierList; //Properties.Settings.Default.UPROPERTY_Specifiers;
				break;
			case ESpecifierMode.MODE_UENUM:
				specifierList = SpecifiersPackage.Instance.EnumSpecifierList; //Properties.Settings.Default.UPROPERTY_Specifiers;
				break;
			case ESpecifierMode.MODE_USTRUCT:
				specifierList = SpecifiersPackage.Instance.StructSpecifierList; //Properties.Settings.Default.UPROPERTY_Specifiers;
				break;
			case ESpecifierMode.MODE_UCLASS:
				specifierList = SpecifiersPackage.Instance.ClassSpecifierList; //Properties.Settings.Default.UPROPERTY_Specifiers;
				break;
			case ESpecifierMode.MODE_UFUNCTION:
				specifierList = SpecifiersPackage.Instance.FunctionSpecifierList; //Properties.Settings.Default.UPROPERTY_Specifiers;
				break;
			}

			CreateMeta(mode);
			
			TheList = new ObservableCollection<BoolStringClass>();
			for(int i = 0; i < specifierList.Count; ++i)
			{
				TheList.Add(new BoolStringClass { TheText = specifierList[i], TheValue = i + 1, TheCheck = false });
			}

			this.DataContext = this;
		}

		public void CreateMeta(ESpecifierMode mode)
		{
			if(mode == ESpecifierMode.MODE_UPROPERTY)
			{
				AddMetaTextBox("DisplayName");
				AddMetaTextBox("Category", false);
				AddMetaTextBox("Config", false, false);
				AddMetaTextBox("ReplicatedUsing", false, false);
				AddMetaTextBox("BitmaskEnum");
				AddMetaCheckBox("Bitmask");
				AddMetaCheckBox("AllowAbstract");
				AddMetaCheckBox("AllowClasses");
				AddMetaCheckBox("AllowPreserveRatio");
				AddMetaCheckBox("ArrayClamp");
				AddMetaCheckBox("DisplayThumbnail");
				AddMetaTextBox("EditCondition", true, false);
				AddMetaCheckBox("ExactClass");
				AddMetaCheckBox("ExposeOnSpawn");
				AddMetaCheckBox("HideAlphaChannel");
				AddMetaCheckBox("MultiLine");
				AddMetaCheckBox("NoElementDuplicate");
				AddMetaCheckBox("NoSpinbox");
				AddMetaCheckBox("FilePathFilter");
				AddMetaCheckBox("RelativePath");
				AddMetaCheckBox("ShowOnlyInnerProperties");

				// Will have to add MetaInteger scroll viewer for ranged values
				//AddMetaCheckBox("ClampMin");
				//AddMetaCheckBox("ClampMax");
			}
			else if(mode == ESpecifierMode.MODE_UCLASS)
			{
				// Not implemented at the moment
				//AddMetaTextBox("HideCategories");
				//AddMetaTextBox("HideFunctions");
				//AddMetaTextBox("ShowCategories");
				//AddMetaTextBox("AutoCollapseCategories");
				//AddMetaTextBox("AutoExpandCategories");
				AddMetaTextBox("Within", false, false);
				AddMetaCheckBox("BlueprintSpawnableComponent");
			}
			else if(mode == ESpecifierMode.MODE_UENUM)
			{
				AddMetaCheckBox("Bitflags");
			}
			else if(mode == ESpecifierMode.MODE_UFUNCTION)
			{
				AddMetaTextBox("Category", false);
				AddMetaTextBox("CustomThunk", false, false);
			}
			else if(mode == ESpecifierMode.MODE_USTRUCT)
			{
				// Not implemented at the moment
				//AddMetaTextBox("HideCategories");
				//AddMetaTextBox("HideFunctions");
				//AddMetaTextBox("ShowCategories");
				//AddMetaTextBox("AutoCollapseCategories");
				//AddMetaTextBox("AutoExpandCategories");
			}
		}

		public void AddMetaTextBox(string name, bool isMeta = true, bool quotedValue = true)
		{
			StackPanel panel = new StackPanel();
			panel.Orientation = Orientation.Horizontal;
			panel.HorizontalAlignment = HorizontalAlignment.Stretch;
			panel.Margin = new Thickness(0, 4, 4, 4);

			Label label = new Label();
			label.Content = name;
			label.Foreground = this.FindResource(SystemColors.WindowBrushKey) as Brush;
			label.HorizontalAlignment = HorizontalAlignment.Left;
			label.Width = 120.0;
			label.Margin = new Thickness(4, 0, 4, 0);

			TextBox textBox = new TextBox();
			textBox.Name = name + "_TextBox";
			textBox.HorizontalAlignment = HorizontalAlignment.Right;
			textBox.Width = 130.0;
			textBox.MinWidth = 130.0;
			textBox.MaxWidth = 130.0;
			textBox.Margin = new Thickness(0, 0, 4, 0);
			
			panel.Children.Add(label);
			panel.Children.Add(textBox);
			//panel.RegisterName(textBox.Name, textBox);

			if(NameScope.GetNameScope(textBox) == null)
				NameScope.SetNameScope(textBox, new NameScope());
			NameScope.GetNameScope(textBox).RegisterName(textBox.Name, textBox);

			metaPanel.Children.Add(panel);

			SpecifierData data = new SpecifierData();
			data.Type = ESpecifierType.TYPE_STRING;
			data.Name = name;
			data.textBox = textBox;
			data.Quoted = quotedValue;
			data.WithinMeta = isMeta;

			m_Data.Add(name, data);
		}

		public void AddMetaCheckBox(string name)
		{
			StackPanel panel = new StackPanel();
			panel.Orientation = Orientation.Horizontal;
			panel.HorizontalAlignment = HorizontalAlignment.Stretch;
			panel.Margin = new Thickness(0, 4, 4, 4);

			CheckBox checkbox = new CheckBox();
			checkbox.Content = name;
			checkbox.Name = name + "_CheckBox";
			checkbox.HorizontalAlignment = HorizontalAlignment.Right;
			checkbox.Margin = new Thickness(8, 0, 4, 0);
			checkbox.Foreground = this.FindResource(SystemColors.WindowBrushKey) as Brush;

			panel.Children.Add(checkbox);

			if(NameScope.GetNameScope(checkbox) == null)
				NameScope.SetNameScope(checkbox, new NameScope());
			NameScope.GetNameScope(checkbox).RegisterName(checkbox.Name, checkbox);

			metaPanel.Children.Add(panel);
			
			SpecifierData data = new SpecifierData();
			data.Type = ESpecifierType.TYPE_BOOL;
			data.Name = name;
			data.checkBox = checkbox;
			data.WithinMeta = true;	// All checkboxes are presumed WithinMeta given that, otherwise, they would be standard specifiers

			m_Data.Add(name, data);
		}

		private void CheckBoxZone_Checked(object sender, RoutedEventArgs e)
		{
			CheckBox chkZone = (CheckBox)sender;
			//ZoneText.Text = "Selected Zone Name= " + chkZone.Content.ToString();
			//ZoneValue.Text = "Selected Zone Value= " + chkZone.Tag.ToString();
		}

		private sealed class SpecifierData
		{
			public ESpecifierType Type;
			public TextBox textBox;
			public CheckBox checkBox;
			public string Name;
			public bool Quoted;
			public bool WithinMeta;

			public SpecifierData()
			{
				Type = ESpecifierType.TYPE_STRING;
				Name = "";
				Quoted = false;
				WithinMeta = false;
			}
		};

		private SortedDictionary<string, SpecifierData> m_Data;

		public string OutputResult;

		private string CompileSpecifier()
		{
			string result = "";
			bool firstEntry = true;

			switch(currentMode)
			{
			case ESpecifierMode.MODE_UPROPERTY: result += "UPROPERTY("; break;
			case ESpecifierMode.MODE_UENUM:		result += "UENUM("; break;
			case ESpecifierMode.MODE_USTRUCT:	result += "USTRUCT("; break;
			case ESpecifierMode.MODE_UFUNCTION: result += "UFUNCTION("; break;
			case ESpecifierMode.MODE_UCLASS:	result += "UCLASS("; break;
			}

			foreach(BoolStringClass boolstring in SpecifierListBox.Items)
			{
				if(boolstring.TheCheck.HasValue && boolstring.TheCheck.Value)
				{
					if(!firstEntry)
						result += ", ";
					else
						firstEntry = false;
					result += boolstring.TheText;
				}
			}

			// First process non-meta stuff
			foreach(KeyValuePair<string, SpecifierData> entry in m_Data)
			{
				if(entry.Value.WithinMeta)
					continue;

				/*
				if(entry.Value.Type == ESpecifierType.TYPE_BOOL)
				{
					if(entry.Value.checkBox.IsChecked.HasValue && entry.Value.checkBox.IsChecked.Value)
						result += entry.Value.Name;
				}
				*/
				if(entry.Value.Type == ESpecifierType.TYPE_STRING)
				{
					if(!string.IsNullOrWhiteSpace(entry.Value.textBox.Text))
					{
						if(!firstEntry)
							result += ", ";
						else
							firstEntry = false;
						result += entry.Value.Name;
						result += " = ";
						if(entry.Value.Quoted)
							result += "\"";
						result += entry.Value.textBox.Text;
						if(entry.Value.Quoted)
							result += "\"";
					}
				}
			}

			// Now do meta stuff
			bool wroteMeta = false;
			foreach(KeyValuePair<string, SpecifierData> entry in m_Data)
			{
				if(!entry.Value.WithinMeta)
					continue;

				if(entry.Value.Type == ESpecifierType.TYPE_BOOL)
				{
					if(entry.Value.checkBox.IsChecked.HasValue && entry.Value.checkBox.IsChecked.Value)
					{
						if(!firstEntry)
							result += ", ";
						else
							firstEntry = false;
						if(!wroteMeta)
						{
							wroteMeta = true;
							result += "meta = (";
						}
						result += entry.Value.Name;
					}
				}
				else if(entry.Value.Type == ESpecifierType.TYPE_STRING)
				{
					if(!string.IsNullOrWhiteSpace(entry.Value.textBox.Text))
					{
						if(!firstEntry)
							result += ", ";
						else
							firstEntry = false;
						if(!wroteMeta)
						{
							wroteMeta = true;
							result += "meta = (";
						}
						result += entry.Value.Name;
						result += " = ";
						if(entry.Value.Quoted)
							result += "\"";
						result += entry.Value.textBox.Text;
						if(entry.Value.Quoted)
							result += "\"";
					}
				}
			}

			// Close the meta section if needed
			if(wroteMeta)
				result += ")";

			result += ")";
			return result;
		}

		void ParseExistingSpecifier(string parameters)
		{
			if(string.IsNullOrWhiteSpace(parameters))
				return;
			
			// Go through normal specifier tags
			foreach(BoolStringClass boolstring in TheList)
			{
				if(parameters.Contains(boolstring.TheText))
				{
					boolstring.TheCheck = true;
				}
			}

			// Now do meta specifier things
			foreach(KeyValuePair<string, SpecifierData> entry in m_Data)
			{
				if(parameters.Contains(entry.Value.Name))
				{
					if(entry.Value.Type == ESpecifierType.TYPE_BOOL)
						entry.Value.checkBox.IsChecked = true;
					else if(entry.Value.Type == ESpecifierType.TYPE_STRING)
					{
						int startPos = parameters.IndexOf(entry.Value.Name);
						if(startPos == -1)
							continue;
						string splitoff = parameters.Substring(startPos);
						char[] seperators = { '=', ',', '(', ')' };
						string[] pairs = splitoff.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
						pairs[1] = pairs[1].Replace('\"', ' ');
						pairs[1] = pairs[1].Trim();

						entry.Value.textBox.Text = pairs[1];
					}
				}
			}
		}
	}
}