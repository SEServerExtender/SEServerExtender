using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Sandbox;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.World;
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
        static CachingList<MyEntity> m_entitiesForUpdate;

        //Entities updated each 10th frame
        static CachingList<MyEntity> m_entitiesForUpdate10;

        //Entities updated each 100th frame
        static CachingList<MyEntity> m_entitiesForUpdate100;
        static MyEntityCreationThread m_creationThread;
        public static bool ProfilePerBlock = false;

        public static void Init()
        {
            BaseLog.Warn("ProfilerInjector disabled on this build!");
            return;
            var entType = typeof(MyEntities);
            m_entitiesForUpdate = (CachingList<MyEntity>)entType.GetField("m_entitiesForUpdate", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            m_entitiesForUpdate10 = (CachingList<MyEntity>)entType.GetField("m_entitiesForUpdate10", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            m_entitiesForUpdate100 = (CachingList<MyEntity>)entType.GetField("m_entitiesForUpdate100", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            m_creationThread = (MyEntityCreationThread)entType.GetField("m_creationThread", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            var keenBefore = entType.GetMethod("UpdateBeforeSimulation");
            var ourBefore = typeof(ProfilerInjection).GetMethod("UpdateBeforeSimulation");
            var keenAfter = entType.GetMethod("UpdateAfterSimulation");
            var ourAfter = typeof(ProfilerInjection).GetMethod("UpdateAfterSimulation");


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
            MyCubeBlock cubeBlock;
            MyCharacter character;

            {
                ProfilerShort.Begin("Before first frame");
                MyEntities.UpdateOnceBeforeFrame();

                ProfilerShort.BeginNextBlock("Each update");
                m_entitiesForUpdate.ApplyChanges();
                foreach (MyEntity entity in m_entitiesForUpdate)
                {
                    string typeName = entity.GetType().Name;
                    cubeBlock = entity as MyCubeBlock;
                    character = entity as MyCharacter;
                    if (cubeBlock != null && ProfilePerBlock)
                    {
                        MySimpleProfiler.Begin("Blocks");
                        MySimpleProfiler.Begin(cubeBlock.DefinitionDisplayNameText);
                    }
                    else if (character != null)
                        MySimpleProfiler.Begin("CharactersB");
                    //ProfilerShort.Begin(Partition.Select(entity.GetType().GetHashCode(), "Part1", "Part2", "Part3"));
                    ProfilerShort.Begin(entity.GetType().Name);
                    if (entity.MarkedForClose == false)
                    {
                        entity.UpdateBeforeSimulation();
                    }
                    ProfilerShort.End();
                    //ProfilerShort.End();
                    if (cubeBlock != null && ProfilePerBlock)
                    {
                        MySimpleProfiler.End("Blocks");
                        MySimpleProfiler.End(cubeBlock.DefinitionDisplayNameText);
                    }
                    else if (character != null)
                        MySimpleProfiler.End("CharactersB");
                }

                ProfilerShort.BeginNextBlock("10th update");
                m_entitiesForUpdate10.ApplyChanges();
                if (m_entitiesForUpdate10.Count > 0)
                {
                    ++m_update10Index;
                    m_update10Index %= 10;
                    for (int i = m_update10Index; i < m_entitiesForUpdate10.Count; i += 10)
                    {
                        var entity = m_entitiesForUpdate10[i];

                        string typeName = entity.GetType().Name;
                        cubeBlock = entity as MyCubeBlock;
                        character = entity as MyCharacter;
                        if (cubeBlock != null && ProfilePerBlock)
                        {
                            MySimpleProfiler.Begin("Blocks");
                            MySimpleProfiler.Begin(cubeBlock.DefinitionDisplayNameText);
                        }
                        else if (character != null)
                            MySimpleProfiler.Begin("CharactersB10");
                        //ProfilerShort.Begin(Partition.Select(typeName.GetHashCode(), "Part1", "Part2", "Part3"));
                        ProfilerShort.Begin(typeName);
                        if (entity.MarkedForClose == false)
                        {
                            entity.UpdateBeforeSimulation10();
                        }
                        ProfilerShort.End();
                        //ProfilerShort.End();
                        if (cubeBlock != null && ProfilePerBlock)
                        {
                            MySimpleProfiler.End("Blocks");
                            MySimpleProfiler.End(cubeBlock.DefinitionDisplayNameText);
                        }
                        else if (character != null)
                            MySimpleProfiler.End("CharactersB10");
                    }
                }

                ProfilerShort.BeginNextBlock("100th update");
                m_entitiesForUpdate100.ApplyChanges();
                if (m_entitiesForUpdate100.Count > 0)
                {
                    ++m_update100Index;
                    m_update100Index %= 100;
                    for (int i = m_update100Index; i < m_entitiesForUpdate100.Count; i += 100)
                    {
                        var entity = m_entitiesForUpdate100[i];

                        string typeName = entity.GetType().Name;
                        cubeBlock = entity as MyCubeBlock;
                        character = entity as MyCharacter;
                        if (cubeBlock != null && ProfilePerBlock)
                        {
                            MySimpleProfiler.Begin("Blocks");
                            MySimpleProfiler.Begin(cubeBlock.DefinitionDisplayNameText);
                        }
                        else if (character != null)
                            MySimpleProfiler.Begin("CharactersB100");
                        //ProfilerShort.Begin(Partition.Select(typeName.GetHashCode(), "Part1", "Part2", "Part3"));
                        ProfilerShort.Begin(typeName);
                        if (entity.MarkedForClose == false)
                        {
                            entity.UpdateBeforeSimulation100();
                        }
                        ProfilerShort.End();
                        //ProfilerShort.End();
                        if (cubeBlock != null && ProfilePerBlock)
                        {
                            MySimpleProfiler.End("Blocks");
                            MySimpleProfiler.End(cubeBlock.DefinitionDisplayNameText);
                        }
                        else if (character != null)
                            MySimpleProfiler.End("CharactersB100");
                    }
                }
                ProfilerShort.End();
            }

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
                MyCubeBlock cubeBlock;
                MyCharacter character;

                ProfilerShort.Begin("UpdateAfter1");
                m_entitiesForUpdate.ApplyChanges();
                for (int i = 0; i < m_entitiesForUpdate.Count; i++)
                {
                    MyEntity entity = m_entitiesForUpdate[i];

                    string typeName = entity.GetType().Name;
                    cubeBlock = entity as MyCubeBlock;
                    character = entity as MyCharacter;
                    if (cubeBlock != null && ProfilePerBlock)
                    {
                        MySimpleProfiler.Begin("Blocks");
                        MySimpleProfiler.Begin(cubeBlock.DefinitionDisplayNameText);
                    }
                    else if (character != null)
                        MySimpleProfiler.Begin("CharactersA");
                    //ProfilerShort.Begin(Partition.Select(typeName.GetHashCode(), "Part1", "Part2", "Part3"));
                    ProfilerShort.Begin(typeName);
                    if (entity.MarkedForClose == false)
                    {
                        entity.UpdateAfterSimulation();
                    }
                    ProfilerShort.End();
                    //ProfilerShort.End();
                    if (cubeBlock != null && ProfilePerBlock)
                    {
                        MySimpleProfiler.End("Blocks");
                        MySimpleProfiler.End(cubeBlock.DefinitionDisplayNameText);
                    }
                    else if(character!=null)
                        MySimpleProfiler.End("CharactersA");
                }

                ProfilerShort.End();

                ProfilerShort.Begin("UpdateAfter10");
                m_entitiesForUpdate10.ApplyChanges();
                if (m_entitiesForUpdate10.Count > 0)
                {
                    for (int i = m_update10Index; i < m_entitiesForUpdate10.Count; i += 10)
                    {
                        MyEntity entity = m_entitiesForUpdate10[i];

                        string typeName = entity.GetType().Name;
                        cubeBlock = entity as MyCubeBlock;
                        character = entity as MyCharacter;
                        if (cubeBlock != null && ProfilePerBlock)
                        {
                            MySimpleProfiler.Begin("Blocks");
                            MySimpleProfiler.Begin(cubeBlock.DefinitionDisplayNameText);
                        }
                        else if (character != null)
                            MySimpleProfiler.Begin("CharactersA10");
                        //ProfilerShort.Begin(Partition.Select(typeName.GetHashCode(), "Part1", "Part2", "Part3"));
                        ProfilerShort.Begin(typeName);
                        if (entity.MarkedForClose == false)
                        {
                            entity.UpdateAfterSimulation10();
                        }
                        ProfilerShort.End();
                        //ProfilerShort.End();
                        if (cubeBlock != null && ProfilePerBlock)
                        {
                            MySimpleProfiler.End("Blocks");
                            MySimpleProfiler.End(cubeBlock.DefinitionDisplayNameText);
                        }
                        else if (character != null)
                            MySimpleProfiler.End("CharactersA10");
                    }
                }
                ProfilerShort.End();

                ProfilerShort.Begin("UpdateAfter100");
                m_entitiesForUpdate100.ApplyChanges();
                if (m_entitiesForUpdate100.Count > 0)
                {
                    for (int i = m_update100Index; i < m_entitiesForUpdate100.Count; i += 100)
                    {
                        MyEntity entity = m_entitiesForUpdate100[i];

                        string typeName = entity.GetType().Name;
                        cubeBlock = entity as MyCubeBlock;
                        character = entity as MyCharacter;
                        if (cubeBlock != null && ProfilePerBlock)
                        {
                            MySimpleProfiler.Begin("Blocks");
                            MySimpleProfiler.Begin(cubeBlock.DefinitionDisplayNameText);
                        }
                        else if (character != null)
                            MySimpleProfiler.Begin("CharactersA100");
                        //ProfilerShort.Begin(Partition.Select(typeName.GetHashCode(), "Part1", "Part2", "Part3"));
                        ProfilerShort.Begin(typeName);
                        if (entity.MarkedForClose == false)
                        {
                            entity.UpdateAfterSimulation100();
                        }
                        ProfilerShort.End();
                        //ProfilerShort.End();
                        if (cubeBlock != null && ProfilePerBlock)
                        {
                            MySimpleProfiler.End("Blocks");
                            MySimpleProfiler.End(cubeBlock.DefinitionDisplayNameText);
                        }
                        else if (character != null)
                            MySimpleProfiler.End("CharactersA100");
                    }
                }
                ProfilerShort.End();

                MyEntities.UpdateInProgress = false;

                MyEntities.DeleteRememberedEntities();
            }

            while (m_creationThread.ConsumeResult()) ; // Add entities created asynchronously

            VRageRender.MyRenderProxy.GetRenderProfiler().EndProfilingBlock();
        }
    }
}
