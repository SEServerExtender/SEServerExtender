using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SEModAPIInternal.API.Common;

namespace SEServerExtender.EntityWrappers.BlockWrappers
{
    public class ConnectorWrapper : FunctionalBlockWrapper
    {
        [Browsable( false )]
        public MyShipConnector Block;

        public ConnectorWrapper( MySlimBlock block ) : base( block )
        {
            Block = (MyShipConnector)block.FatBlock;
        }

        [Category( "General" )]
        public bool ThrowOut
        {
            get { return Block.ThrowOut; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction( () => Block.ThrowOut.Value=value ); }
        }

        [Category( "General" )]
        public bool CollectAll
        {
            get { return Block.CollectAll; } 
            set { SandboxGameAssemblyWrapper.Instance.GameAction( () => Block.CollectAll.Value = value ); }
        }

        [Category( "General" )]
        public bool Connected
        {
            get { return Block.Connected; }
            set
            {
                SandboxGameAssemblyWrapper.Instance.GameAction( () =>
                                                                {
                                                                    if ( !value )
                                                                        Block.TryDisconnect();
                                                                    else
                                                                        Block.TryConnect();
                                                                } );
            }
        }
    }
}
