using System.ComponentModel;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.ModAPI.Interfaces;
using SEModAPIInternal.API.Common;

namespace SEServerExtender.EntityWrappers.BlockWrappers
{
    public class ThrustWrapper : FunctionalBlockWrapper
    {
        [Browsable(false)]
        public MyThrust Block;

        public ThrustWrapper(MySlimBlock block) : base(block)
        {
            Block = (MyThrust)block.FatBlock;
        }

        [Category("Thruster")]
        public float ThrustOverride
        {
            get { return Block.ThrustOverride; }
            set
            {
                SandboxGameAssemblyWrapper.Instance.GameAction(() =>
                                                               {
                                                                   float max = Block.GetMaximum<float>("Override");
                                                                   if (value > max)
                                                                       value = max;
                                                                   Block.SetValueFloat("Override", value);
                                                               });
            }
        }
    }
}