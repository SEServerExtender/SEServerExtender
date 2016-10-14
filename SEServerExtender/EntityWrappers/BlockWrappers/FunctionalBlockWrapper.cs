using System.ComponentModel;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI;
using SEModAPIInternal.API.Common;

namespace SEServerExtender.EntityWrappers.BlockWrappers
{
    public class FunctionalBlockWrapper : CubeBlockWrapper
    {
        [Browsable(false)]
        private readonly MyFunctionalBlock Block;

        public FunctionalBlockWrapper(MySlimBlock block) : base(block)
        {
            Block = (MyFunctionalBlock)block.FatBlock;
        }

        public string DetailedInfo
        {
            get { return Block.DetailedInfo.ToString(); }
        }

        public string CustomInfo
        {
            get { return Block.CustomInfo.ToString(); }
        }

        [Category("Terminal")]
        public bool Enabled
        {
            get { return Block.Enabled; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => (Block as IMyFunctionalBlock).RequestEnable(value)); }
        }

        [Category("Terminal")]
        public bool ShowOnHUD
        {
            get { return Block.ShowOnHUD; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.ShowOnHUD = value); }
        }
    }
}