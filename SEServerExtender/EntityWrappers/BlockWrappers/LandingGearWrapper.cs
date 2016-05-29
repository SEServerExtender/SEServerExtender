using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Entities.Interfaces;
using SEModAPIInternal.API.Common;
using SpaceEngineers.Game.Entities.Blocks;

namespace SEServerExtender.EntityWrappers.BlockWrappers
{
    public class LandingGearWrapper : FunctionalBlockWrapper
    {
        [Browsable( false )]
        public MyLandingGear Block;
        public LandingGearWrapper( MySlimBlock block ) : base( block )
        {
            Block = (MyLandingGear)block.FatBlock;
        }

        [Category( "General" )]
        public bool AutoLock
        {
            get { return Block.AutoLock; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction( () => Block.AutoLock = value ); }
        }

        [Category( "General" )]
        public bool Locked
        {
            get { return Block.IsLocked; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction( () =>
                                                                  {
                                                                      if ( value )
                                                                          Block.RequestLandingGearLock();
                                                                      else
                                                                          Block.RequestLandingGearUnlock();
                                                                  } ); }
        }
    }
}
