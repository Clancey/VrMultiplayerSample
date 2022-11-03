using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BuildManagerWindow : EditorWindow
{
	static BuildManagerWindow ()
	{
		Directory.CreateDirectory(DefaultPath);
	}
	Settings settings = new Settings();

	public static List<string> Platforms = Settings.TargetMappings.Keys.ToList();

	public static List<BuildTarget> Targets = Enum.GetValues(typeof(BuildTarget)).OfType<BuildTarget>().ToList();

	[SerializeField] TreeViewState _sceneSelectionState;


	int _platformIndex = 0;
	int platformIndex
	{
		get => _platformIndex;
		set
		{
			_platformIndex = value;
			settings.Platform = value >= 0 ? Platforms[value] : "";
		}
	}

	static string BaseDir = Directory.GetCurrentDirectory();
	static string AssetsPath = Path.Combine(BaseDir, "Assets");
	static string DefaultPath = Path.Combine(BaseDir, ".build");
	static string LibraryPath = Path.Combine(BaseDir, "Library");
	static string DefaultSettings = Path.Combine(DefaultPath, "lastBuild");

	public void LoadDefault ()
	{
		settings = LoadSettings();
		foundScenes = Directory.GetFiles(AssetsPath, "*.unity", SearchOption.AllDirectories).Select(x => x.Replace(BaseDir, "").TrimStart(Path.DirectorySeparatorChar)).ToList();
		UpdateFoundScenes();

	}
	string currentFoundScenes = "";
	void UpdateFoundScenes ()
	{
		if (currentFoundScenes == settings.FileName)
			return;
		//Debug.LogError(settings.FileName);
		var sceneSettings = LoadSettings(settings.FileName);
		currentFoundScenes = settings.FileName;
		sceneIncluded = foundScenes.Select(x => (x, sceneSettings.Levels.Contains(x))).OrderBy(x => x.Item2).ToDictionary(x => x.x, x => x.Item2);
	}
	public void SaveDefaults (bool saveLevels)
	{
		if (saveLevels)
		{
			var levels = sceneIncluded.Where(x => x.Value).Select(x => x.Key).ToList();
			settings.Levels = levels;
		}

		var json = JsonUtility.ToJson(settings);
		//Debug.LogError(json);
		File.WriteAllText(CurrentLibrarySettingPath, json);
		File.WriteAllText(Path.Combine(DefaultPath, settings.FileName), json);
		File.WriteAllText(DefaultSettings, settings.FileName);
	}

	[Serializable]
	public class Settings
	{
		[SerializeField]
		bool isVr;
		[SerializeField]
		private string platform;

		public bool IsVr
		{
			get => isVr;
			set
			{
				isVr = value;
				if (!isVr)
					Platform = "";

			}
		}
		public static Dictionary<string, BuildTarget> TargetMappings = new Dictionary<string, BuildTarget>
		{
			["Steam"] = BuildTarget.StandaloneWindows64,
			["Oculus Desktop"] = BuildTarget.StandaloneWindows64,
			["Oculus Quest"] = BuildTarget.Android,
		};


		[SerializeField]
		private BuildTarget target;

		[SerializeField]
		private List<string> levels = new List<string>();

		[SerializeField]
		private string buildOutput;

		public string Platform
		{
			get => platform;
			set
			{
				platform = value;
				if (TargetMappings.TryGetValue(value, out var target))
					Target = target;
			}
		}
		public BuildTarget Target { get => target; set => target = value; }

		public List<string> Levels { get => levels; set => levels = value; }
		public string FileName => string.IsNullOrWhiteSpace(Platform) ? $"{Target}" : $"{Target}-{Platform}";

		public string BuildOutput { get => buildOutput; set => buildOutput = value; }

		public string VrSdk => (platform?.Contains("Oculus") ?? false) ? "Oculus" : "Open VR";

		[SerializeField]
		string customDefines;
		public string DefineStatements
		{
			get
			{
				var defines = new List<string>
				{
					IsVr ? "VR" : "NONVR",
				};
				if (IsVr)
				{
					defines.Add("VIU_PLUGIN");
					if (platform == "Steam")
						defines.Add("VIU_OPENVR_SUPPORT");
					else if (platform == "Oculus Desktop")
						defines.Add("VIU_OCULUSVR_DESKTOP_SUPPORT");
				}
				return String.Join("; ", defines);
			}
		}

	}

	[MenuItem("Tools/CustomBuild")]
	public static void Init ()
	{
		var window = GetWindow<BuildManagerWindow>();
		window.LoadDefault();
		window.Show();

	}

	bool didLoadScenes = false;
	string errorMessage;
	int targetIndex;
	private void OnInspectorUpdate ()
	{

	}

	List<string> foundScenes = new List<string>();
	Dictionary<string, bool> sceneIncluded = new Dictionary<string, bool>();
	bool isComplete;
	string outputPath
	{
		get
		{
			if (!string.IsNullOrWhiteSpace(settings?.BuildOutput))
				return settings.BuildOutput;
			return Path.Combine(BaseDir, "build", settings.FileName);
		}
	}
	private void OnGUI ()
	{
		GUILayout.Label("Custom Build Settings", EditorStyles.boldLabel);
		settings.IsVr = EditorGUILayout.Toggle("Is VR", settings.IsVr);
		GUI.enabled = settings.IsVr;

		platformIndex = Platforms.IndexOf(settings.Platform);
		platformIndex = EditorGUILayout.Popup(platformIndex, Platforms.ToArray());
		GUILayout.Label(settings.Platform, EditorStyles.label);
		GUI.enabled = !settings.IsVr;
		targetIndex = Targets.IndexOf(settings.Target);
		targetIndex = EditorGUILayout.Popup(targetIndex, Targets.Select(x => x.ToString()).ToArray());
		isComplete = (settings.IsVr && platformIndex >= 0) || (!settings.IsVr && targetIndex >= 0);

		GUI.enabled = isComplete;
		if (isComplete)
		{
			UpdateFoundScenes();
		}
		//EditorGUILayout.BeginScrollView(Vector2.zero);
		var data = sceneIncluded.ToList();
		foreach (var item in data)
		{
			EditorGUILayout.BeginHorizontal();
			sceneIncluded[item.Key] = EditorGUILayout.Toggle("", item.Value, GUILayout.MaxWidth(40));
			GUILayout.Label(item.Key, EditorStyles.label);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}

		//EditorGUILayout.EndScrollView();

		GUI.enabled = isComplete;

		GUILayout.Label("Output Path:", EditorStyles.boldLabel);
		GUILayout.Label(outputPath);
		if (GUILayout.Button("Set Output"))
			settings.BuildOutput = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");


		string error = "";
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Save"))
		{
			SaveDefaults(true);
		}
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Apply Settings"))
		{

			var levels = sceneIncluded.Where(x => x.Value).Select(x => x.Key).ToList();
			settings.Levels = levels;

			var setupResult = SetupLib();
			if (setupResult.success)
			{
				SaveDefaults(false);
			}
			else
				error = setupResult.error;

		}
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Build"))
		{
			var result = Build();
			EditorUtility.DisplayDialog($"Build Complete: {result.success}", result.error, "Ok");
			error = result.error;
		}
		GUILayout.FlexibleSpace();

		if (GUILayout.Button("Build and Run"))
		{
			var result = Build(true);
			if (!result.success)
				EditorUtility.DisplayDialog($"Build Complete: {result.success}", result.error, "Ok");
			error = result.error;
		}

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		GUILayout.Label(error);
		GUILayout.Space(20);
	}

	(bool success, string error) Build (bool run = false)
	{
		var levels = sceneIncluded.Where(x => x.Value).Select(x => x.Key).ToList();
		settings.Levels = levels;

		var setupResult = SetupLib();
		if (!setupResult.success)
			return setupResult;

		SaveDefaults(false);

		var extension = ".zip";
		if (settings.Target == BuildTarget.Android)
			extension = ".apk";
		else if (settings.Target == BuildTarget.StandaloneWindows || settings.Target == BuildTarget.StandaloneWindows64)
		{
			extension = ".exe";
		}
		var output = Path.Combine(outputPath, $"{PlayerSettings.productName}{extension}");

		var result = BuildPipeline.BuildPlayer(levels.ToArray(), output, settings.Target, BuildOptions.None);
		var failure = result.steps.SelectMany(x => x.messages.Where(y => y.type == LogType.Error || y.type == LogType.Exception)).FirstOrDefault();
		if (!string.IsNullOrWhiteSpace(failure.content))
		{
			return (false, failure.content);
		}
		//if(result.steps[0].)
		//if(result.)
		if (run)
		{
			var proc = new Process();
			proc.StartInfo.FileName = output;
			proc.Start();
		}
		return (true, output);

	}
	static Settings LoadSettings (string fileName = null)
	{
		try
		{
			if (fileName == null)
			{
				fileName = File.ReadAllText(DefaultSettings);
			}
			var path = Path.Combine(DefaultPath, fileName);
			//Debug.LogError(path);
			var json = File.ReadAllText(path);
			return JsonUtility.FromJson<Settings>(json);
		}
		catch
		{
			return new Settings
			{
				Target = EditorUserBuildSettings.activeBuildTarget
			};
		}
	}
	static string CurrentLibrarySettingPath = Path.Combine(LibraryPath, "buildSettings.json");
	(bool success, string error) SetupLib ()
	{

		if (settings.Target == 0)
		{
			return (false, "You didn't select a valid Target Platform!");
		}

		var targetPlatform = settings.FileName;

		//Debug.Log("current platform: " + currentPlatform);
		//Debug.Log("next platform: " + buildTarget);

		//if (currentSettings.FileName == settings.FileName)
		//{
		//	return (true, $"You selected the current platform as the Target Platform!: {currentSettings.FileName}  - {settings.FileName}");
		//}

		// Don't switch when compiling
		if (EditorApplication.isCompiling)
		{
			return (false, "Could not build platform because Unity is compiling!");
		}

		// Don't switch while playing
		if (EditorApplication.isPlayingOrWillChangePlaymode)
		{
			return (false, "Could not build platform because Unity is in Play Mode!");
		}

		var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(settings.Target);
		EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, settings.Target);

		PlayerSettings.virtualRealitySupported = settings.IsVr;
		//if (settings.IsVr)
		//{
		//	PlayerSettings.SetVirtualRealitySDKs(buildTargetGroup, new[] { settings.VrSdk });
		//}
		Debug.Log($"Setting defines: {settings.DefineStatements}");
		PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, settings.DefineStatements);

		Debug.Log("Platform switched to " + targetPlatform);


		var json = JsonUtility.ToJson(settings);
		File.WriteAllText(CurrentLibrarySettingPath, json);
		return (true, "");
	}

}
