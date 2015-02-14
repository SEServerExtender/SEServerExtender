namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	using System.Runtime.Serialization;
	using Sandbox.Common.ObjectBuilders;
	using Sandbox.Common.ObjectBuilders.Definitions;
	using VRage;

	[DataContract]
	public class ProductionQueueItem
	{
		public decimal Amount;
		public SerializableDefinitionId Id;
		public uint ItemId;

		public ProductionQueueItem( decimal amount, SerializableDefinitionId id, uint itemId )
		{
			Amount = amount;
			Id = id;
			ItemId = itemId;
		}

		public ProductionQueueItem( MyObjectBuilder_ProductionBlock.QueueItem q )
		{
			Amount = (decimal)q.Amount;
			Id = q.Id;
			ItemId = q.ItemId.GetValueOrDefault( 0 );
		}

		public static implicit operator ProductionQueueItem( MyObjectBuilder_ProductionBlock.QueueItem q )
		{
			return new ProductionQueueItem( q );
		}

		public static implicit operator MyObjectBuilder_ProductionBlock.QueueItem( ProductionQueueItem q )
		{
			MyObjectBuilder_ProductionBlock.QueueItem item = new MyObjectBuilder_ProductionBlock.QueueItem { Amount = (MyFixedPoint) q.Amount, Id = q.Id, ItemId = q.ItemId };

			return item;
		}
	}
}