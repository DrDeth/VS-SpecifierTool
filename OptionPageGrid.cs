using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecifierTool
{
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
