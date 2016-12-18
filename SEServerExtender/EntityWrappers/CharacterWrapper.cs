using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Sandbox.Engine.Multiplayer;
using Sandbox.Game;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.Entities.Character.Components;
using Sandbox.Game.EntityComponents;
using Sandbox.Game.Gui;
using Sandbox.Game.World;
using SEModAPIExtensions.API;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Server;
using SpaceEngineers.Game.World;
using VRage.Game.ModAPI;
using VRage.Network;
using VRageMath;

namespace SEServerExtender.EntityWrappers
{
    public class CharacterWrapper
    {
        [Browsable(false)]
        public readonly MyCharacter Character;

        public CharacterWrapper(MyCharacter character)
        {
            Character = character;
        }

        [Category("General")]
        public string Name
        {
            get { return Character.DisplayName; }
        }

        [Category("General")]
        public long EntityId
        {
            get { return Character.EntityId; }
        }

        [Category("General")]
        public ulong SteamId
        {
            get { return Character.ControllerInfo.Controller.Player.Client.SteamUserId; }
        }

        [Category( "General" )]
        public MyPromoteLevel PromoteLevel
        {
            get { return MySession.Static.GetUserPromoteLevel( SteamId ); }
            set { SandboxGameAssemblyWrapper.Instance.BeginGameAction( () => MySession.Static.SetUserPromoteLevel( SteamId, value ), null, null ); }
        }

        [Category("General")]
        public float Mass
        {
            get { return Character.CurrentMass; }
        }

        [Category("General")]
        public float Speed
        {
            get
            {
                if (Character.Physics == null)
                    return 0;
                return Character.Physics.LinearVelocity.Length();
            }
            set
            {
                if (Character.Physics == null)
                    return;

                SandboxGameAssemblyWrapper.Instance.GameAction(() =>
                                                               {
                                                                   Character.Physics.SetSpeeds(
                                                                       Vector3.Normalize(Character.Physics.LinearVelocity) * value,
                                                                       Character.Physics.AngularVelocity);
                                                               });
            }
        }

        [Category("General")]
        public string Position
        {
            get { return Character.PositionComp.GetPosition().ToString(); }
            set
            {
                Vector3D val;
                if (!Vector3D.TryParse(value, out val))
                    return;

                SandboxGameAssemblyWrapper.Instance.GameAction(() => Character.PositionComp.SetPosition(val));
            }
        }

        [Category("General")]
        public string PositionGPS
        {
            get
            {
                //GPS:rexxar #1:-2.5:-5.01:10.83:
                Vector3D pos = Character.PositionComp.GetPosition();
                return $"GPS:{Character.DisplayName}:{pos.X}:{pos.Y}:{pos.Z}:";
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

                    SandboxGameAssemblyWrapper.Instance.GameAction(() => Character.PositionComp.SetPosition(new Vector3D(x, y, z)));
                    return;
                }
            }
        }

        [Category("General")]
        public float Battery
        {
            get { return Character.SuitEnergyLevel; }
            set
            {
                if (value > 1)
                    value = 1;
                if (value < 0)
                    value = 0;

                SandboxGameAssemblyWrapper.Instance.GameAction(() =>
                                                               {
                                                                   Character.SuitBattery.ResourceSource.SetRemainingCapacityByType(
                                                                       MyResourceDistributorComponent.ElectricityId, value * MyEnergyConstants.BATTERY_MAX_CAPACITY);
                                                               });
            }
        }

        [Category("General")]
        public float Health
        {
            get { return Character.Integrity; }
            /*
            set
            {
                if ( value > Character.Integrity )
                {
                    var effect = new MyObjectBuilder_EntityStatRegenEffect
                                 {
                                     MinRegenRatio = value - Character.Integrity,
                                     MaxRegenRatio = value - Character.Integrity,
                                     Interval = 100f / 240f,
                                     TickAmount = 1,
                                     Duration = 1,
                                     SubtypeName="SESEHeal"
                                 };

                    Character.StatComp.Health.AddEffect( effect );
                    Character.StatComp.DoAction( "SESEHeal" );
                }
                else
                {
                    Character.DoDamage( Character.Integrity - value, MyStringHash.GetOrCompute( "SESEDamage" ), true );
                }
            }
            */
        }

        [Category("General")]
        public float Oxygen
        {
            get { return Character.OxygenComponent.SuitOxygenLevel; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Character.OxygenComponent.CharacterGasSource.SetRemainingCapacityByType(MyCharacterOxygenComponent.OxygenId, value)); }
        }

        [Category("General")]
        public float Hydrogen
        {
            get { return Character.GetSuitGasFillLevel(MyCharacterOxygenComponent.HydrogenId); }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Character.OxygenComponent.CharacterGasSource.SetRemainingCapacityByType(MyCharacterOxygenComponent.HydrogenId, value)); }
        }
    }
}