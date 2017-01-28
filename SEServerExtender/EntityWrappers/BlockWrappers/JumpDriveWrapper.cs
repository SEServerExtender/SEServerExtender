using System.ComponentModel;
using System.Reflection;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using SEModAPIInternal.API.Common;
using VRage.Sync;

namespace SEServerExtender.EntityWrappers.BlockWrappers
{
    public class JumpDriveWrapper : FunctionalBlockWrapper
    {
        [Browsable(false)]
        public MyJumpDrive Block;

        public JumpDriveWrapper(MySlimBlock block) : base(block)
        {
            Block = (MyJumpDrive)block.FatBlock;
        }

        [Category("Jump Drive")]
        [Description("Percentage")]
        public float StoredPower
        {
            get { return 100 * ((Sync<float, SyncDirection.BothWays>)typeof(MyJumpDrive).GetField("m_storedPower", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Block)).Value / Block.BlockDefinition.PowerNeededForJump; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.SetStoredPower(value / 100)); }
        }
    }
}