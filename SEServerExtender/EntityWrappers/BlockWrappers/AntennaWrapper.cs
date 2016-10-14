using System.ComponentModel;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI.Interfaces;
using SEModAPIInternal.API.Common;

namespace SEServerExtender.EntityWrappers.BlockWrappers
{
    public class AntennaWrapper : FunctionalBlockWrapper
    {
        [Browsable(false)]
        public MyRadioAntenna Block;

        public AntennaWrapper(MySlimBlock block) : base(block)
        {
            Block = (MyRadioAntenna)block.FatBlock;
        }

        [Category("Antenna")]
        public float BroadcastRange
        {
            get { return Block.GetValueFloat("Radius"); }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.SetValueFloat("Radius", value)); }
        }

        [Category("Antenna")]
        public bool EnableBroadcast
        {
            get { return Block.GetValueBool("EnableBroadcast"); }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.SetValueBool("EnableBroadcast", value)); }
        }

        [Category("Antenna")]
        public bool ShowShipName
        {
            get { return Block.GetValueBool("ShowShipName"); }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Block.SetValueBool("ShowShipName", value)); }
        }
    }
}