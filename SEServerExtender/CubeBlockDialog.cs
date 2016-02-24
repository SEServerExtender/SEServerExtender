using VRage.Game;

namespace SEServerExtender
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Windows.Forms;
	using Sandbox.Definitions;
	using SEModAPIInternal.API.Common;
	using SEModAPIInternal.API.Entity.Sector.SectorObject;
	using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
	using SEModAPIInternal.Support;
	using VRage.Collections;
	using VRageMath;

	public partial class CubeBlockDialog : Form
	{
		#region "Attributes"

		#endregion

		#region "Constructors and Initializers"

		public CubeBlockDialog()
		{
			InitializeComponent();

			CMB_BlockType.BeginUpdate();
			foreach (var entry in BlockRegistry.Instance.TypeMap)
			{
				CMB_BlockType.Items.Add(entry.Value.Name);
			}
			CMB_BlockType.EndUpdate();

			TXT_Position_X.Text = "0";
			TXT_Position_Y.Text = "0";
			TXT_Position_Z.Text = "0";
		}

		#endregion

		#region "Properties"

		public CubeGridEntity ParentCubeGrid { get; set; }

		public KeyValuePair<Type, Type> SelectedType
		{
			get
			{
				KeyValuePair<Type, Type> entry = BlockRegistry.Instance.TypeMap.ElementAt(CMB_BlockType.SelectedIndex);
				return entry;
			}
		}

		public string SelectedSubType
		{
			get { return (string) CMB_BlockSubType.SelectedItem; }
		}

		public Vector3I Position
		{
			get
			{
				try
				{
					int posX = int.Parse(TXT_Position_X.Text);
					int posY = int.Parse(TXT_Position_Y.Text);
					int posZ = int.Parse(TXT_Position_Z.Text);

					return new Vector3I(posX, posY, posZ);
				}
				catch (Exception ex)
				{
					ApplicationLog.BaseLog.Error(ex);
					return Vector3I.Zero;
				}
			}
		}

		#endregion

		#region "Methods"

		private void BTN_CubeBlock_Add_Click(object sender, EventArgs e)
		{
			if (CMB_BlockType.SelectedIndex < 0)
				return;

			try
			{
				MyObjectBuilder_CubeBlock objectBuilder = (MyObjectBuilder_CubeBlock) Activator.CreateInstance(SelectedType.Key);
				objectBuilder.SubtypeName = SelectedSubType;
				objectBuilder.Min = Position;

				CubeBlockEntity cubeBlock = (CubeBlockEntity) Activator.CreateInstance(SelectedType.Value, new object[] { Parent, objectBuilder });
				ParentCubeGrid.AddCubeBlock(cubeBlock);

				Close();
			}
			catch (Exception ex)
			{
				ApplicationLog.BaseLog.Error(ex);
			}
		}

		private void CMB_BlockType_SelectedIndexChanged(object sender, EventArgs e)
		{
			CMB_BlockSubType.BeginUpdate();
			CMB_BlockSubType.Items.Clear();
			DictionaryValuesReader<MyDefinitionId, MyDefinitionBase> allDefinitions = MyDefinitionManager.Static.GetAllDefinitions( );
			foreach ( MyDefinitionBase o in allDefinitions )
			{
				MyCubeBlockDefinition cubeBlockDefinition = o as MyCubeBlockDefinition;
				if ( cubeBlockDefinition == null )
				{
					continue;
				}
				if ( cubeBlockDefinition.Id.TypeId == SelectedType.Key )
				{
					CMB_BlockSubType.Items.Add( cubeBlockDefinition.Id.SubtypeName );
				}
			}
			CMB_BlockSubType.EndUpdate();
		}

		#endregion
	}
}
