using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Weapons;
using Sandbox.ModAPI.Ingame;
using SEModAPIInternal.API.Common;

namespace SEServerExtender.EntityWrappers.BlockWrappers
{
    public class TurretWrapper : FunctionalBlockWrapper
    {
        [Browsable(false)]
        MyLargeTurretBase Block;

        public TurretWrapper( MySlimBlock block ) : base( block )
        {
            Block = (MyLargeTurretBase)block.FatBlock;
        }

        [Category("General")]
        public float ShootingRange
        {
            get { return Block.ShootingRange; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.ShootingRange = value); }
        }

        [Category("General")]
        public bool IdleMovement
        {
            get { return ((IMyLargeTurretBase)Block).EnableIdleRotation; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => ((IMyLargeTurretBase)Block).EnableIdleRotation = value); }
        }

        [Category("General")]
        public bool TargetMeteors
        {
            get { return Block.TargetMeteors; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.TargetMeteors = value); }
        }

        [Category("General")]
        public bool TargetMoving
        {
            get { return Block.TargetMoving; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.TargetMoving = value); }
        }

        [Category("General")]
        public bool TargetSmallGrids
        {
            get { return Block.TargetSmallGrids; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.TargetSmallGrids = value); }
        }

        [Category("General")]
        public bool TargetLargeGrids
        {
            get { return Block.TargetLargeGrids; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.TargetLargeGrids = value); }
        }

        [Category("General")]
        public bool TargetCharacters
        {
            get { return Block.TargetCharacters; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.TargetCharacters = value); }
        }

        [Category("General")]
        public bool TargetStations
        {
            get { return Block.TargetStations; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.TargetStations = value); }
        }

        [Category("General")]
        public bool TargetNeutrals
        {
            get { return Block.TargetNeutrals; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.TargetNeutrals = value); }
        }
    }
}
