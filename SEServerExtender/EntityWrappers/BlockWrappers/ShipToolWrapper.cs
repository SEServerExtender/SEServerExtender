using System.ComponentModel;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Weapons;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using SEModAPIInternal.API.Common;

namespace SEServerExtender.EntityWrappers.BlockWrappers
{
    public class ShipToolWrapper : FunctionalBlockWrapper
    {
        [Browsable(false)]
        public MyShipToolBase Block;

        public ShipToolWrapper(MySlimBlock block) : base(block)
        {
            Block = (MyShipToolBase)block.FatBlock;
        }

        [Category("Terminal")]
        public bool UseConveyorSystem
        {
            get { return Block.UseConveyorSystem; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.UseConveyorSystem = value); }
        }

        [Category("Terminal")]
        public bool HelpOthers
        {
            get { return (Block as IMyShipWelder)?.HelpOthers ?? false; }
            set
            {
                if (Block is IMyShipWelder)
                    SandboxGameAssemblyWrapper.Instance.GameAction(() => (Block as IMyShipWelder).SetValue("helpOthers", value));
            }
        }
    }
}