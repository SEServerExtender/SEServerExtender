using System.ComponentModel;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using SEModAPIInternal.API.Common;

namespace SEServerExtender.EntityWrappers.BlockWrappers
{
    public class GyroWrapper : FunctionalBlockWrapper
    {
        private readonly IMyGyro IBlock;

        [Browsable(false)]
        public MyGyro Block;

        public GyroWrapper(MySlimBlock block) : base(block)
        {
            Block = (MyGyro)block.FatBlock;
            IBlock = Block;
        }

        [Category("Gyro")]
        public float GyroPower
        {
            get { return Block.GyroPower; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.GyroPower = value); }
        }

        [Category("Gyro")]
        public bool GyroOverride
        {
            get { return Block.GyroOverride; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => IBlock.SetValueBool("Override", value)); }
        }

        [Category("Gyro")]
        public float Yaw
        {
            get { return IBlock.Yaw; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => IBlock.SetValue("Yaw", value)); }
        }

        [Category("Gyro")]
        public float Pitch
        {
            get { return IBlock.Pitch; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => IBlock.SetValue("Pitch", value)); }
        }

        [Category("Gyro")]
        public float Roll
        {
            get { return IBlock.Roll; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => IBlock.SetValue("Roll", value)); }
        }
    }
}