using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using SEModAPI.API.Definitions;

namespace SEModAPI
{
    public class LimitEditButton : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            BlockLimitConfig form = new BlockLimitConfig(DedicatedConfigDefinition.Limits);
            var result = form.ShowDialog();
            if (result == DialogResult.OK)
                DedicatedConfigDefinition.Limits = form.BlockLimits;
            return value;
        }
    }
}
