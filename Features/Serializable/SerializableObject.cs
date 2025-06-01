using Exiled.API.Enums;
using LabApi.Features.Wrappers;
using UnityEngine;
using YamlDotNet.Serialization;
using Room = Exiled.API.Features.Room;

namespace ProjectMER.Features.Serializable;

public abstract class SerializableObject
{
	public virtual Vector3 Position { get; set; } = Vector3.zero;
	public virtual Vector3 Rotation { get; set; } = Vector3.zero;
	public virtual Vector3 Scale { get; set; } = Vector3.one;

	public virtual RoomType RoomType { get; set; } = RoomType.Unknown;

	public virtual int Index { get; set; } = -1;

	public virtual GameObject? SpawnOrUpdateObject(Room? room = null, GameObject? instance = null) => throw new NotSupportedException();

	[YamlIgnore]
	public virtual bool RequiresReloading => Index != _prevIndex;

	public int _prevIndex;
}
