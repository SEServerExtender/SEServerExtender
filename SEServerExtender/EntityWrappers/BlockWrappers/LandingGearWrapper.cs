using System.ComponentModel;
using Sandbox.Game.Entities.Cube;
using SEModAPIInternal.API.Common;
using SpaceEngineers.Game.Entities.Blocks;

namespace SEServerExtender.EntityWrappers.BlockWrappers
{
    public class LandingGearWrapper : FunctionalBlockWrapper
    {
        [Browsable(false)]
        public MyLandingGear Block;

        public LandingGearWrapper(MySlimBlock block) : base(block)
        {
            Block = (MyLandingGear)block.FatBlock;
        }

        [Category("Landing Gear")]
        public bool AutoLock
        {
            get { return Block.AutoLock; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.AutoLock = value); }
        }

        [Category("Landing Gear")]
        public bool Locked
        {
            get { return Block.IsLocked; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.RequestLock(value)); }
        }
    }
}