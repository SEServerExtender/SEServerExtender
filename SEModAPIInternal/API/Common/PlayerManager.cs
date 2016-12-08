using System.Diagnostics;
using Sandbox.Engine.Multiplayer;
using VRage.Game;
using VRage.Game.ModAPI;

namespace SEModAPIInternal.API.Common
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Sandbox;
	using Sandbox.Game.Multiplayer;
	using Sandbox.Game.World;
	using Sandbox.ModAPI;
	using SEModAPI.API.Utility;
	using SEModAPIInternal.API.Entity;
	using SEModAPIInternal.API.Server;
	using SEModAPIInternal.API.Utility;
	using SEModAPIInternal.Support;

	public class PlayerMap
	{

		public struct InternalPlayerItem
		{
            public string Name;
			public bool IsDead;
			public ulong SteamId;
			public string Model;
            public long PlayerId;
            public long EntityId;
		}

        public struct InternalIdentityItem
        {
            public string Name;
            public string Model;
            public long PlayerId;
            public long EntityId;

            public InternalIdentityItem(Object source)
            {
				Name = (string)BaseObject.GetEntityFieldValue( source, "<DisplayName>k__BackingField" );
				Model = (string)BaseObject.GetEntityFieldValue( source, "<Model>k__BackingField" );
				PlayerId = (long)BaseObject.GetEntityFieldValue(source, "<IdentityId>k__BackingField");
                EntityId = 0;
            }
        }

        public struct InternalClientItem : IComparable<InternalClientItem>
        {
            public ulong SteamId;
            public int SerialId;

            public InternalClientItem(Object source)
            {
				SteamId = (ulong)BaseObject.GetEntityFieldValue( source, "SteamId" );
				SerialId = (int)BaseObject.GetEntityFieldValue( source, "SerialId" );
            }

			public InternalClientItem(IMyPlayer player)
			{
				SteamId = player.SteamUserId;
				SerialId = 0;
			}

            public int CompareTo(InternalClientItem item)
            {
                if (SteamId < item.SteamId)
                {
                    return -1;
                }
                if (SteamId > item.SteamId)
                {
                    return 1;
                }
                if (SerialId < item.SerialId)
                {
                    return -1;
                }
                if (SerialId > item.SerialId)
                {
                    return 1;
                }
                return 0;
            }

            public override bool Equals(object item)
            {
                if (!(item is InternalClientItem))
                {
                    return false;
                }
                return (InternalClientItem)item == this;
            }

            public override int GetHashCode()
            {
                return SteamId.GetHashCode() * 571 ^ SerialId.GetHashCode();
            }

            public static InternalClientItem operator --(InternalClientItem item)
            {
                item.SerialId = item.SerialId - 1;
                return item;
            }

            public static bool operator ==(InternalClientItem item1, InternalClientItem item2)
            {
	            return item1.SteamId == item2.SteamId && item1.SerialId == item2.SerialId;
            }

	        public static InternalClientItem operator ++(InternalClientItem item)
            {
                item.SerialId = item.SerialId + 1;
                return item;
            }

            public static bool operator !=(InternalClientItem item1, InternalClientItem item2)
            {
                return !(item1 == item2);
            }

            public override string ToString()
            {
	            return string.Format( "{0}:{1}", SteamId, SerialId );
            }
        }

		#region "Attributes"

		private static PlayerMap _instance;

		public static string PlayerMapNamespace = "Sandbox.Game.Multiplayer";
		public static string PlayerMapClass = "MyPlayerCollection";

		public static string PlayerMapGetPlayerItemMappingField = "m_allIdentities";
		public static string PlayerMapGetSteamItemMappingField = "m_playerIdentityIds";
		public static string PlayerMapGetFastPlayerIdFromSteamIdMethod = "TryGetIdentityId";
		public static string PlayerMapGetFastIdentityFromPlayerIdMethod = "TryGetIdentity";
		public static string PlayerMapGetFastIdentityNameField = "<DisplayName>k__BackingField";

		public static string PlayerMapSessionNamespace = "Sandbox.Game.World";
		public static string PlayerMapSessionClass = "MySession";
		public static string PlayerMapSessionCameraField = "Cameras";
		public static string PlayerMapCameraDataClass = "MyCameraCollection";
		public static string PlayerMapGetCameraDataField = "m_entityCameraSettings";

		public static string PlayerMapForceDisplaySpawnMenu = "OnRespawnRequestFailure";

		public static string PlayerMapCreateNewPlayerInternalMethod = "CreateNewPlayerInternal";
		public static string PlayerMapCreateMyIdentity = "CreateNewIdentity";
		public static string PlayerMapPlayerDictionary = "m_players";

		public static string PlayerMapPlayerIdentity = "<Identity>k__BackingField";
		public static string PlayerMapIdentityPlayerId = "<IdentityId>k__BackingField";

        // SteamIdToPlayerId? public long TryGetIdentityId(ulong u00336ADF4D8C43635669729322024D2AD33, int u0032FA8049E153F637DEA99600B785ECCA0 = 0)


		//////////////////////////////////////////////////////

		public static string PlayerMapEntryNamespace = "";
		public static string PlayerMapEntryClass = "";

		#endregion

		#region "Constructors and Initializers"

		protected PlayerMap()
		{
			_instance = this;

			ApplicationLog.BaseLog.Info("Finished loading PlayerMap");
		}

		#endregion

		#region "Properties"

		public static PlayerMap Instance
		{
			get { return _instance ?? ( _instance = new PlayerMap( ) ); }
		}

		public MyPlayerCollection BackingObject => Sync.Players;

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(PlayerMapNamespace, PlayerMapClass);
				if (type1 == null)
					throw new Exception("Could not find internal type for PlayerMap");
				bool result = true;
                result &= Reflection.HasField(type1, PlayerMapGetPlayerItemMappingField);
                result &= Reflection.HasField(type1, PlayerMapGetSteamItemMappingField);
				result &= Reflection.HasMethod(type1, PlayerMapGetFastPlayerIdFromSteamIdMethod);
								
				Type type2 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(PlayerMapNamespace, PlayerMapCameraDataClass);
				if (type2 == null)
					throw new Exception("Could not find camera data type for PlayerMap");
				result &= Reflection.HasField(type2, PlayerMapGetCameraDataField);

				Type type3 = WorldManager.InternalType;
				if(type3 == null)
					throw new Exception("Could not find world manager type for PlayerMap");
				result &= Reflection.HasField(type3, PlayerMapSessionCameraField);

				return result;
			}
			catch (Exception ex)
			{
				ApplicationLog.BaseLog.Error(ex);
				return false;
			}
		}

	    public string GetPlayerNameFromPlayerId(long playerId)
	    {
	        if (playerId == 0)
	            return "nobody";

	        var playerDictionary = InternalGetPlayerDictionary();
	        if (!playerDictionary.ContainsKey(playerId))
	            return null;
	        return playerDictionary[playerId].Name;
	    }

        public string GetPlayerNameFromSteamId(ulong steamId)
        {
			if (steamId.ToString().StartsWith("9009"))
				return "Server";

            string playerName = steamId.ToString();

            Dictionary<ulong, InternalPlayerItem> steamDictionary = InternalGetSteamDictionary();
	        InternalPlayerItem internalPlayerItem;
	        return steamDictionary.TryGetValue( steamId, out internalPlayerItem ) ? internalPlayerItem.Name : playerName;
        }

        public ulong GetSteamId(long playerEntityId)
        {
			if (BackingObject == null)
				return 0;

			if(!(BackingObject is IMyPlayerCollection))
				return 0;

            IMyPlayerCollection collection = (IMyPlayerCollection)BackingObject;

            List<IMyPlayer> players = new List<IMyPlayer>();
            //collection.GetPlayers(players, x => x.Controller.ControlledEntity.Entity.EntityId == playerEntityId);
			collection.GetPlayers(players, x => x.Controller != null && x.Controller.ControlledEntity != null && x.Controller.ControlledEntity.Entity != null && x.Controller.ControlledEntity.Entity.EntityId == playerEntityId);
            if (players != null && players.Count > 0)
            {
                return players.First().SteamUserId;
            }

            return 0;
        }

		/// <summary>
		/// Gets a List of InternalPlayerItems matching a players name
		/// Can be partial or full name
		/// </summary>
		/// <param name="playerName"> the name of the player</param>
		/// <returns>returns a list of matches</returns>
		public List<InternalPlayerItem> GetPlayerItemsFromPlayerName(string playerName)
		{
			string lowerName = playerName.ToLower();

			List<InternalPlayerItem> playerItemstoReturn = new List<InternalPlayerItem>();

			foreach (InternalPlayerItem playerItem in InternalGetSteamDictionary().Values)
			{
				if (playerItem.Name.ToLower().Contains(lowerName))
				{
					// if the playeritem occurs more than once, replace it.
					if (playerItemstoReturn.Exists(x => String.Equals( x.Name, playerItem.Name, StringComparison.CurrentCultureIgnoreCase )))
					{
						playerItemstoReturn[playerItemstoReturn.IndexOf(playerItemstoReturn.First(x => x.Name.ToLower() == playerItem.Name.ToLower()))] = playerItem;
					}
					else
						playerItemstoReturn.Add(playerItem);
				}
			}
			return playerItemstoReturn;
		}

        public MyObjectBuilder_Checkpoint.PlayerItem GetPlayerItemFromPlayerId(long playerId)
        {            
            MyObjectBuilder_Checkpoint.PlayerItem playerItem = new MyObjectBuilder_Checkpoint.PlayerItem();

            try
            {
                Dictionary<long, InternalPlayerItem> playerDictionary = InternalGetPlayerDictionary();
                if (!playerDictionary.ContainsKey(playerId))
                    return playerItem;

                InternalPlayerItem item = playerDictionary[playerId];
                playerItem.PlayerId = item.PlayerId;
                playerItem.SteamId = item.SteamId;
                playerItem.Name = item.Name;
                playerItem.Model = item.Model;
                playerItem.IsDead = item.IsDead;
            }
            catch (Exception ex)
            {
                ApplicationLog.BaseLog.Error(ex);
            }

            return playerItem;
        }

        public ulong GetSteamIdFromPlayerName(string playerName, bool partial = false)
        {
            ulong steamId = 0;
            Dictionary<ulong, InternalPlayerItem> steamDictionary = InternalGetSteamDictionary();
			if(!partial)
				steamId = steamDictionary.FirstOrDefault(x => x.Value.Name == playerName && x.Value.SteamId != 0).Key;
			else
				steamId = steamDictionary.FirstOrDefault(x => x.Value.Name.ToLower().Contains(playerName.ToLower()) && x.Value.SteamId != 0).Key;

            if (steamId == 0)
            {
                try
                {
                    steamId = ulong.Parse(playerName);
                }
                catch (Exception ex)
                {
                    ApplicationLog.BaseLog.Error(ex);
                }
            }

            return steamId;
        }

		public ulong GetSteamIdFromPlayerId(long playerId)
		{
			Dictionary<long, InternalPlayerItem> allPlayers = InternalGetPlayerDictionary();
			if (allPlayers.ContainsKey(playerId))
				return allPlayers[playerId].SteamId;

			return 0;
		}

        public long GetPlayerEntityId(ulong steamId)
        {
            long result = 0;
            try
            {
                IMyPlayerCollection collection = (IMyPlayerCollection)BackingObject;

                List<IMyPlayer> players = new List<IMyPlayer>();
                collection.GetPlayers(players, x => x.SteamUserId == steamId);

                if (players.Count > 0)
                {
                    IMyPlayer player = players.FirstOrDefault();
                    if (player != null)
                    {
                        if (player.Controller != null && player.Controller.ControlledEntity != null && player.Controller.ControlledEntity.Entity != null)
                            result = player.Controller.ControlledEntity.Entity.EntityId;
                    }
                }
            }
            catch(Exception ex)
            {
                ApplicationLog.BaseLog.Error(ex);
            }
            return result;
        }

        public List<long> GetPlayerIdsFromSteamId(ulong steamId, bool ignoreDead = true)
        {
            List<long> matchingPlayerIds = new List<long>();

            try
            {
                if (ignoreDead)
                {
                    Dictionary<ulong, InternalPlayerItem> steamDictionary = InternalGetSteamDictionary();
	                InternalPlayerItem internalPlayerItem;
	                if (steamDictionary.TryGetValue( steamId, out internalPlayerItem ))
                        matchingPlayerIds.Add(internalPlayerItem.PlayerId);
                }
                else
                {
                    foreach (InternalPlayerItem item in InternalGetPlayerList())
                    {
                        if (item.SteamId == steamId)
                            matchingPlayerIds.Add(item.PlayerId);
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLog.BaseLog.Error(ex);
            }

            return matchingPlayerIds;
        }

        public List<long> GetPlayerIds()
        {
            return InternalGetPlayerList().Select(x => x.PlayerId).ToList();
        }

		public long GetFastPlayerIdFromSteamId(ulong steamId)
		{
			int num = 0;
			long result = (long)BaseObject.InvokeEntityMethod(BackingObject, PlayerMapGetFastPlayerIdFromSteamIdMethod, new object[] {steamId, num });
			return result;
		}

		public string GetFastPlayerNameFromSteamId(ulong steamId)
		{
			long playerId = GetFastPlayerIdFromSteamId(steamId);
			if (playerId < 1)
				return "";

			object identity = BaseObject.InvokeEntityMethod(BackingObject, PlayerMapGetFastIdentityFromPlayerIdMethod, new object[] { playerId });
			object result = BaseObject.GetEntityFieldValue(identity, PlayerMapGetFastIdentityNameField);
			if (result != null)
				return (string)result;
			else
				return "";
		}

		public IMyIdentity GetFastPlayerIdentityFromPlayerId(long playerId)
		{
			//FieldInfo identitiesInfo = BaseObject.GetEntityField(BackingObject, PlayerMapGetPlayerItemMappingField);
			object identities = BaseObject.GetEntityFieldValue(BackingObject, PlayerMapGetPlayerItemMappingField);
			if(identities == null)
				return null;

			//return (IMyIdentity)identitiesInfo.DeclaringType.GetMethod("get_Item").Invoke(identities, new object[] { playerId });
			if ((bool)identities.GetType().GetMethod("ContainsKey").Invoke(identities, new object[] { playerId }))
				return (IMyIdentity)identities.GetType().GetMethod("get_Item").Invoke(identities, new object[] { playerId });
			else
				return null;
		}

		/*
		public static string PlayerMapSessionCameraField = "=IG2GK7sB3xsB5eFsL2B0XDgz3l=";
		public static string PlayerMapGetCameraDataClass = "=Eveq1oFdjy7EIBPUC0ToMTRCSM=";
		public static string PlayerMapGetCameraDataField = "=3anAXWGikB6XQYAajgxQ6B4nMx=";
		 */
 
		public void ClearCameraData()
		{
			try
			{
				Object cameraDataObject = BaseObject.GetEntityFieldValue(WorldManager.Instance.BackingObject, PlayerMapSessionCameraField);
				Object cameraDictionaryObject = BaseObject.GetEntityFieldValue(cameraDataObject, PlayerMapGetCameraDataField);
				MethodInfo removeMethod = cameraDictionaryObject.GetType().GetMethod("Clear");
				removeMethod.Invoke(cameraDictionaryObject, new object[] { });				
			}
			catch (Exception ex)
			{
				ApplicationLog.BaseLog.Error(string.Format("ClearCameraData(): {0}", ex.ToString()));
			}
		}

        // --
        protected Dictionary<long, Object> InternalGetPlayerItemMapping()
        {
            try
            {
                Object rawPlayerItemMapping = BaseObject.GetEntityFieldValue(BackingObject, PlayerMapGetPlayerItemMappingField);
                Dictionary<long, Object> allPlayersMapping = UtilityFunctions.ConvertDictionary<long>(rawPlayerItemMapping);
                return allPlayersMapping;
            }
            catch (Exception ex)
            {
                ApplicationLog.BaseLog.Error(ex);
                return new Dictionary<long, Object>();
            }
        }

        protected Dictionary<object, long> InternalGetSteamIdMapping()
        {
            try
            {
                object rawPlayerItemMapping = BaseObject.GetEntityFieldValue(BackingObject, PlayerMapGetSteamItemMappingField);
                Dictionary<object, long> allSteamPlayersMapping = UtilityFunctions.ConvertDictionaryReverse<long>(rawPlayerItemMapping);
                return allSteamPlayersMapping;
            }
            catch (Exception ex)
            {
                ApplicationLog.BaseLog.Error(ex);
                return new Dictionary<object, long>();
            }
        }

        protected Dictionary<long, InternalPlayerItem> InternalGetPlayerDictionary()
        {
            Dictionary<long, InternalPlayerItem> result = new Dictionary<long, InternalPlayerItem>();

            Dictionary<long, InternalClientItem> allSteamList;
            Dictionary<long, InternalIdentityItem> allPlayerList;
            InternalGetReferenceLists(out allSteamList, out allPlayerList);

            foreach (KeyValuePair<long, InternalIdentityItem> p in allPlayerList)
            {
	            InternalPlayerItem item = new InternalPlayerItem { IsDead = false, Model = p.Value.Model, Name = p.Value.Name, PlayerId = p.Value.PlayerId, SteamId = 0 };
	            InternalClientItem internalClientItem;
	            if (allSteamList.TryGetValue( p.Value.PlayerId, out internalClientItem ))
                    item.SteamId = internalClientItem.SteamId;

                if (result.ContainsKey(item.PlayerId))
                    result[item.PlayerId] = item;
                else
                    result.Add(item.PlayerId, item);
            }

            return result;
        }

        protected Dictionary<ulong, InternalPlayerItem> InternalGetSteamDictionary()
        {
            Dictionary<ulong, InternalPlayerItem> result = new Dictionary<ulong, InternalPlayerItem>();

            Dictionary<long, InternalClientItem> allSteamList;
            Dictionary<long, InternalIdentityItem> allPlayerList;
            InternalGetReferenceLists(out allSteamList, out allPlayerList);

            foreach (KeyValuePair<long, InternalIdentityItem> p in allPlayerList)
            {
                InternalPlayerItem item = new InternalPlayerItem();
                item.IsDead = false;
                item.Model = p.Value.Model;
                item.Name = p.Value.Name;
                item.PlayerId = p.Value.PlayerId;
                item.SteamId = 0;
	            InternalClientItem internalClientItem;
	            if (allSteamList.TryGetValue( p.Value.PlayerId, out internalClientItem ))
                    item.SteamId = internalClientItem.SteamId;

                if (result.ContainsKey(item.SteamId))
                    result[item.SteamId] = item;
                else
                    result.Add(item.SteamId, item);
            }

            return result;
        }

        protected List<InternalPlayerItem> InternalGetPlayerList()
        {
            try
            {
                Dictionary<long, InternalClientItem> allSteamList;
                Dictionary<long, InternalIdentityItem> allPlayerList;
                InternalGetReferenceLists(out allSteamList, out allPlayerList);

                List<InternalPlayerItem> result = new List<InternalPlayerItem>();
                foreach (KeyValuePair<long, InternalIdentityItem> p in allPlayerList)
                {
                    for (int x = 0; x < result.Count; x++)
                    {
                        InternalPlayerItem test = result[x];
                        if (test.Name == p.Value.Name)
                            test.IsDead = true;
                    }

                    InternalPlayerItem item = new InternalPlayerItem();
                    item.IsDead = false;
                    item.Model = p.Value.Model;
                    item.Name = p.Value.Name;
                    item.PlayerId = p.Value.PlayerId;
                    item.SteamId = 0;
	                InternalClientItem internalClientItem;
	                if (allSteamList.TryGetValue( p.Value.PlayerId, out internalClientItem ))
                        item.SteamId = internalClientItem.SteamId;

                    result.Add(item);
                }

                return result;
            }
            catch (Exception ex)
            {
                ApplicationLog.BaseLog.Error(ex.ToString());
                return new List<InternalPlayerItem>();
            }
        }

        private void InternalGetReferenceLists(out Dictionary<long, InternalClientItem> allSteamList, out Dictionary<long, InternalIdentityItem> allPlayerList)
        {			
			if (MyAPIGateway.Players == null)
			{
				allSteamList = new Dictionary<long, InternalClientItem>();
				allPlayerList = new Dictionary<long, InternalIdentityItem>();
				return;
			}
			/*
			List<IMyPlayer> players = new List<IMyPlayer>();
			List<IMyIdentity> identities = new List<IMyIdentity>();
//			SandboxGameAssemblyWrapper.Instance.GameAction(() =>
//			{
				MyAPIGateway.Players.GetPlayers(players);
				MyAPIGateway.Players.GetAllIdentites(identities);
//			});

			allSteamList = new Dictionary<long, InternalClientItem>();
			foreach (IMyPlayer player in players)
			{
				InternalClientItem item = new InternalClientItem(player);
				allSteamList.Add(player.PlayerID, item);
			}

			allPlayerList = new Dictionary<long, InternalIdentityItem>();
			foreach (IMyIdentity identity in identities)
			{
				InternalIdentityItem item = new InternalIdentityItem(identity);
				allPlayerList.Add(identity.PlayerId, item);
			}
			 */ 
            Dictionary<object, long> steamList = InternalGetSteamIdMapping();
            allSteamList = new Dictionary<long, InternalClientItem>();
            foreach (KeyValuePair<object, long> p in steamList)
            {
                InternalClientItem item = new InternalClientItem(p.Key);
                allSteamList.Add(p.Value, item);
            }
			
            Dictionary<long, Object> playerList = InternalGetPlayerItemMapping();
            allPlayerList = new Dictionary<long, InternalIdentityItem>();
            foreach (KeyValuePair<long, object> p in playerList)
            {
                InternalIdentityItem item = new InternalIdentityItem(p.Value);
                allPlayerList.Add(p.Key, item);
            }
        }

		public bool CreatePlayer(string playerName, long playerId, ulong steamId, bool safe = true)
		{
			try
			{
				if (GetFastPlayerIdentityFromPlayerId(playerId) != null)
				{					
					return false;
				}

				if (GetFastPlayerIdFromSteamId(steamId) != 0)
				{
					return false;
				}

				MyPlayerCollection playerCollection = Sync.Players;

				// This method adds the player to online players, so it's not quite what we want, but the parameters allow us to pull types
				// This should be replaced by just grabbing playerIdentifierType from elsewhere, but this already took too long
				MethodInfo createNewPlayerMethod = BaseObject.GetEntityMethod(playerCollection, PlayerMapCreateNewPlayerInternalMethod);
				Type playerIdentifierType = createNewPlayerMethod.GetParameters()[3].ParameterType.GetElementType();

				SandboxGameAssemblyWrapper.Instance.GameAction(() =>
				{
					// Create Network Client
					//object networkClient = Activator.CreateInstance(networkClientType, new object[] { steamId });

					// Create Identity

					// Create MyPlayer.PlayerId type

					object playerIdentifier = Activator.CreateInstance(playerIdentifierType, steamId, 0 );

					// Adding to m_playerIdentityIds should save player to checkpoint
					//result = BaseObject.InvokeEntityMethod(myPlayerCollection, PlayerMapCreateNewPlayerInternalMethod, new object[] { myIdentity, myNetworkClient, playerName, myPlayerId });
					object playerIdentiferDictionary = BaseObject.GetEntityFieldValue(playerCollection, PlayerMapGetSteamItemMappingField);
					playerIdentiferDictionary.GetType().GetMethod("Add").Invoke(playerIdentiferDictionary, new[] { playerIdentifier, playerId });
				});

				return true;
			}
			catch (Exception ex)
			{
				ApplicationLog.BaseLog.Error(ex);
				return false;
			}
		}

		public bool UpdatePlayer(ulong steamId, long newPlayerId)
		{
			try
			{
				long playerId = Instance.GetFastPlayerIdFromSteamId(steamId);
				if(playerId == 0)
					return false;

				MyPlayerCollection playerCollection = Sync.Players;
				object playerIdentityDictionary = BaseObject.GetEntityFieldValue(playerCollection, PlayerMapGetPlayerItemMappingField);
				object playerIdentiferDictionary = BaseObject.GetEntityFieldValue(playerCollection, PlayerMapGetSteamItemMappingField);
				object onlinePlayers = BaseObject.GetEntityFieldValue(playerCollection, PlayerMapPlayerDictionary);

				Type playerIdentifierType = playerIdentiferDictionary.GetType().GetGenericArguments()[0];

				// Create MyPlayer.PlayerId type
				object playerIdentifier = Activator.CreateInstance(playerIdentifierType, steamId, 0 );

				bool result = false;

				if ((bool)playerIdentityDictionary.GetType().GetMethod("ContainsKey").Invoke(playerIdentityDictionary, new object[] { playerId }))
				{
					SandboxGameAssemblyWrapper.Instance.GameAction(() =>
					{
						try
						{
							object identity = playerIdentityDictionary.GetType().GetMethod("get_Item").Invoke(playerIdentityDictionary, new object[] { playerId });
							if (identity != null)
							{
								BaseObject.SetEntityFieldValue(identity, PlayerMapIdentityPlayerId, newPlayerId);
							}
						}
						catch (Exception ex)
						{
							result = false;
						}
					});
				}

				if ((bool)playerIdentiferDictionary.GetType().GetMethod("ContainsKey").Invoke(playerIdentiferDictionary, new[] { playerIdentifier }))
				{
					SandboxGameAssemblyWrapper.Instance.GameAction(() =>
					{
						try
						{
							playerIdentiferDictionary.GetType().GetMethod("Remove").Invoke(playerIdentiferDictionary, new[] { playerIdentifier });
							playerIdentiferDictionary.GetType().GetMethod("Add").Invoke(playerIdentiferDictionary, new[] { playerIdentifier, newPlayerId });
						}
						catch (Exception ex)
						{
							result = false;
						}
					});
				}

				if ((bool)onlinePlayers.GetType().GetMethod("ContainsKey").Invoke(onlinePlayers, new[] { playerIdentifier }))
				{
					SandboxGameAssemblyWrapper.Instance.GameAction(() =>
					{
						try
						{
							object player = onlinePlayers.GetType().GetMethod("get_Item").Invoke(onlinePlayers, new[] { playerIdentifier });
							if (player != null)
							{
								BaseObject.SetEntityFieldValue(player, PlayerMapPlayerIdentity, newPlayerId);
								//object identity = BaseObject.GetEntityFieldValue(player, PlayerMapPlayerIdentity);
								//identity.GetType().GetField(PlayerMapIdentityPlayerId).SetValue(identity, newPlayerId);
							}
						}
						catch (Exception ex)
						{
							result = false;
						}
					});
				}
			}
			catch(Exception ex)
			{
				return false;
			}

			return true;
		}

		public bool RemovePlayer(long playerId)
		{
			ulong steamId = GetSteamIdFromPlayerId(playerId);
			return steamId != 0 && RemovePlayer(steamId);
		}
		public bool RemovePlayer(ulong steamId)
		{
			try
			{
				MyPlayerCollection playerCollection = Sync.Players;
				object playerIdentiferDictionary = BaseObject.GetEntityFieldValue(playerCollection, PlayerMapGetSteamItemMappingField);
				object onlinePlayers = BaseObject.GetEntityFieldValue(playerCollection, PlayerMapPlayerDictionary);

				Type playerIdentifierType = playerIdentiferDictionary.GetType().GetGenericArguments()[0];

				// Create MyPlayer.PlayerId type
				object playerIdentifier = Activator.CreateInstance(playerIdentifierType, steamId, 0 );

				bool result = false;
				if ((bool)playerIdentiferDictionary.GetType().GetMethod("ContainsKey").Invoke(playerIdentiferDictionary, new[] { playerIdentifier }))
				{
					SandboxGameAssemblyWrapper.Instance.GameAction(() =>
					{
						result = (bool)playerIdentiferDictionary.GetType().GetMethod("Remove").Invoke(playerIdentiferDictionary, new[] { playerIdentifier });
					});
				}
				
				if ((bool)onlinePlayers.GetType().GetMethod("ContainsKey").Invoke(onlinePlayers, new[] { playerIdentifier }))
				{
					SandboxGameAssemblyWrapper.Instance.GameAction(() =>
					{
						result = result && (bool)onlinePlayers.GetType().GetMethod("Remove").Invoke(onlinePlayers, new[] { playerIdentifier });
					});
				}

				return result;
			}
			catch (Exception ex)
			{
				ApplicationLog.BaseLog.Error(ex);
				return false;
			}
		}

		public bool RemoveIdentity(long playerId)
		{
			try
			{
				MyPlayerCollection myPlayerCollection = Sync.Players;
				object myAllIdentities = BaseObject.GetEntityFieldValue(myPlayerCollection, PlayerMapGetPlayerItemMappingField);

				bool result = false;
				SandboxGameAssemblyWrapper.Instance.GameAction(() =>
				{
					result = (bool)myAllIdentities.GetType().GetMethod("Remove").Invoke(myAllIdentities, new object[] { playerId });
				});

				return result;
			}
			catch (Exception ex)
			{
				ApplicationLog.BaseLog.Error(ex);
				return false;
			}
		}

        #endregion
    }

	public class PlayerManager
	{
		#region "Attributes"

		private static PlayerManager m_instance;
        private static Type m_internalType;

		//public static string PlayerManagerNamespace = "";
		//public static string PlayerManagerClass = "08FBF1782D25BEBDA2070CAF8CE47D72";
        //public static string PlayerManagerPlayerMapField = "3F86E23829227B55C95CEB9F813578B2";

		public static string PlayerManagerNamespace = "Sandbox.Game.Multiplayer";
		public static string PlayerManagerClass = "Sync";
        public static string PlayerManagerPlayerMapField = "get_Players";

        //
        //=Yp93o6tmyK8AK8RCczmC0mKcgQ=
        //get_Players

		#endregion

		#region "Constructors and Initializers"

		protected PlayerManager()
		{
			m_instance = this;

			ApplicationLog.BaseLog.Info("Finished loading PlayerManager");
		}

		#endregion

		#region "Properties"

		public static PlayerManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new PlayerManager();

				return m_instance;
			}
		}

		public Object BackingObject
		{
			get
			{
				Object backingObject = WorldManager.Instance.InternalGetPlayerManager();

				return backingObject;
			}
		}

		public PlayerMap PlayerMap
		{
			get { return PlayerMap.Instance; }
		}

		public List<ulong> ConnectedPlayers
		{
			get
			{
				List<ulong> connectedPlayers = new List<ulong>(ServerNetworkManager.Instance.GetConnectedPlayers());

				return connectedPlayers;
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(PlayerManagerNamespace, PlayerManagerClass);
				if (type1 == null)
					throw new Exception("Could not find internal type for PlayerManager");
				bool result = true;
                result &= Reflection.HasMethod(type1, PlayerManagerPlayerMapField);

				return result;
			}
			catch (Exception ex)
			{
				ApplicationLog.BaseLog.Error(ex);
				return false;
			}
		}

		public void KickPlayer(ulong steamId)
		{
            SandboxGameAssemblyWrapper.Instance.GameAction(()=>MyMultiplayer.Static.KickClient( steamId ));
			//ServerNetworkManager.Instance.KickPlayer(steamId);
		}

		public void BanPlayer(ulong steamId)
		{
            SandboxGameAssemblyWrapper.Instance.GameAction(()=>MyMultiplayer.Static.BanClient( steamId, true ));
			//ServerNetworkManager.Instance.SetPlayerBan(steamId, true);
		}

		public void UnBanPlayer(ulong steamId)
		{
            SandboxGameAssemblyWrapper.Instance.GameAction(()=>MyMultiplayer.Static.BanClient( steamId, false ));
			//ServerNetworkManager.Instance.SetPlayerBan(steamId, false);
		}

        [Obsolete("Use MySession.Static.IsUserAdmin")]
		public bool IsUserAdmin(ulong remoteUserId)
		{
		    return MySandboxGame.ConfigDedicated.Administrators.Any( userId => remoteUserId.ToString().Equals( userId ) );
		    //return MyMultiplayer.Static.IsAdmin( remoteUserId );
		}

        [Obsolete("User MySession.Static.IsUserSpaceMaster")]
	    public bool IsUserPromoted( ulong remoteUserId )
	    {
	        //return MySession.Static.IsUserPromoted( remoteUserId );
	        return MySession.Static.IsUserSpaceMaster(remoteUserId);
	    }

		#endregion
	}
}

