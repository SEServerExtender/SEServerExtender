using System.ComponentModel;
using Sandbox.Game.Entities.Cube;
using SEModAPIInternal.API.Common;

namespace SEServerExtender.EntityWrappers.BlockWrappers
{
    public class ConnectorWrapper : FunctionalBlockWrapper
    {
        [Browsable(false)]
        public MyShipConnector Block;

        public ConnectorWrapper(MySlimBlock block) : base(block)
        {
            Block = (MyShipConnector)block.FatBlock;
        }

        [Category("Connector")]
        public bool ThrowOut
        {
            get { return Block.ThrowOut; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.ThrowOut.Value = value); }
        }

        [Category("Connector")]
        public bool CollectAll
        {
            get { return Block.CollectAll; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.CollectAll.Value = value); }
        }

        [Category("Connector")]
        public bool Connected
        {
            get { return Block.Connected; }
            set
            {
                SandboxGameAssemblyWrapper.Instance.GameAction(() =>
                                                               {
                                                                   if (!value)
                                                                       Block.TryDisconnect();
                                                                   else
                                                                       Block.TryConnect();
                                                               });
            }
        }
    }
}