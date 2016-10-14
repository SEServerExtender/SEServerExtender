using System.ComponentModel;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.World;
using SEModAPIInternal.API.Common;

namespace SEServerExtender.EntityWrappers
{
    public class FactionWrapper
    {
        [Browsable(false)]
        public readonly MyFaction Faction;

        public FactionWrapper(MyFaction faction)
        {
            Faction = faction;
        }

        [Category("General")]
        public string Name
        {
            get { return Faction.Name; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => MySession.Static.Factions.EditFaction(Faction.FactionId, Tag, value, Description, PrivateInfo)); }
        }

        [Category("General")]
        public string Tag
        {
            get { return Faction.Tag; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => MySession.Static.Factions.EditFaction(Faction.FactionId, value, Name, Description, PrivateInfo)); }
        }

        [Category("Details")]
        public string Description
        {
            get { return Faction.Description; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => MySession.Static.Factions.EditFaction(Faction.FactionId, Tag, Name, value, PrivateInfo)); }
        }

        [Category("Details")]
        public string PrivateInfo
        {
            get { return Faction.PrivateInfo; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => MySession.Static.Factions.EditFaction(Faction.FactionId, Tag, Name, Description, value)); }
        }

        [Category("General")]
        [ReadOnly(true)]
        public string Founder
        {
            get { return $"{PlayerMap.Instance.GetPlayerNameFromPlayerId(Faction.FounderId)}: {Faction.FounderId}"; }
        }

        [Category("Settings")]
        public bool AcceptAll
        {
            get { return Faction.AutoAcceptMember; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => MySession.Static.Factions.ChangeAutoAccept(Faction.FactionId, Faction.FounderId, value, Faction.AutoAcceptPeace)); }
        }

        [Category("Settings")]
        public bool AcceptPeace
        {
            get { return Faction.AutoAcceptPeace; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => MySession.Static.Factions.ChangeAutoAccept(Faction.FactionId, Faction.FounderId, Faction.AutoAcceptMember, value)); }
        }

        //There doesn't appear to be a sync method for this?
        [Category("Settings")]
        [Description("When this is false, players can't hurt each other with rifles or hand tools. Does not apply to grids.")]
        public bool FriendlyFire
        {
            get { return Faction.EnableFriendlyFire; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Faction.EnableFriendlyFire = value); }
        }

        //no sync for this either
        [Category("Settings")]
        public bool AcceptHumans
        {
            get { return Faction.AcceptHumans; }
            set { SandboxGameAssemblyWrapper.Instance.GameAction(() => Faction.AcceptHumans = value); }
        }

        public void Accept(long playerId)
        {
            SandboxGameAssemblyWrapper.Instance.GameAction(() => MyFactionCollection.AcceptJoin(Faction.FactionId, playerId));
        }

        public void Deny(long playerId)
        {
            SandboxGameAssemblyWrapper.Instance.GameAction(() => MyFactionCollection.CancelJoinRequest(Faction.FactionId, playerId));
        }

        public void Kick(long playerId)
        {
            SandboxGameAssemblyWrapper.Instance.GameAction(() => MyFactionCollection.KickMember(Faction.FactionId, playerId));
        }

        public void Promote(long playerId)
        {
            SandboxGameAssemblyWrapper.Instance.GameAction(() => MyFactionCollection.PromoteMember(Faction.FactionId, playerId));
        }

        public void Demote(long playerId)
        {
            SandboxGameAssemblyWrapper.Instance.GameAction(() => MyFactionCollection.DemoteMember(Faction.FactionId, playerId));
        }
    }
}