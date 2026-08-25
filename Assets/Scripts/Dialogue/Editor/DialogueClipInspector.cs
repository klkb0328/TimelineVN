using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace TimelineVN.Dialogue.Editor
{
	/// <summary>
	/// 대사 클립을 골랐을 때 Inspector 에 뜨는 화면.
	/// 대사 아래에 글자 수랑 타이핑에 걸리는 시간을 같이 보여준다.
	/// 대사를 쓰는 자리에서 바로 보여야 클립 길이를 손볼지 판단할 수 있음
	/// </summary>
	[CustomEditor(typeof(DialogueClip))]
	[CanEditMultipleObjects]
	public class DialogueClipInspector : UnityEditor.Editor
	{
		/// <summary>
		/// 기본 필드를 그리고 그 아래에 타이핑 정보를 붙인다
		/// </summary>
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			// 여러 개를 골랐으면 어느 클립 얘긴지 못 정해서 타이핑 정보는 건너뛴다.
			// 속도를 한꺼번에 바꾸려고 여러 개 고르는 경우라 이게 자연스럽다
			if (targets.Length > 1)
			{
				return;
			}

			var dialogueClip = target as DialogueClip;
			if (dialogueClip == null || !dialogueClip.Line.HasText)
			{
				return;
			}

			EditorGUILayout.Space();
			DrawTypingInfo(dialogueClip);
		}

		/// <summary>
		/// 글자 수와 타이핑에 걸리는 시간을 그린다
		/// </summary>
		private static void DrawTypingInfo(DialogueClip dialogueClip)
		{
			int characterCount = dialogueClip.Line.CharacterCount;
			TimelineClip clip = TimelineEditor.selectedClip;

			// 클립을 못 잡으면 길이를 몰라서 시간 계산이 안 된다. 글자 수만이라도 보여준다
			if (clip == null || clip.asset != dialogueClip)
			{
				EditorGUILayout.LabelField($"{characterCount}자");

				return;
			}

			TypingData typing = DialogueClipEditor.BuildTypingData(clip, dialogueClip);
			string summary = $"{characterCount}자, 타이핑 {typing.Duration:0.##}초 / 클립 {clip.duration:0.##}초";

			if (typing.IsAccelerated)
			{
				EditorGUILayout.HelpBox($"{summary}\n클립이 짧아서 타이핑이 빨라진다. 클립을 늘리면 원래 속도로 찍힌다", MessageType.Warning);

				return;
			}

			EditorGUILayout.LabelField(summary);
		}
	}
}
