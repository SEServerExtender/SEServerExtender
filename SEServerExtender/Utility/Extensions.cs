using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Engine.Multiplayer;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Replication;
using Sandbox.Game.World;
using SEModAPIInternal.API.Common;
using VRage.Game.ModAPI;
using VRage.Game.ObjectBuilders.Definitions;

namespace SEServerExtender.Utility
{
    public static class Extensions
    {
        public static void Repair( this MyCubeGrid grid )
        {
            if ( grid?.Physics == null || grid.Closed )
                return;

            //do this in a new thread so we don't lock up the GUI
            Parallel.ForEach( grid.CubeBlocks, ( block ) =>
                                               {
                                                   if ( !block.IsFullIntegrity || block.HasDeformation )
                                                       block.Repair();
                                               } );
        }

         public static void Repair( this MySlimBlock block )
         {
             if ( block.CubeGrid?.Physics != null && (!block.IsFullIntegrity || block.HasDeformation ))
             {

                 SandboxGameAssemblyWrapper.Instance.BeginGameAction( () =>
                                                                 {
                                                                     block.IncreaseMountLevelToDesiredRatio( block.MaxIntegrity, 0, null, 100f, true );
                                                                     //block.IncreaseMountLevel( 100f, 0, null, 100f, true );
                                                                     //block.CubeGrid.ResetBlockSkeleton( block );
                                                                 }, null, null );
             }
         }

        public static string GetOwner(this IMyCubeGrid grid)
        {
            if (grid.BigOwners.Count > 0 && grid.BigOwners[0] > 0)
            {
                MyIdentity ownerIdentity = MySession.Static.Players.TryGetIdentity(grid.BigOwners[0]);

                if (ownerIdentity != null)
                    return ownerIdentity.DisplayName;
            }
            return "";
        }

        public static string GetOwner(this MyCubeGrid grid)
        {
            return ((IMyCubeGrid)grid).GetOwner();
        }
    }
}
