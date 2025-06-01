global using Logger = LabApi.Features.Console.Logger;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using HarmonyLib;
using LabApi.Events.CustomHandlers;
using LabApi.Loader.Features.Paths;
using LabApi.Loader.Features.Plugins;
using ProjectMER.Configs;
using ProjectMER.Events.Handlers.Internal;
using System.Drawing.Drawing2D;

namespace ProjectMER;

public class ProjectMER : Exiled.API.Features.Plugin<Config>
{
	 public static ProjectMER Singleton { get; private set; }
         public static string PluginDir { get; private set; }
	 public static string MapsDir { get; private set; }
	 public static string SchematicsDir { get; private set; }

	 public GenericEventsHandler GenericEventsHandler { get; } = new();

	 public ToolGunEventsHandler ToolGunEventsHandler { get; } = new();

	 public MapOnEventHandlers MapOnEventHandlers { get; } = new();

	 public PickupEventsHandler PickupEventsHandler { get; } = new();

         public InvisibleTeleportEventsHandler TeleportEventsHandler { get; } = new();

         public static Harmony? harmony;
         public const string HarmonyId = "PMER-Harmony";

         public override void OnEnabled()
	{
		Singleton = this;

		PluginDir = Path.Combine(Paths.Configs, Singleton.Config.FolderName);
		MapsDir = Path.Combine(PluginDir, "Maps");
		SchematicsDir = Path.Combine(PluginDir, "Schematics");

		if (!Directory.Exists(PluginDir))
		{
			Logger.Warn("Plugin directory does not exist. Creating...");
			Directory.CreateDirectory(PluginDir);
		}
		
		if (!Directory.Exists(MapsDir))
		{
			Logger.Warn("Maps directory does not exist. Creating...");
			Directory.CreateDirectory(MapsDir);
		}

		if (!Directory.Exists(SchematicsDir))
		{
			Logger.Warn("Schematics directory does not exist. Creating...");
			Directory.CreateDirectory(SchematicsDir);
		}

		Exiled.Events.Handlers.Player.Verified += OnVerified;
		
		CustomHandlersManager.RegisterEventsHandler(GenericEventsHandler);
		CustomHandlersManager.RegisterEventsHandler(ToolGunEventsHandler);
		CustomHandlersManager.RegisterEventsHandler(MapOnEventHandlers);
		CustomHandlersManager.RegisterEventsHandler(PickupEventsHandler);
                CustomHandlersManager.RegisterEventsHandler(TeleportEventsHandler);


                harmony = new Harmony(HarmonyId);
		harmony.PatchAll();
	}

	public override void OnDisabled()
	{
		Singleton = null!;
		
		CustomHandlersManager.UnregisterEventsHandler(GenericEventsHandler);
		CustomHandlersManager.UnregisterEventsHandler(ToolGunEventsHandler);
		CustomHandlersManager.UnregisterEventsHandler(MapOnEventHandlers);
		CustomHandlersManager.UnregisterEventsHandler(PickupEventsHandler);
                CustomHandlersManager.UnregisterEventsHandler(TeleportEventsHandler);

                Exiled.Events.Handlers.Player.Verified -= OnVerified;

                harmony.UnpatchAll();
                harmony = null;
	}

	public override string Name => "ProjectMER";

	public override string Author => "Michal78900 (Exiled Port by ScoutTrooper)";

	public override Version Version => new Version(2025, 6, 1, 13);

	void OnVerified(VerifiedEventArgs ev) => ev.Player.SendConsoleMessage("This server using ProjectMER EXILED port. Author of port - ScoutTrooper. Original Plugin - Michal78900", "red");
}
