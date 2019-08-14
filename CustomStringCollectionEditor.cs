using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecifierTool
{
    public class CustomStringCollectionEditor : CollectionEditor
    {
        public CustomStringCollectionEditor() : base(type: typeof(StringCollection)) { }
        protected override object CreateInstance(Type itemType)
        {
            return string.Empty;
        }
    }
}
