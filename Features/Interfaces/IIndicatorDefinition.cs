using LabApi.Features.Wrappers;
using UnityEngine;
using Room = Exiled.API.Features.Room;

namespace ProjectMER.Features.Interfaces;

public interface IIndicatorDefinition
{
	public GameObject SpawnOrUpdateIndicator(Room room, GameObject? instance = null);
}

