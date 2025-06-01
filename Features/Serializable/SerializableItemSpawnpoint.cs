using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.Pickups;
using MEC;
using ProjectMER.Events.Handlers.Internal;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Interfaces;
using UnityEngine;
using PrimitiveObjectToy = AdminToys.PrimitiveObjectToy;
using Room = Exiled.API.Features.Room;

namespace ProjectMER.Features.Serializable;

public class SerializableItemSpawnpoint : SerializableObject, IIndicatorDefinition
{
	public ItemType ItemType { get; set; } = ItemType.Lantern;
	public uint CustomItem { get; set; } = 0;
    public float Weight { get; set; } = -1;
	public string AttachmentsCode { get; set; } = "-1";
	public uint NumberOfItems { get; set; } = 1;
	public int NumberOfUses { get; set; } = 1;
	public bool UseGravity { get; set; } = true;
	public bool CanBePickedUp { get; set; } = true;

	public override GameObject? SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
	{
		GameObject itemSpawnPoint = instance ?? new GameObject("ItemSpawnpoint");
		Vector3 position = room.GetAbsolutePosition(Position);
		Quaternion rotation = room.GetAbsoluteRotation(Rotation);
		_prevIndex = Index;

		itemSpawnPoint.transform.SetPositionAndRotation(position, rotation);

		if (instance != null)
		{
			foreach (ItemPickupBase pickup in instance.GetComponentsInChildren<ItemPickupBase>())
			{
				PickupEventsHandler.PickupUsesLeft.Remove(pickup.Info.Serial);
				pickup.DestroySelf();
			}
		}

		for (int i = 0; i < NumberOfItems; i++)
		{
			Pickup pickup = null;

            if (CustomItem > 0)
			{
                pickup = Exiled.CustomItems.API.Features.CustomItem.Get(CustomItem).Spawn(position);
			}
			else
			{
				pickup = Pickup.CreateAndSpawn(ItemType, Vector3.zero);
			}
			
            //, position, rotation, Scale

			pickup.Position = position;
			pickup.Rotation = rotation;
			pickup.Scale = Scale; 

            pickup.Transform.parent = itemSpawnPoint.transform;
			if (Weight != -1)
				pickup.Weight = Weight;

			pickup.Rigidbody!.isKinematic = !UseGravity;
			pickup.IsLocked = !CanBePickedUp;
			PickupEventsHandler.PickupUsesLeft.Add(pickup.Serial, NumberOfUses);

			pickup.Spawn();

			if (pickup is FirearmPickup firearmPickup)
			{
				Timing.CallDelayed(0.01f, () =>
				{
					firearmPickup.Base.OnDistributed();
					firearmPickup.Attachments = uint.TryParse(AttachmentsCode, out uint attachmentsCode) ? attachmentsCode : AttachmentsUtils.GetRandomAttachmentsCode(firearmPickup.Type);
					if (firearmPickup.Base.Template.TryGetModule(out MagazineModule magazineModule))
						magazineModule.ServerResyncData();
				});
			}
		}

		return itemSpawnPoint.gameObject;
	}
	public GameObject SpawnOrUpdateIndicator(Room room, GameObject? instance = null)
	{
		PrimitiveObjectToy cube;

		Vector3 position = room.GetAbsolutePosition(Position);
		Quaternion rotation = room.GetAbsoluteRotation(Rotation);

		if (instance == null)
		{
			cube = PrefabHelper.Spawn<PrimitiveObjectToy>(Exiled.API.Enums.PrefabType.PrimitiveObjectToy);
			cube.NetworkPrimitiveType = PrimitiveType.Cube;
			cube.NetworkPrimitiveFlags = AdminToys.PrimitiveFlags.Visible;
			cube.NetworkMaterialColor = new Color(0f, 1f, 0f, 0.9f);
			cube.transform.localScale = Vector3.one * 0.25f;
		}
		else
		{
			cube = instance.GetComponent<PrimitiveObjectToy>();
		}

		cube.transform.SetPositionAndRotation(position, rotation);

		return cube.gameObject;
	}
}
