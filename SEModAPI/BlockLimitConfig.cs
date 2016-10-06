using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using NLog;
using Sandbox.Definitions;
using VRage.Dedicated;

namespace SEModAPI
{
    public partial class BlockLimitConfig : Form
    {
        public Dictionary<string, short> BlockLimits;
        private List<string> blockTypeNames=new List<string>();
        public BlockLimitConfig( Dictionary<string, short> limits )
        {
            InitializeComponent();

            BlockLimits = limits;
            
                var blockTypeResources = new System.ComponentModel.ComponentResourceManager(typeof(BlockTypeList));
                blockTypeNames=blockTypeResources.GetString("textBox1.Text").Split(',').ToList();
            
            if (MyDefinitionManager.Static != null)
            {
                foreach (var def in MyDefinitionManager.Static.GetAllDefinitions())
                {
                    var cubeDef = def as MyCubeBlockDefinition;
                    if (cubeDef == null)
                        continue;
                    if (!cubeDef.Public)
                        continue;
                    string name = cubeDef.BlockPairName;
                    if (!blockTypeNames.Contains(name))
                        blockTypeNames.Add(name);
                }
            }
            blockTypeNames.Sort();
            TP_Block_Limits.RowStyles.Clear();
            bool first = true;
            foreach (var entry in BlockLimits.OrderBy( e=>e.Key))
            {
                if (first)
                {
                    first = false;
                    TP_Block_Limits.RowCount = 1;
                    var fp = CreateRow(entry.Key, entry.Value);
                    TP_Block_Limits.Controls.Add(fp);
                    TP_Block_Limits.SetRow(fp, 0);
                    continue;
                }
                AddRow(CreateRow(entry.Key, entry.Value));
            }
        }
        private static readonly Logger BaseLog = LogManager.GetLogger("BaseLog");
        private void AddRow(FlowLayoutPanel fp)
        {
            TP_Block_Limits.RowCount++;
            TP_Block_Limits.Controls.Add(fp);
            TP_Block_Limits.SetRow(fp, TP_Block_Limits.RowCount - 1);
        }

        private FlowLayoutPanel CreateRow(string name = "", short value = 0)
        {
            FlowLayoutPanel fp = new FlowLayoutPanel();
            fp.FlowDirection=FlowDirection.LeftToRight;
            fp.AutoSize = true;
            fp.Margin = new Padding(0);

            ComboBox blockNames = new ComboBox();
            NumericUpDown limitCount = new NumericUpDown();
            Button removeButton = new Button();

            blockNames.Width = 190;
            blockNames.Text = name;
            blockNames.Items.AddRange(blockTypeNames.ToArray());

            limitCount.Minimum = 0;
            limitCount.Maximum = short.MaxValue;
            limitCount.Value = value;
            limitCount.Width = 50;

            removeButton.Text = "Remove";
            removeButton.Click += RemoveButton_Click;
            removeButton.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            fp.Controls.Add(blockNames);
            fp.Controls.Add(limitCount);
            fp.Controls.Add(removeButton);

            return fp;
        }
        
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var panel = (FlowLayoutPanel)button.Parent;
            
            var cmb = panel.Controls[0] as ComboBox;
            cmb.Text = string.Empty;

            var nud = panel.Controls[1] as NumericUpDown;
            nud.Value = 0;

            //we can't remove rows from the table easily, so just set this panel invisible
            panel.Visible = false;
        }
        
        private void BTN_Limit_Add_Click(object sender, EventArgs e)
        {
            AddRow(CreateRow());
        }

        private void BTN_Limit_Save_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            BlockLimits.Clear();
            foreach (var control in TP_Block_Limits.Controls)
            {
                var panel = (FlowLayoutPanel)control;
                string type = ((ComboBox)panel.Controls[0]).Text;
                if (string.IsNullOrEmpty(type))
                    continue;

                short count = (short)((NumericUpDown)panel.Controls[1]).Value;

                BlockLimits.Add(type, count);
            }
        }
    }
}
