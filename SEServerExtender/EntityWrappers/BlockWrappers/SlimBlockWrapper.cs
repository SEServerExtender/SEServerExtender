using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Weapons;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;
using SpaceEngineers.Game.Entities.Blocks;
using VRageMath;

namespace SEServerExtender.EntityWrappers.BlockWrappers
{
    public class SlimBlockWrapper
    {
        [Browsable(false)]
        public MySlimBlock SlimBlock;

        public SlimBlockWrapper(MySlimBlock block)
        {
            SlimBlock = block;
        }

        [Category("SlimBlock")]
        public float Mass
        {
            get { return SlimBlock.GetMass(); }
        }

        [Category("SlimBlock")]
        public Vector3I GridPosition
        {
            get { return SlimBlock.Position; }
        }

        [Category("SlimBlock")]
        public Vector3D WorldPosition
        {
            get { return SlimBlock.CubeGrid.GridIntegerToWorld(SlimBlock.Position); }
        }

        [Category("SlimBlock")]
        public string BlockType
        {
            get
            {
                //trims "MyObjectBuilder_" off the beginning
                string blockType = SlimBlock.BlockDefinition.Id.TypeId.ToString();
                if (blockType.StartsWith("MyObjectBuilder_"))
                    return blockType.Substring(16);

                return blockType;
            }
        }

        [Category("SlimBlock")]
        public string BlockSubtype
        {
            get { return SlimBlock.BlockDefinition.Id.SubtypeName; }
        }

        [Category("SlimBlock")]
        [Description("This field affects player block limits. Input 0 to remove ownership.")]
        public long BuiltBy
        {
            get { return SlimBlock.BuiltBy; }
            set
            {
                if (value == 0)
                {
                    DialogResult removeResult = MessageBox.Show(
                        $"Are you sure you want to remove ownership of this block?",
                        "Confirm",
                        MessageBoxButtons.YesNo);
                    if (removeResult == DialogResult.No)
                        return;

                    SandboxGameAssemblyWrapper.Instance.GameAction(() => SlimBlock.RemoveAuthorship());
                    return;
                }

                if (!PlayerMap.Instance.GetPlayerIds().Contains(value))
                    throw new Exception("That is not a valid PlayerID.");

                DialogResult changeResult = MessageBox.Show(
                    $"Are you sure you want to change the owner of this block to {PlayerMap.Instance.GetPlayerNameFromPlayerId(value)}?",
                    "Confirm",
                    MessageBoxButtons.YesNo);
                if (changeResult == DialogResult.No)
                    return;

                if (BuiltBy != 0)
                    SandboxGameAssemblyWrapper.Instance.GameAction(() => SlimBlock.TransferAuthorship(value));
                else
                    SandboxGameAssemblyWrapper.Instance.GameAction(() =>
                                                                   {
                                                                       try
                                                                       {
                                                                           typeof(MySlimBlock).GetField("m_builtByID", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(SlimBlock, value);
                                                                           SlimBlock.AddAuthorship();
                                                                       }
                                                                       catch (Exception ex)
                                                                       {
                                                                           ApplicationLog.Error("Error assigning block author: " + ex);
                                                                       }
                                                                   });
            }
        }

        [Category("SlimBlock")]
        public string BuiltByName
        {
            get { return PlayerMap.Instance.GetPlayerNameFromPlayerId(BuiltBy); }
        }

        public static SlimBlockWrapper GetWrapper(MySlimBlock slimBlock)
        {
            if (slimBlock.FatBlock == null)
                return new SlimBlockWrapper(slimBlock);

            MyCubeBlock block = slimBlock.FatBlock;

            if (block is MyThrust)
                return new ThrustWrapper(slimBlock);
            if (block is MyGyro)
                return new GyroWrapper(slimBlock);
            if (block is MyLargeTurretBase)
                return new TurretWrapper(slimBlock);
            if (block is MyBatteryBlock)
                return new BatteryWrapper(slimBlock);
            if (block is MyShipToolBase)
                return new ShipToolWrapper(slimBlock);
            if (block is MyLandingGear)
                return new LandingGearWrapper(slimBlock);
            if (block is MyShipConnector)
                return new ConnectorWrapper(slimBlock);
            if (block is MyRadioAntenna)
                return new AntennaWrapper(slimBlock);
            if (block is MyBeacon)
                return new BeaconWrapper(slimBlock);
            if (block is MyJumpDrive)
                return new JumpDriveWrapper(slimBlock);


            if (block is MyFunctionalBlock)
                return new FunctionalBlockWrapper(slimBlock);

            return new CubeBlockWrapper(slimBlock);
        }
    }
}