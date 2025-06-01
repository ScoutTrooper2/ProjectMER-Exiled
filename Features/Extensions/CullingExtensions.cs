namespace ProjectMER.Features.Extensions;

using System.Reflection;
using LabApi.Features.Wrappers;
using Mirror;
using Objects;

/// <summary>
/// A set of useful extensions to easily interact with culling features.
/// </summary>
public static class CullingExtensions
{
	private static MethodInfo? _sendSpawnMessage;
	private static MethodInfo SendSpawnMessage => _sendSpawnMessage ??= typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.NonPublic | BindingFlags.Static);
	public static void SpawnSchematic(this Player player, SchematicObject schematic)
	{
		foreach (NetworkIdentity networkIdentity in schematic.NetworkIdentities)
			player.SpawnNetworkIdentity(networkIdentity);
	}
	public static void DestroySchematic(this Player player, SchematicObject schematic)
	{
		foreach (NetworkIdentity networkIdentity in schematic.NetworkIdentities)
			player.DestroyNetworkIdentity(networkIdentity);
	}
	public static void SpawnNetworkIdentity(this Player player, NetworkIdentity networkIdentity) =>
		SendSpawnMessage.Invoke(null, [networkIdentity, player.Connection]);
	public static void DestroyNetworkIdentity(this Player player, NetworkIdentity networkIdentity) =>
		player.Connection.Send(new ObjectDestroyMessage { netId = networkIdentity.netId });
}