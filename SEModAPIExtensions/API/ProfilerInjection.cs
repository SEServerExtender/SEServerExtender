using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Sandbox;
using Sandbox.Engine.Multiplayer;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.World;
using SEModAPIInternal.Support;
using VRage;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Profiler;
using VRage.Utils;

namespace SEModAPIExtensions.API
{
    /// <summary>
    /// Keen removed per-block profiling after performance concerns.
    /// Let's put it back because it's too useful to not have.
    /// There's a tickbox in the GUI that enables this
    /// </summary>
    public class ProfilerInjection 
    {
        private static readonly Logger BaseLog = LogManager.GetLogger("BaseLog");
        //Entities updated each frame
        static MyDistributedUpdater<CachingList<MyEntity>, MyEntity> m_entitiesForUpdate;

        //Entities updated each 10th frame
        static MyDistributedUpdater<CachingList<MyEntity>, MyEntity> m_entitiesForUpdate10;

        //Entities updated each 100th frame
        static MyDistributedUpdater<CachingList<MyEntity>, MyEntity> m_entitiesForUpdate100;

        static MyEntityCreationThread m_creationThread;

        public static bool ProfilePerBlock = false;
        public static bool ProfilePerGrid = false;

        public static void Init()
        {
            //ApplicationLog.BaseLog.Warn("Skipping profiler init");
            //return;
            var entType = typeof(MyEntities);
            m_entitiesForUpdate = (MyDistributedUpdater<CachingList<MyEntity>, MyEntity>)entType.GetField("m_entitiesForUpdate", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            m_entitiesForUpdate10 = (MyDistributedUpdater<CachingList<MyEntity>, MyEntity>)entType.GetField("m_entitiesForUpdate10", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            m_entitiesForUpdate100 = (MyDistributedUpdater<CachingList<MyEntity>, MyEntity>)entType.GetField("m_entitiesForUpdate100", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            m_creationThread = (MyEntityCreationThread)entType.GetField("m_creationThread", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            var keenBefore = entType.GetMethod("UpdateBeforeSimulation");
            var ourBefore = typeof(ProfilerInjection).GetMethod("UpdateBeforeSimulation");
            var keenAfter = entType.GetMethod("UpdateAfterSimulation");
            var ourAfter = typeof(ProfilerInjection).GetMethod("UpdateAfterSimulation");

            //using (var md5 = MD5.Create())
            //{
            //    var body = keenBefore.GetMethodBody().GetILAsByteArray();
            //    var hash = md5.ComputeHash(body);
            //    string compStr = Convert.ToBase64String(hash).ToUpper();
            //    if (compStr!= "YXXKWK+W+OHP+LNOGGM32A==" || body.Length != 238)
            //    {
            //        BaseLog.Warn("UpdateBeforeSimulation signature has changed! Cannot load profiler injector!");
            //        BaseLog.Error($"{compStr}:{body.Length}");
            //        return;
            //    }

            //    body = keenAfter.GetMethodBody().GetILAsByteArray();
            //    hash = md5.ComputeHash(body);
            //    compStr = Convert.ToBase64String(hash).ToUpper();
            //    if (compStr!= "7ZKFYWZRON9U3G43AV4QRA==" || body.Length != 220)
            //    {
            //        BaseLog.Warn("UpdateAfterSimulation signature has changed! Cannot load profiler injector!");
            //        BaseLog.Error($"{compStr}:{body.Length}");
            //        return;
            //    }
            //}
            BaseLog.Info("Replacing UpdateBeforeSimulation");
            MethodUtil.ReplaceMethod(ourBefore, keenBefore);
            BaseLog.Info("Replacing UpdateAfterSimulation");
            MethodUtil.ReplaceMethod(ourAfter, keenAfter);

        }

        static int m_update10Index = 0;
        static int m_update100Index = 0;
        static float m_update10Count = 0;
        static float m_update100Count = 0;

        public static void UpdateBeforeSimulation()
        {
            if (MySandboxGame.IsGameReady == false)
            {
                return;
            }

            ProfilerShort.Begin("MyEntities.UpdateBeforeSimulation");
            System.Diagnostics.Debug.Assert(MyEntities.UpdateInProgress == false);
            MyEntities.UpdateInProgress = true;

            {
                ProfilerShort.Begin("Before first frame");
                MyEntities.UpdateOnceBeforeFrame();

                ProfilerShort.BeginNextBlock("Each update");
                m_entitiesForUpdate.List.ApplyChanges();
                m_entitiesForUpdate.Update();
                MySimpleProfiler.Begin("Blocks");
                m_entitiesForUpdate.Iterate((x) =>
                {
                    ProfilerShort.Begin(x.GetType().Name);
                    if (x.MarkedForClose == false)
                    {
                        if (ProfilePerBlock)
                        {
                            MyCubeBlock block = x as MyCubeBlock;
                            if (block != null)
                            {
                                MySimpleProfiler.Begin(block.DefinitionDisplayNameText + (ProfilePerGrid ? $" - {block.CubeGrid.DisplayName}" : string.Empty));
                                if (ProfilePerGrid)
                                    MySimpleProfiler.Begin("&&GRID&&" + block.CubeGrid.DisplayName);
                            }
                        }
                        if (ProfilePerGrid)
                        {
                            MyCubeGrid grid = x as MyCubeGrid;
                            if (grid != null)
                                MySimpleProfiler.Begin("&&GRID&&" + grid.DisplayName);
                        }

                        x.UpdateBeforeSimulation();

                        if (ProfilePerBlock)
                        {
                            MyCubeBlock block = x as MyCubeBlock;
                            if (block != null)
                            {
                                MySimpleProfiler.End(block.DefinitionDisplayNameText + (ProfilePerGrid ? $" - {block.CubeGrid.DisplayName}" : string.Empty));
                                if (ProfilePerGrid)
                                    MySimpleProfiler.End("&&GRID&&" + block.CubeGrid.DisplayName);
                            }
                        }
                        if (ProfilePerGrid)
                        {
                            MyCubeGrid grid = x as MyCubeGrid;
                            if (grid != null)
                                MySimpleProfiler.End("&&GRID&&" + grid.DisplayName);
                        }
                    }
                    ProfilerShort.End();
                });

                ProfilerShort.BeginNextBlock("10th update");
                m_entitiesForUpdate10.List.ApplyChanges();
                m_entitiesForUpdate10.Update();
                m_entitiesForUpdate10.Iterate((x) => 
                {
                    ProfilerShort.Begin(x.GetType().Name);
                    if (x.MarkedForClose == false)
                    {
                        if (ProfilePerBlock)
                        {
                            MyCubeBlock block = x as MyCubeBlock;
                            if (block != null)
                            {
                                MySimpleProfiler.Begin(block.DefinitionDisplayNameText + (ProfilePerGrid ? $" - {block.CubeGrid.DisplayName}" : string.Empty));
                                if (ProfilePerGrid)
                                    MySimpleProfiler.Begin("&&GRID&&" + block.CubeGrid.DisplayName);
                            }
                        }
                        if (ProfilePerGrid)
                        {
                            MyCubeGrid grid = x as MyCubeGrid;
                            if (grid != null)
                                MySimpleProfiler.Begin("&&GRID&&" + grid.DisplayName);
                        }

                        x.UpdateBeforeSimulation10();

                        if (ProfilePerBlock)
                        {
                            MyCubeBlock block = x as MyCubeBlock;
                            if (block != null)
                            {
                                MySimpleProfiler.End(block.DefinitionDisplayNameText + (ProfilePerGrid ? $" - {block.CubeGrid.DisplayName}" : string.Empty));
                                if (ProfilePerGrid)
                                    MySimpleProfiler.End("&&GRID&&" + block.CubeGrid.DisplayName);
                            }
                        }
                        if (ProfilePerGrid)
                        {
                            MyCubeGrid grid = x as MyCubeGrid;
                            if (grid != null)
                                MySimpleProfiler.End("&&GRID&&" + grid.DisplayName);
                        }
                    }
                    ProfilerShort.End();
                });

                
                ProfilerShort.BeginNextBlock("100th update");
                m_entitiesForUpdate100.List.ApplyChanges();
                m_entitiesForUpdate100.Update();
                m_entitiesForUpdate100.Iterate((x) =>
                {
                    ProfilerShort.Begin(x.GetType().Name);
                    if (x.MarkedForClose == false)
                    {
                        if (ProfilePerBlock)
                        {
                            MyCubeBlock block = x as MyCubeBlock;
                            if (block != null)
                            {
                                MySimpleProfiler.Begin(block.DefinitionDisplayNameText + (ProfilePerGrid ? $" - {block.CubeGrid.DisplayName}" : string.Empty));
                                if (ProfilePerGrid)
                                    MySimpleProfiler.Begin("&&GRID&&" + block.CubeGrid.DisplayName);
                            }
                        }
                        if (ProfilePerGrid)
                        {
                            MyCubeGrid grid = x as MyCubeGrid;
                            if (grid != null)
                                MySimpleProfiler.Begin("&&GRID&&" + grid.DisplayName);
                        }

                        x.UpdateBeforeSimulation100();

                        if (ProfilePerBlock)
                        {
                            MyCubeBlock block = x as MyCubeBlock;
                            if (block != null)
                            {
                                MySimpleProfiler.End(block.DefinitionDisplayNameText + (ProfilePerGrid ? $" - {block.CubeGrid.DisplayName}" : string.Empty));
                                if (ProfilePerGrid)
                                    MySimpleProfiler.End("&&GRID&&" + block.CubeGrid.DisplayName);
                            }
                        }
                        if (ProfilePerGrid)
                        {
                            MyCubeGrid grid = x as MyCubeGrid;
                            if (grid != null)
                                MySimpleProfiler.Begin("&&GRID&&" + grid.DisplayName);
                        }
                    }
                    ProfilerShort.End();
                });

                ProfilerShort.End();
            }
            MySimpleProfiler.End("Blocks");

            MyEntities.UpdateInProgress = false;

            ProfilerShort.End();
        }
        
        //  Update all physics objects - AFTER physics simulation
        public static void UpdateAfterSimulation()
        {
            if (MySandboxGame.IsGameReady == false)
            {
                return;
            }
            VRageRender.MyRenderProxy.GetRenderProfiler().StartProfilingBlock("UpdateAfterSimulation");
            {
                System.Diagnostics.Debug.Assert(MyEntities.UpdateInProgress == false);
                MyEntities.UpdateInProgress = true;

                ProfilerShort.Begin("UpdateAfter1");
                m_entitiesForUpdate.List.ApplyChanges();
                MySimpleProfiler.Begin("Blocks");
                m_entitiesForUpdate.Iterate((x) =>
                {
                    ProfilerShort.Begin(x.GetType().Name);
                    if (x.MarkedForClose == false)
                    {
                        if (ProfilePerBlock)
                        {
                            MyCubeBlock block = x as MyCubeBlock;
                            if (block != null)
                            {
                                MySimpleProfiler.Begin(block.DefinitionDisplayNameText + (ProfilePerGrid ? $" - {block.CubeGrid.DisplayName}" : string.Empty));
                                if (ProfilePerGrid)
                                    MySimpleProfiler.Begin("&&GRID&&" + block.CubeGrid.DisplayName);
                            }
                        }
                        if (ProfilePerGrid)
                        {
                            MyCubeGrid grid = x as MyCubeGrid;
                            if (grid != null)
                                MySimpleProfiler.Begin("&&GRID&&" + grid.DisplayName);
                        }

                        x.UpdateAfterSimulation();

                        if (ProfilePerBlock)
                        {
                            MyCubeBlock block = x as MyCubeBlock;
                            if (block != null)
                            {
                                MySimpleProfiler.End(block.DefinitionDisplayNameText + (ProfilePerGrid ? $" - {block.CubeGrid.DisplayName}" : string.Empty));
                                if (ProfilePerGrid)
                                    MySimpleProfiler.End("&&GRID&&" + block.CubeGrid.DisplayName);
                            }
                        }
                        if (ProfilePerGrid)
                        {
                            MyCubeGrid grid = x as MyCubeGrid;
                            if (grid != null)
                                MySimpleProfiler.End("&&GRID&&" + grid.DisplayName);
                        }
                    }
                    ProfilerShort.End();
                });
                ProfilerShort.End();

                ProfilerShort.Begin("UpdateAfter10");
                m_entitiesForUpdate10.List.ApplyChanges();
                m_entitiesForUpdate10.Iterate((x) =>
                    {
                        ProfilerShort.Begin(x.GetType().Name);
                        if (x.MarkedForClose == false)
                        {
                            if (ProfilePerBlock)
                            {
                                MyCubeBlock block = x as MyCubeBlock;
                                if (block != null)
                                {
                                    MySimpleProfiler.Begin(block.DefinitionDisplayNameText + (ProfilePerGrid ? $" - {block.CubeGrid.DisplayName}" : string.Empty));
                                    if (ProfilePerGrid)
                                        MySimpleProfiler.Begin("&&GRID&&" + block.CubeGrid.DisplayName);
                                }
                            }
                            if (ProfilePerGrid)
                            {
                                MyCubeGrid grid = x as MyCubeGrid;
                                if (grid != null)
                                    MySimpleProfiler.Begin("&&GRID&&" + grid.DisplayName);
                            }

                            x.UpdateAfterSimulation10();

                            if (ProfilePerBlock)
                            {
                                MyCubeBlock block = x as MyCubeBlock;
                                if (block != null)
                                {
                                    MySimpleProfiler.End(block.DefinitionDisplayNameText + (ProfilePerGrid ? $" - {block.CubeGrid.DisplayName}" : string.Empty));
                                    if (ProfilePerGrid)
                                        MySimpleProfiler.End("&&GRID&&" + block.CubeGrid.DisplayName);
                                }
                            }
                            if (ProfilePerGrid)
                            {
                                MyCubeGrid grid = x as MyCubeGrid;
                                if (grid != null)
                                    MySimpleProfiler.End("&&GRID&&" + grid.DisplayName);
                            }
                        }
                        ProfilerShort.End();
                    });
                ProfilerShort.End();

                ProfilerShort.Begin("UpdateAfter100");
                m_entitiesForUpdate100.List.ApplyChanges();
                m_entitiesForUpdate100.Iterate((x) =>
                {
                    ProfilerShort.Begin(x.GetType().Name);
                    if (x.MarkedForClose == false)
                    {
                        if (ProfilePerBlock)
                        {
                            MyCubeBlock block = x as MyCubeBlock;
                            if (block != null)
                            {
                                MySimpleProfiler.Begin(block.DefinitionDisplayNameText + (ProfilePerGrid ? $" - {block.CubeGrid.DisplayName}" : string.Empty));
                                if (ProfilePerGrid)
                                    MySimpleProfiler.Begin("&&GRID&&" + block.CubeGrid.DisplayName);
                            }
                        }
                        if (ProfilePerGrid)
                        {
                            MyCubeGrid grid = x as MyCubeGrid;
                            if (grid != null)
                                MySimpleProfiler.Begin("&&GRID&&" + grid.DisplayName);
                        }

                        x.UpdateAfterSimulation100();

                        if (ProfilePerBlock)
                        {
                            MyCubeBlock block = x as MyCubeBlock;
                            if (block != null)
                            {
                                MySimpleProfiler.End(block.DefinitionDisplayNameText + (ProfilePerGrid ? $" - {block.CubeGrid.DisplayName}" : string.Empty));
                                if (ProfilePerGrid)
                                    MySimpleProfiler.End("&&GRID&&" + block.CubeGrid.DisplayName);
                            }
                        }
                        if (ProfilePerGrid)
                        {
                            MyCubeGrid grid = x as MyCubeGrid;
                            if (grid != null)
                                MySimpleProfiler.End("&&GRID&&" + grid.DisplayName);
                        }
                    }
                    ProfilerShort.End();
                });
                ProfilerShort.End();
                MySimpleProfiler.End("Blocks");

                MyEntities.UpdateInProgress = false;

                MyEntities.DeleteRememberedEntities();
            }
            
                if (m_creationThread!= null && MyMultiplayer.Static != null)
                    while (m_creationThread.ConsumeResult(MyMultiplayer.Static.ReplicationLayer.GetSimulationUpdateTime())); // Add entities created asynchronously            
            VRageRender.MyRenderProxy.GetRenderProfiler().EndProfilingBlock();
        }
    }
}
