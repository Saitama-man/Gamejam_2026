using UnityEngine;

public class ObservatoryStateController : MonoBehaviour
{
	[Header("Before Cutscene")]
	[SerializeField] private GameObject beforeCutsceneRoot;

	[Header("After Cutscene")]
	[SerializeField] private GameObject afterCutsceneRoot;

	[Header("Story Flag")]
	[SerializeField] private string cutscenePlayedFlag = "ObservatoryCutscenePlayed";

	private void Start()
	{
		bool cutscenePlayed = StoryFlags.HasFlag(cutscenePlayedFlag);

		if (beforeCutsceneRoot != null)
		{
			beforeCutsceneRoot.SetActive(!cutscenePlayed);
		}

		if (afterCutsceneRoot != null)
		{
			afterCutsceneRoot.SetActive(cutscenePlayed);
		}
	}
}