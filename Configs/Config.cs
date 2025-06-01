using Exiled.API.Interfaces;
using System.ComponentModel;

namespace ProjectMER.Configs;

public class Config : IConfig
{
	[Description("Whether the object will be auto selected when spawning it.")]
	public bool AutoSelect { get; set; } = true;

	[Description("Use load:map to load, use unload:map to unload.")]
	public List<string> OnWaitingForPlayers { get; set; } = new();
	public List<string> OnRoundStarted { get; set; } = new();
	public List<string> OnLczDecontaminationStarted { get; set; } = new();
	public List<string> OnWarheadStarted { get; set; } = new();
	public List<string> OnWarheadStopped { get; set; } = new();
	public List<string> OnWarheadDetonated { get; set; } = new();
	public bool IsEnabled { get; set; } = true;
	public bool Debug { get; set; } = false;
	public bool DisableItemDistributor { get; set; } = true;
}
