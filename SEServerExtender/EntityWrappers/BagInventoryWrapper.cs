using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Sandbox.Game.Entities;
using SEModAPIInternal.API.Common;
using VRageMath;

namespace SEServerExtender.EntityWrappers
{
    public class BagInventoryWrapper
    {
        [Browsable(false)]
        public MyInventoryBagEntity Entity;

        public BagInventoryWrapper(MyInventoryBagEntity entity)
        {
            Entity = entity;
        }

        [Category("General")]
        public float Mass
        {
            get { return (float)Entity.GetInventory().CurrentMass; }
        }

        [Category("General")]
        public long EntityId
        {
            get { return Entity.EntityId; }
        }

        [Category("General")]
        public float Speed
        {
            get
            {
                if (Entity.Physics == null)
                    return 0;
                return Entity.Physics.LinearVelocity.Length();
            }
            set
            {
                if (Entity.Physics == null)
                    return;

                SandboxGameAssemblyWrapper.Instance.GameAction(() =>
                                                               {
                                                                   Entity.Physics.SetSpeeds(
                                                                       Vector3.Normalize(Entity.Physics.LinearVelocity) * value,
                                                                       Entity.Physics.AngularVelocity);
                                                               });
            }
        }

        [Category("General")]
        public string Position
        {
            get { return Entity.PositionComp.GetPosition().ToString(); }
            set
            {
                var val = new Vector3D();
                if (!Vector3D.TryParse(value, out val))
                    return;

                SandboxGameAssemblyWrapper.Instance.GameAction(() => Entity.PositionComp.SetPosition(val));
            }
        }

        [Category("General")]
        public string PositionGPS
        {
            get
            {
                //GPS:rexxar #1:-2.5:-5.01:10.83:
                Vector3D pos = Entity.PositionComp.GetPosition();
                return $"GPS:{Entity.DisplayName}:{pos.X}:{pos.Y}:{pos.Z}:";
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

                    SandboxGameAssemblyWrapper.Instance.GameAction(() => Entity.PositionComp.SetPosition(new Vector3D(x, y, z)));
                    return;
                }
            }
        }
    }
}