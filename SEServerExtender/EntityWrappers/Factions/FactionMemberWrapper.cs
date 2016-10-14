using System.ComponentModel;
using SEModAPIInternal.API.Common;
using VRage.Game;

namespace SEServerExtender.EntityWrappers.Factions
{
    public class FactionMemberWrapper
    {
        private readonly FactionWrapper _faction;

        [Browsable(false)]
        public readonly MyFactionMember Member;

        public FactionMemberWrapper(MyFactionMember member, FactionWrapper faction)
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