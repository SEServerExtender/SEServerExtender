using System;
using System.ComponentModel;
using System.Windows.Forms;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using SEModAPIInternal.API.Common;
using VRage.Game;

namespace SEServerExtender.EntityWrappers.BlockWrappers
{
    public class CubeBlockWrapper : SlimBlockWrapper
    {
        [Browsable(false)]
        public MyCubeBlock Entity;

        public CubeBlockWrapper(MySlimBlock block) : base(block)
        {
            Entity = block.FatBlock;
        }

        [Category("General")]
        public long EntityId
        {
            get { return Entity.EntityId; }
        }

        [Category("General")]
        public string DisplayName
        {
            get { return Entity.DisplayNameText; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Entity.DisplayName = value); }
        }

        [Category("General")]
        public long OwnerId
        {
            get { return Entity.OwnerId; }
            set
            {
                if (value != 0 && !PlayerMap.Instance.GetPlayerIds().Contains(value))
                    throw new Exception("That is not a valid PlayerID.");

                DialogResult messageResult = MessageBox.Show(
                    $"Are you sure you want to change the owner of this block to {PlayerMap.Instance.GetPlayerNameFromPlayerId(value)}?",
                    "Confirm",
                    MessageBoxButtons.YesNo);
                if (messageResult == DialogResult.No)
                    return;

                Entity.ChangeOwner(value, MyOwnershipShareModeEnum.Faction);
            }
        }

        [Category("General")]
        public string OwnerName
        {
            get { return PlayerMap.Instance.GetPlayerNameFromPlayerId(OwnerId); }
        }
    }
}