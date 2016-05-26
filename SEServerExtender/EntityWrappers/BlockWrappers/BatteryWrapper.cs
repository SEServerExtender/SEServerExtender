using System.ComponentModel;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using SEModAPIInternal.API.Common;

namespace SEServerExtender.EntityWrappers.BlockWrappers
{
    public class BatteryWrapper : FunctionalBlockWrapper
    {
        [Browsable( false )]
        public MyBatteryBlock Block;

        public BatteryWrapper( MySlimBlock block ) : base( block )
        {
            Block = (MyBatteryBlock)block.FatBlock;
        }

        [Category( "General" )]
        [Description( "In MW" )]
        public float CurrentStoredPower
        {
            get { return Block.CurrentStoredPower; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction( () => Block.CurrentStoredPower = value ); }
        }

        [Category( "General" )]
        [Description( "In MW" )]
        public float MaxStoredPower
        {
            get { return Block.MaxStoredPower; }
        }

        [Category( "General" )]
        public bool OnlyRecharge
        {
            get { return Block.OnlyRecharge; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction( () => Block.OnlyRecharge = value ); }
        }

        [Category( "General" )]
        public bool OnlyDischarge
        {
            get { return Block.OnlyDischarge; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction( () => Block.OnlyDischarge = value ); }
        }

        [Category( "General" )]
        public bool SemiautoEnabled
        {
            get { return Block.SemiautoEnabled; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction( () => Block.SemiautoEnabled = value ); }
        }
    }
}