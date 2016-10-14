using System.ComponentModel;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI.Interfaces;
using SEModAPIInternal.API.Common;

namespace SEServerExtender.EntityWrappers.BlockWrappers
{
    public class BeaconWrapper : FunctionalBlockWrapper
    {
        [Browsable(false)]
        public MyBeacon Block;

        public BeaconWrapper(MySlimBlock block) : base(block)
        {
            Block = (MyBeacon)block.FatBlock;
        }

        [Category("Beacon")]
        public float BroadcastRange
        {
            get { return Block.GetValueFloat("Radius"); }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.SetValueFloat("Radius", value)); }
        }
    }
}