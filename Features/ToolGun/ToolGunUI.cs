using System.Reflection;
using System.Text;
using LabApi.Features.Wrappers;
using MapGeneration;
using NorthwoodLib.Pools;
using ProjectMER.Features.Enums;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Objects;
using UnityEngine;
using UserSettings.ServerSpecific;
using YamlDotNet.Serialization;
using Room = Exiled.API.Features.Room;

namespace ProjectMER.Features.ToolGun;

public static class ToolGunUI
{
    private static string GetZoneColor(FacilityZone zone)
    {
        return zone switch
        {
            FacilityZone.HeavyContainment => "#FF0000",    // HCZ => Красный
            FacilityZone.Entrance => "#00FF00",          // EZ => Зелёный
            FacilityZone.LightContainment => "#00FFFF",   // LCZ => Голубой
            FacilityZone.Surface => "#0000FF",            // SZ => Синий
            _ => "#FFFFFF"                           // По умолчанию белый для неизвестных зон
        };
    }
    public static string GetHintHUD(Player player)
    {
        StringBuilder sb = StringBuilderPool.Shared.Rent();
        sb.Append("<font=\"LiberationSans SDF\">");

        const string sizeTag = "<size=49%>";
        const string lineHeight = "<br><line-height=0.54em>";
        int offset = 0;
        if (player.CurrentItem.IsToolGun(out ToolGunItem toolGun) && toolGun != null)
        {
            string zoneDisplay = player.Room != null
                ? $"<color={GetZoneColor(player.Room.Zone)}>{player.Room.Zone}</color>"
                : "За пределами карты";
            sb.Append(sizeTag).Append(zoneDisplay).Append("</size>").Append(lineHeight);
            offset++;

            sb.Append(sizeTag).Append($"{player.Position:F3}").Append("</size>").Append(lineHeight);
            offset++;

            sb.Append(sizeTag).Append(GetToolGunModeString(player, toolGun)).Append("</size>").Append(lineHeight);
            offset++;
        }



        if (ToolGunHandler.TryGetSelectedMapObject(player, out MapEditorObject mapEditorObject) && mapEditorObject != null)
        {
            FieldInfo baseField = mapEditorObject.GetType().GetField("Base", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (baseField != null)
            {
                object? instance = baseField.GetValue(mapEditorObject);
                if (instance != null)
                {
                    var properties = instance.GetType().GetProperties()
                        .Where(p =>
                            !Attribute.IsDefined(p, typeof(YamlIgnoreAttribute)))
                        .ToList();

                    foreach (string property in properties.GetColoredProperties(instance))
                    {
                        sb.Append($"<size=50%>");
                        sb.Append(property);
                        sb.Append("</size>");
                        sb.AppendLine();
                    }

                    if (mapEditorObject != null)
                    {
                        sb.AppendLine();
                        sb.AppendLine($"<size=49%>MapName: {MapUtils.GetColoredMapName(mapEditorObject.MapName)}</size>");
                    }

                    foreach (string property in properties.GetColoredProperties(instance))
                    {
                        sb.Append(sizeTag).Append(property).Append("</size>").Append(lineHeight);
                        offset++;
                    }
                }
            }
        }

        if (offset > 10 && offset != 3)
        {
            for (int i = offset; i < 39; i++)
            {
                sb.AppendLine();
            }
        }
        else
        {
            for (int i = offset; i < 36; i++)
            {
                sb.AppendLine();
            }
        }

        sb.Append("</font>");
        return StringBuilderPool.Shared.ToStringReturn(sb);

    }

    private static string GetToolGunModeString(Player player, ToolGunItem toolGun)
	{
		if (toolGun.CreateMode)
		{
			string output;
            output = toolGun.SelectedObjectToSpawn.ToString().ToUpper();

            return $"<color=green>CREATE</color>\n<color=yellow>{output}</color>";
		}

		string name = " ";
		if (ToolGunHandler.Raycast(player, out RaycastHit hit))
		{
			if (hit.transform.TryGetComponentInParent(out MapEditorObject mapEditorObject))
			{
				if (mapEditorObject is IndicatorObject indicatorObject)
					mapEditorObject = IndicatorObject.Dictionary[indicatorObject];

				if (mapEditorObject.gameObject.TryGetComponent(out SchematicObject schematicObject))
				{
					name = schematicObject.Name.ToUpper();
				}
				else
				{
					name = mapEditorObject.Base.ToString().Split('.').Last().Replace("Serializable", "").ToUpper();
				}
			}
		}

		if (toolGun.DeleteMode)
			return $"<color=red>DELETE</color>\n<color=yellow>{name}</color>";

		if (toolGun.SelectMode)
			return $"<color=yellow>SELECT</color>\n<color=yellow>{name}</color>";

		return "\n ";
	}

	private static string GetRoomString(Player player)
	{
		if (!player.Camera.transform.position.TryGetRoom(out RoomIdentifier? roomIdentity))
			return "Unknown";

		Room room = Room.Get(roomIdentity);

		List<Room> list = ListPool<Room>.Shared.Rent(Room.List.Where(x => x != null && x.Zone == room.Zone && x.RoomShape == room.RoomShape && x.Name == room.Name));

		string roomString;
		if (list.Count == 1)
		{
			roomString = room.GetRoomStringId();
		}
		else
		{
			roomString = $"{room.GetRoomStringId()} (ID: {list.IndexOf(room)}) (OF: {list.Count})";
		}

		ListPool<Room>.Shared.Return(list);
		return roomString;
	}
}
