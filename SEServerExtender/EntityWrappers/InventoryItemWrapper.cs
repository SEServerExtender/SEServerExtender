using System.ComponentModel;
using Sandbox.Definitions;
using Sandbox.Game;
using SEModAPIInternal.API.Common;
using VRage;
using VRage.Game.Entity;
using VRage.Game.ModAPI.Ingame;

namespace SEServerExtender.EntityWrappers
{
    internal class InventoryItemWrapper
    {
        private readonly MyInventory Inventory;

        [Browsable(false)]
        public MyPhysicalInventoryItem Item;

        public InventoryItemWrapper(MyPhysicalInventoryItem item, MyInventory inventory)
        {
            Item = item;
            Inventory = inventory;
        }

        [Category("General")]
        public float Amount
        {
            get { return (float)Item.Amount; }
            set
            {
                SandboxGameAssemblyWrapper.Instance.GameAction(() =>
                                                               {
                                                                   if (value > (float)Item.Amount)
                                                                   {
                                                                       float amountThatFits = (float)Inventory.ComputeAmountThatFits(Item.GetDefinitionId());
                                                                       if (value - (float)Item.Amount > amountThatFits)
                                                                           value = amountThatFits;

                                                                       Inventory.Add(Item, (MyFixedPoint)value - Item.Amount);
                                                                   }
                                                                   else
                                                                   {
                                                                       var amount = (MyFixedPoint)((float)Item.Amount - value);
                                                                       Inventory.Remove(Item, amount);
                                                                   }
                                                               });
            }
        }

        [Category("General")]
        public string Name
        {
            get { return MyDefinitionManager.Static.GetPhysicalItemDefinition(Item.Content).DisplayNameText; }
        }
    }
}