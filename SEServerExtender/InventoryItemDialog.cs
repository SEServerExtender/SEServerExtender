using Sandbox.Game;
using SEModAPIInternal.API.Common;
using VRage.Game;
using VRage.Game.Entity;

namespace SEServerExtender
{
	using System;
	using System.Collections.Generic;
	using System.Windows.Forms;
	using Sandbox.Definitions;
	using SEModAPIInternal.API.Entity;
	using SEModAPIInternal.Support;
	using VRage;
	using VRage.Collections;
	using VRage.ObjectBuilders;

	public partial class InventoryItemDialog : Form
	{
		#region "Attributes"

		private static List<MyDefinitionId> _idList;

		#endregion

		#region "Constructors and Initializers"

		public InventoryItemDialog()
		{
			//Populate the static list with the ids from the items
			if (_idList == null)
			{
				_idList = new List<MyDefinitionId>();

				DictionaryValuesReader<MyDefinitionId, MyDefinitionBase> allDefinitions = MyDefinitionManager.Static.GetAllDefinitions();
				foreach ( MyDefinitionBase definition in allDefinitions )
				{
					MyPhysicalItemDefinition def = definition as MyPhysicalItemDefinition;
					if ( def != null )
					{
						_idList.Add( def.Id );
					}
				}
				foreach ( MyDefinitionBase definition in allDefinitions )
				{
					MyComponentDefinition def = definition as MyComponentDefinition;
					if ( def != null )
					{
						_idList.Add( def.Id );
					}
				}
				foreach ( MyDefinitionBase definition in allDefinitions )
				{
					MyAmmoMagazineDefinition def = definition as MyAmmoMagazineDefinition;
					if ( def != null )
					{
						_idList.Add( def.Id );
					}
				}
			}

			InitializeComponent();

			CMB_ItemType.BeginUpdate();
			foreach (var entry in _idList)
			{
				CMB_ItemType.Items.Add(entry);
			}
			CMB_ItemType.EndUpdate();

			TXT_ItemAmount.Text = "0.0";
		}

		#endregion

		#region "Properties"

		public MyInventory InventoryContainer { get; set; }

		public MyDefinitionId SelectedType
		{
			get { return (MyDefinitionId)CMB_ItemType.SelectedItem; }
		}

		public float Amount
		{
			get
			{
				try
				{
					float amount = float.Parse(TXT_ItemAmount.Text);

					return amount;
				}
				catch (Exception ex)
				{
					ApplicationLog.BaseLog.Error(ex);
					return 0;
				}
			}
		}

		#endregion

		#region "Methods"

		private void BTN_InventoryItem_Add_Click(object sender, EventArgs e)
		{
			if (CMB_ItemType.SelectedItem == null)
				return;
			if (Amount <= 0.0f)
				return;

			try
			{
			    SandboxGameAssemblyWrapper.Instance.GameAction(() =>
			    {
			        var amount = (MyFixedPoint) Amount;
			        var content =
			            (MyObjectBuilder_PhysicalObject) MyObjectBuilderSerializer.CreateNewObject(SelectedType);
			        MyObjectBuilder_InventoryItem inventoryItem = new MyObjectBuilder_InventoryItem
			        {
			            Amount = amount,
			            Content = content
			        };

			        if (InventoryContainer.CanItemsBeAdded(amount, SelectedType))
			        {
			            InventoryContainer.AddItems(amount, inventoryItem.Content);
			        }
			    });
				Close();
			}
			catch (Exception ex)
			{
				ApplicationLog.BaseLog.Error(ex);
			}
		}

		#endregion
	}
}
