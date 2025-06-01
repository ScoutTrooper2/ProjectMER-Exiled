
using AdminToys;
using Exiled.API.Features;
using LabApi.Features.Wrappers;
using Mirror;
using ProjectMER.Features.Extensions;
using UnityEngine;
using PrimitiveObjectToy = AdminToys.PrimitiveObjectToy;
using Room = Exiled.API.Features.Room;

namespace ProjectMER.Features.Serializable;

public class SerializablePrimitive : SerializableObject
{
	public PrimitiveType PrimitiveType { get; set; } = PrimitiveType.Cube;
	public string Color { get; set; } = "#FF0000";
	public PrimitiveFlags PrimitiveFlags { get; set; } = (PrimitiveFlags)3;

	public override GameObject SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
	{
		PrimitiveObjectToy primitive = instance == null ? PrefabHelper.Spawn<PrimitiveObjectToy>(Exiled.API.Enums.PrefabType.PrimitiveObjectToy) : instance.GetComponent<PrimitiveObjectToy>();
		Vector3 position = room.GetAbsolutePosition(Position);
		Quaternion rotation = room.GetAbsoluteRotation(Rotation);
		_prevIndex = Index;

		primitive.transform.SetPositionAndRotation(position, rotation);
		primitive.transform.localScale = Scale;
		primitive.NetworkMovementSmoothing = 60;

		primitive.NetworkMaterialColor = Color.GetColorFromString();
		primitive.NetworkPrimitiveType = PrimitiveType;
		primitive.NetworkPrimitiveFlags = PrimitiveFlags;

		if (instance == null)
			NetworkServer.Spawn(primitive.gameObject);

		return primitive.gameObject;
	}
}
