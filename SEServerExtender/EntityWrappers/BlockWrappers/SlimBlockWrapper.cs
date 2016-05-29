using System.ComponentModel;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Weapons;
using SpaceEngineers.Game.Entities.Blocks;
using VRageMath;

namespace SEServerExtender.EntityWrappers.BlockWrappers
{
    public class SlimBlockWrapper
    {
        [Browsable( false )]
        public MySlimBlock SlimBlock;

        public SlimBlockWrapper( MySlimBlock block )
        {
            SlimBlock = block;
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
            if(block is MyLandingGear)
                return new LandingGearWrapper( slimBlock );
            if(block is MyShipConnector)
                return new ConnectorWrapper( slimBlock );

            if (block is MyFunctionalBlock)
                return new FunctionalBlockWrapper(slimBlock);

            return new CubeBlockWrapper(slimBlock);
        }

        [Category( "SlimBlock" )]
        public float Mass
        {
            get { return SlimBlock.GetMass(); }
        }

        [Category( "SlimBlock" )]
        public Vector3I GridPosition
        {
            get { return SlimBlock.Position; }
        }

        [Category( "SlimBlock" )]
        public Vector3D WorldPosition
        {
            get { return SlimBlock.CubeGrid.GridIntegerToWorld( SlimBlock.Position ); }
        }

        [Category( "SlimBlock" )]
        public string BlockType
        {
            get
            {
                //trims "MyObjectBuilder_" off the beginning
                string blockType = SlimBlock.BlockDefinition.Id.TypeId.ToString();
                if ( blockType.StartsWith( "MyObjectBuilder_" ) )
                    return blockType.Substring( 16 );

                return blockType;
            }
        }

        [Category( "SlimBlock" )]
        public string BlockSubtype
        {
            get { return SlimBlock.BlockDefinition.Id.SubtypeName; }
        }
    }
}