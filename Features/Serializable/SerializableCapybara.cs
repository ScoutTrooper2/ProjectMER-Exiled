using Exiled.API.Features;
using LabApi.Features.Wrappers;
using Mirror;
using ProjectMER.Features.Extensions;
using UnityEngine;
using CapybaraToy = AdminToys.CapybaraToy;
using Room = Exiled.API.Features.Room;

namespace ProjectMER.Features.Serializable;

public class SerializableCapybara : SerializableObject
{
	public override GameObject SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
	{
		CapybaraToy capybara = instance == null ? PrefabHelper.Spawn<CapybaraToy>(Exiled.API.Enums.PrefabType.CapybaraToy) : instance.GetComponent<CapybaraToy>();
		Vector3 position = room.GetAbsolutePosition(Position);
		Quaternion rotation = room.GetAbsoluteRotation(Rotation);
		_prevIndex = Index;

		capybara.transform.SetPositionAndRotation(position, rotation);
		capybara.transform.localScale = Scale;

		capybara.Network_collisionsEnabled = true;

		if (instance == null)
			NetworkServer.Spawn(capybara.gameObject);

		return capybara.gameObject;
	}
}
