using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Game.World;
using SEModAPIInternal.API.Common;
using VRage.Game;

namespace SEServerExtender.EntityWrappers.Factions
{
    public class FactionMemberWrapper
    {
        [Browsable(false)]
        public readonly MyFactionMember Member;
        private readonly FactionWrapper _faction;

        public FactionMemberWrapper(MyFactionMember member, FactionWrapper faction )
        {
            Member = member;
            _faction = faction;
        }

        [Category("General")]
        [ReadOnly(true)]
        public string Name
        {
            get { return PlayerMap.Instance.GetPlayerNameFromPlayerId(Member.PlayerId); }
        }

        [Category("General")]
        [ReadOnly(true)]
        public long PlayerId
        {
            get { return Member.PlayerId; }
        }

        [Category("General")]
        public bool Leader
        {
            get { return _faction.Faction.IsLeader(PlayerId); }
            set
            {
                if (value)
                    _faction.Promote(Member.PlayerId);
                else
                    _faction.Demote(Member.PlayerId);
            }
        }
        
        [Category("General")]
        [ReadOnly(true)]
        public bool Founder
        {
            get { return _faction.Faction.IsFounder(PlayerId); }
            /*
            set
            {
                if (_faction == null)
                    return;

                if (value)
                    _faction.Faction.PromoteToFounder(Member.PlayerId);
                else
                    _faction.Demote(Member.PlayerId);
            }
            */
        }
    }
}
