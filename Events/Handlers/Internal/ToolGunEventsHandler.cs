using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using MEC;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Objects;
using ProjectMER.Features.ToolGun;
using UnityEngine;

namespace ProjectMER.Events.Handlers.Internal;

public class ToolGunEventsHandler : CustomEventsHandler
{
	public override void OnServerRoundStarted()
	{
		Timing.RunCoroutine(ToolGunGUI());
	}
    public static Hint ShowMeowHint(Player ply, float duration, string text, HintVerticalAlign Verticalalign = HintVerticalAlign.Top, int ycoordinate = 725, int xcoordinate = 0, HintAlignment aligmenthint = HintAlignment.Center, int fontsize = 27, int degradation = 30, string ID = "s")
    {
        text += "</s><color=#FFFFFF></b></u></i>";
        var hint = new HintServiceMeow.Core.Models.Hints.Hint()
        {
            Text = text,
            YCoordinateAlign = Verticalalign,
            YCoordinate = ycoordinate,
            XCoordinate = xcoordinate,
            Alignment = aligmenthint,
            FontSize = fontsize,
        };

        var playerDisplay = PlayerDisplay.Get(ply);
        if (ID != "s")
        {
            var oldhint = playerDisplay.GetHint(ID);
            if (oldhint != null)
            {
                playerDisplay.RemoveHint(oldhint);
                playerDisplay.ForceUpdate();
            }
            hint.Id = ID;
        }
        playerDisplay.AddHint(hint);
        Timing.CallDelayed(duration, () =>
        {
            playerDisplay.RemoveHint(hint);
            playerDisplay.ForceUpdate();
        });
        return hint;
    }
    private static IEnumerator<float> ToolGunGUI()
	{
		while (true)
		{
			yield return Timing.WaitForSeconds(0.1f);

			foreach (Player player in Player.List)
			{
				if (!player.CurrentItem.IsToolGun(out ToolGunItem _) && !ToolGunHandler.TryGetSelectedMapObject(player, out MapEditorObject _))
					continue;

				string hud;
				try
				{
					hud = ToolGunUI.GetHintHUD(player);
				}
				catch (Exception e)
				{
					Logger.Error(e);
					hud = "ERROR: Check server console";
				}

                ShowMeowHint(player, 0.1f, hud);
			}
		}
	}

	public override void OnPlayerDryFiringWeapon(PlayerDryFiringWeaponEventArgs ev)
	{
		if (!ev.Weapon.IsToolGun(out ToolGunItem toolGun))
			return;

		ev.IsAllowed = false;
		toolGun.Shot(ev.Player);
	}

	public override void OnPlayerReloadingWeapon(PlayerReloadingWeaponEventArgs ev)
	{
		if (!ev.Weapon.IsToolGun(out ToolGunItem toolGun))
			return;

		ev.IsAllowed = false;
		toolGun.SelectedObjectToSpawn--;
	}

	public override void OnPlayerDroppingItem(PlayerDroppingItemEventArgs ev)
	{
		if (!ev.Item.IsToolGun(out ToolGunItem toolGun))
			return;

		ev.IsAllowed = false;
		toolGun.SelectedObjectToSpawn++;
	}
}
