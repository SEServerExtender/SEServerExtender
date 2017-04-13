using System.ComponentModel;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Weapons;
using Sandbox.ModAPI.Ingame;
using SEModAPIInternal.API.Common;

namespace SEServerExtender.EntityWrappers.BlockWrappers
{
    public class TurretWrapper : FunctionalBlockWrapper
    {
        [Browsable(false)]
        private readonly MyLargeTurretBase Block;

        public TurretWrapper(MySlimBlock block) : base(block)
        {
            Block = (MyLargeTurretBase)block.FatBlock;
        }

        [Category("Turret")]
        public float ShootingRange
        {
            get { return Block.ShootingRange; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.ShootingRange = value); }
        }

        [Category("Turret")]
        public bool IdleMovement
        {
            get { return ((IMyLargeTurretBase)Block).EnableIdleRotation; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => ((IMyLargeTurretBase)Block).EnableIdleRotation = value); }
        }

        [Category("Turret")]
        public bool TargetMeteors
        {
            get { return Block.TargetMeteors; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.TargetMeteors = value); }
        }
        
        [Category("Turret")]
        public bool TargetSmallGrids
        {
            get { return Block.TargetSmallGrids; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.TargetSmallGrids = value); }
        }

        [Category("Turret")]
        public bool TargetLargeGrids
        {
            get { return Block.TargetLargeGrids; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.TargetLargeGrids = value); }
        }

        [Category("Turret")]
        public bool TargetCharacters
        {
            get { return Block.TargetCharacters; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.TargetCharacters = value); }
        }

        [Category("Turret")]
        public bool TargetStations
        {
            get { return Block.TargetStations; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.TargetStations = value); }
        }

        [Category("Turret")]
        public bool TargetNeutrals
        {
            get { return Block.TargetNeutrals; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.TargetNeutrals = value); }
        }
    }
}