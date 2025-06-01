using Exiled.API.Enums;
using LabApi.Features.Wrappers;
using MapGeneration;
using NorthwoodLib.Pools;
using ProjectMER.Features.Serializable;
using System.Security.Policy;
using UnityEngine;
using Room = Exiled.API.Features.Room;
namespace ProjectMER.Features.Extensions;

public static class RoomExtensions
{
	public static string GetRoomStringId(this Room room) => $"{room.Type}";

    public static List<Room> GetRooms(this SerializableObject serializableObject)
    {
        return Exiled.API.Features.Room.List.Where(x => x != null && x.Zone == GetZone(serializableObject.RoomType) && x.Type == serializableObject.RoomType).ToList();
    }

    public static ZoneType GetZone(this RoomType room)
    {
        return room switch
        {
            RoomType.LczArmory or
            RoomType.LczCurve or
            RoomType.LczStraight or
            RoomType.Lcz914 or
            RoomType.LczCrossing or
            RoomType.LczTCross or
            RoomType.LczCafe or
            RoomType.LczPlants or
            RoomType.LczToilets or
            RoomType.LczAirlock or
            RoomType.Lcz173 or
            RoomType.LczClassDSpawn or
            RoomType.LczCheckpointA or
            RoomType.LczCheckpointB or
            RoomType.LczGlassBox or
            RoomType.Lcz330 =>
                ZoneType.LightContainment,

            RoomType.Hcz079 or
            RoomType.HczEzCheckpointA or
            RoomType.HczEzCheckpointB or
            RoomType.HczArmory or
            RoomType.Hcz939 or
            RoomType.HczHid or
            RoomType.Hcz049 or
            RoomType.HczCrossing or
            RoomType.Hcz106 or
            RoomType.HczNuke or
            RoomType.HczTesla or
            RoomType.HczCurve or
            RoomType.Hcz096 or
            RoomType.HczStraight or
            RoomType.HczTestRoom or
            RoomType.HczElevatorA or
            RoomType.HczElevatorB or
            RoomType.HczCrossRoomWater or
            RoomType.HczCornerDeep or
            RoomType.HczIntersectionJunk or
            RoomType.HczIntersection or
            RoomType.HczStraightC or
            RoomType.HczStraightPipeRoom or
            RoomType.HczStraightVariant or
            RoomType.Hcz127 or
            RoomType.HczServerRoom =>
                ZoneType.HeavyContainment,

            RoomType.EzVent or
            RoomType.EzIntercom or
            RoomType.EzGateA or
            RoomType.EzDownstairsPcs or
            RoomType.EzCurve or
            RoomType.EzPcs or
            RoomType.EzCrossing or
            RoomType.EzCollapsedTunnel or
            RoomType.EzConference or
            RoomType.EzChef or
            RoomType.EzStraight or
            RoomType.EzStraightColumn or
            RoomType.EzCafeteria or
            RoomType.EzUpstairsPcs or
            RoomType.EzGateB or
            RoomType.EzShelter or
            RoomType.EzTCross or
            RoomType.EzCheckpointHallwayA or
            RoomType.EzCheckpointHallwayB or
            RoomType.EzSmallrooms =>
                ZoneType.Entrance,

            RoomType.Surface =>
                ZoneType.Surface,

            RoomType.Pocket =>
                ZoneType.Pocket,

            _ => ZoneType.Other,
        };
    }
    public static int GetRoomIndex(this Exiled.API.Features.Room room)
    {
        List<Exiled.API.Features.Room> list = Exiled.API.Features.Room.List.Where(x => x != null && x.RoomShape == room.RoomShape && x.Zone == GetZone(room.Type) && x.Type == room.Type).ToList();

        int index = list.IndexOf(room);
        return index;
    }

    public static Vector3 GetAbsolutePosition(this Room? room, Vector3 position)
	{
		if (room is null || room.Type == RoomType.Surface)
			return position;

		return room.Transform.TransformPoint(position);
	}

	public static Quaternion GetAbsoluteRotation(this Room? room, Vector3 eulerAngles)
	{
		if (room is null || room.Type == RoomType.Surface)
			return Quaternion.Euler(eulerAngles);

		return room.Transform.rotation * Quaternion.Euler(eulerAngles);
	}
}
