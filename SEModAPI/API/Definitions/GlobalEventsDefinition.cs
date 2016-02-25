using VRage.Game;

namespace SEModAPI.API.Definitions
{
	using global::Sandbox.Common.ObjectBuilders.Definitions;

	public class GlobalEventsDefinition : ObjectOverLayerDefinition<MyObjectBuilder_GlobalEventDefinition>
	{
		#region "Constructors and Initializers"

		public GlobalEventsDefinition(MyObjectBuilder_GlobalEventDefinition definition): base(definition)
		{}

		#endregion

		#region "Properties"

		public MyGlobalEventTypeEnum EventType
		{
			get { return MyGlobalEventTypeEnum.InvalidEventType; }
			set
			{
				//if (m_baseDefinition.EventType == value) return;
				//m_baseDefinition.EventType = value;
				Changed = true;
			}
		}

		public string DisplayName
		{
			get { return m_baseDefinition.DisplayName; }
			set
			{
				if (m_baseDefinition.DisplayName == value) return;
				m_baseDefinition.DisplayName = value;
				Changed = true;
			}
		}

		public long MinActivation
		{
			get { return m_baseDefinition.MinActivationTimeMs.GetValueOrDefault(0); }
            set
            {
				if (m_baseDefinition.MinActivationTimeMs == value) return;
				m_baseDefinition.MinActivationTimeMs = value;
                Changed = true;
            }
		}

		public long MaxActivation
		{
			get { return m_baseDefinition.MaxActivationTimeMs.GetValueOrDefault(0); }
            set
            {
				if (m_baseDefinition.MaxActivationTimeMs == value) return;
				m_baseDefinition.MaxActivationTimeMs = value;
                Changed = true;
            }
		}

		public long FirstActivation
		{
			get { return m_baseDefinition.FirstActivationTimeMs.GetValueOrDefault(0); }
            set
            {
				if (m_baseDefinition.FirstActivationTimeMs == value) return;
				m_baseDefinition.FirstActivationTimeMs = value;
                Changed = true;
            }
		}

	    #endregion

        #region "Methods"

        protected override string GetNameFrom(MyObjectBuilder_GlobalEventDefinition definition)
	    {
	        return definition.DisplayName;
        }

        #endregion
    }

	public class GlobalEventsDefinitionsManager : SerializableDefinitionsManager<MyObjectBuilder_GlobalEventDefinition, GlobalEventsDefinition>
	{
	}
}
