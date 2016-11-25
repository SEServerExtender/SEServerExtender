using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using SEModAPIInternal.API.Common;
using VRage.Game;
using VRageMath;

namespace SEServerExtender.EntityWrappers
{
    public class CubeGridWrapper
    {
        [Browsable(false)]
        public readonly MyCubeGrid Grid;

        public CubeGridWrapper(MyCubeGrid grid)
        {
            Grid = grid;
        }

        [Category("General")]
        [Description("This doesn't check for collisions, use at your own risk.")]
        public string Position
        {
            get { return Grid.PositionComp.GetPosition().ToString(); }
            set
            {
                Vector3D val;
                if (!Vector3D.TryParse(value, out val))
                    return;

                SandboxGameAssemblyWrapper.Instance.GameAction(() => Grid.PositionComp.SetPosition(val));
            }
        }

        [Category("General")]
        public string PositionGPS
        {
            get
            {
                //GPS:rexxar #1:-2.5:-5.01:10.83:
                Vector3D pos = Grid.PositionComp.GetPosition();
                return $"GPS:{Grid.DisplayName}:{pos.X}:{pos.Y}:{pos.Z}:";
            }
            set
            {
                //copied from SE because MyGpsCollection is private
                foreach (Match match in Regex.Matches(value, @"GPS:([^:]{0,32}):([\d\.-]*):([\d\.-]*):([\d\.-]*):"))
                {
                    double x, y, z;
                    try
                    {
                        x = double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
                        x = Math.Round(x, 2);
                        y = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
                        y = Math.Round(y, 2);
                        z = double.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture);
                        z = Math.Round(z, 2);
                    }
                    catch (SystemException)
                    {
                        continue;
                    }

                    SandboxGameAssemblyWrapper.Instance.GameAction(() => Grid.PositionComp.SetPosition(new Vector3D(x, y, z)));
                    return;
                }
            }
        }

        [Category("General")]
        public int BlockCount
        {
            get { return Grid.BlocksCount; }
        }

        [Category("General")]
        public float Speed
        {
            get
            {
                if (Grid.Physics == null || Grid.IsStatic)
                    return 0;
                return Grid.Physics.LinearVelocity.Length();
            }
            set
            {
                if (Grid.Physics == null || Grid.IsStatic)
                    return;

                SandboxGameAssemblyWrapper.Instance.GameAction(() =>
                                                               {
                                                                   Grid.Physics.SetSpeeds(
                                                                       Vector3.Normalize(Grid.Physics.LinearVelocity) * value,
                                                                       Grid.Physics.AngularVelocity);
                                                               });
            }
        }

        [Category("General")]
        public long EntityId
        {
            get { return Grid.EntityId; }
        }

        [Category("General")]
        public string Name
        {
            get { return Grid.DisplayName; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Grid.ChangeDisplayNameRequest(value)); }
        }

        [Category("General")]
        public bool Static
        {
            get { return Grid.IsStatic; }
            set
            {
                SandboxGameAssemblyWrapper.Instance.GameAction(() =>
                                                               {
                                                                   if (Grid.Physics == null)
                                                                       return;

                                                                   if (!value)
                                                                       Grid.ConvertToDynamic();
                                                                   if (value) // && !MySession.Static.EnableStationVoxelSupport)
                                                                   {
                                                                       Grid.Physics.ClearSpeed();
                                                                       Grid.ConvertToStatic();
                                                                   }
                                                               });
            }
        }

        [Category("General")]
        public float Mass
        {
            get
            {
                if (Grid.IsStatic)
                    return 0;
                return Grid.Physics.Mass;
            }
        }

        [Category("General")]
        public Vector3D Size
        {
            get
            {
                //get the width of blocks for this grid size
                float blocksize = MyDefinitionManager.Static.GetCubeSize(Grid.GridSizeEnum);

                //get the halfextents of the grid, then multiply by block size to get world halfextents
                //add one so the line sits on the outside edge of the block instead of the center
                return new Vector3D(
                    (Math.Abs(Grid.Max.X - Grid.Min.X) + 1) * blocksize / 2,
                    (Math.Abs(Grid.Max.Y - Grid.Min.Y) + 1) * blocksize / 2,
                    (Math.Abs(Grid.Max.Z - Grid.Min.Z) + 1) * blocksize / 2);
            }
        }

        [Category("General")]
        public MyCubeSize GridSize
        {
            get { return Grid.GridSizeEnum; }
        }

        [Category("Ownership")]
        [Description("This will set the owner of ALL BLOCKS on the grid to the given playerId")]
        public long MajorityOwner
        {
            get
            {
                if (Grid.BigOwners.Count > 0)
                    return Grid.BigOwners[0];
                return 0;
            }
            set
            {
                if (value != 0 && !PlayerMap.Instance.GetPlayerIds().Contains(value))
                    throw new Exception("That is not a valid PlayerID.");

                DialogResult messageResult =
                    MessageBox.Show(
                        $"Are you sure you want to change the owner of ALL BLOCKS on this grid to {PlayerMap.Instance.GetPlayerNameFromPlayerId(value)}?",
                        "Confirm",
                        MessageBoxButtons.YesNo);
                if (messageResult == DialogResult.No)
                    return;

                SandboxGameAssemblyWrapper.Instance.GameAction(
                    () => Grid.ChangeGridOwner(value, MyOwnershipShareModeEnum.Faction));
            }
        }

        [Category("Ownership")]
        public string[] BigOwners
        {
            get
            {
                var result = new List<string>();
                foreach (long ownerId in Grid.BigOwners)
                    result.Add($"{PlayerMap.Instance.GetPlayerNameFromPlayerId(ownerId)}: {ownerId}");

                return result.ToArray();
            }
        }

        [Category("Ownership")]
        public string[] SmallOwners
        {
            get
            {
                var result = new List<string>();
                foreach (long ownerId in Grid.SmallOwners)
                {
                    if (ownerId == 0)
                        continue;
                    result.Add($"{PlayerMap.Instance.GetPlayerNameFromPlayerId(ownerId)}: {ownerId}");
                }

                return result.ToArray();
            }
        }

        [Category("Ownership")]
        [Description("Sets the author of ALL BLOCKS on the grid to the given player. Ignore the value displayed, it will always be 0")]
        public long SetFullAuthor
        {
            get { return 0; }
            set
            {
                if (value != 0 && !PlayerMap.Instance.GetPlayerIds().Contains(value))
                    throw new Exception("That is not a valid PlayerID.");

                DialogResult messageResult =
                    MessageBox.Show(
                        $"Are you sure you want to change the author of ALL BLOCKS on this grid to {PlayerMap.Instance.GetPlayerNameFromPlayerId(value)}?",
                        "Confirm",
                        MessageBoxButtons.YesNo);
                if (messageResult == DialogResult.No)
                    return;

                SandboxGameAssemblyWrapper.Instance.GameAction(() =>
                                                               {
                                                                   FieldInfo builtBy = typeof(MySlimBlock).GetField("m_builtByID", BindingFlags.NonPublic|BindingFlags.Instance);
                                                                   foreach (var block in Grid.GetBlocks())
                                                                   {
                                                                       if(block==null)
                                                                           continue;
                                                                       block.RemoveAuthorship();
                                                                       builtBy.SetValue(block, value);
                                                                       block.AddAuthorship();
                                                                   }
                                                               });
            }
        }

        [Category("Misc")]
        public bool MarkedForClose
        {
            get { return Grid.MarkedForClose; }
        }

        [Category("Misc")]
        public bool Closed
        {
            get { return Grid.Closed; }
        }

        [Category("Misc")]
        public bool InScene
        {
            get { return Grid.InScene; }
        }

        [Category("Misc")]
        public Vector3I Max
        {
            get { return Grid.Max; }
        }

        [Category("Misc")]
        public Vector3I Min
        {
            get { return Grid.Min; }
        }
    }
}