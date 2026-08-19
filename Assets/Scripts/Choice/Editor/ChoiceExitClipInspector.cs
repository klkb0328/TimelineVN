using System.Collections.Generic;
using TimelineVN.Timeline;
using TimelineVN.Timeline.Editor;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace TimelineVN.Choice.Editor
{
	/// <summary>
	/// 분기 끝 클립을 골랐을 때 Inspector 에 뜨는 화면. 돌아갈 자리를 여기서 바꾼다.
	/// 오브젝트 칸에 끌어다 놓는 방식이 안 되더라.. 그래서 직접 선택해야함.
	/// </summary>
	[CustomEditor(typeof(ChoiceExitClip))]
	public class ChoiceExitClipInspector : UnityEditor.Editor
	{
		/// <summary>
		/// 아무 데도 안 가리키는 상태를 나타내는 항목
		/// </summary>
		private const string NoneLabel = "(연결 없음)";

		/// <summary>
		/// 돌아갈 자리를 고르는 목록을 그린다
		/// </summary>
		public override void OnInspectorGUI()
		{
			var exit = target as ChoiceExitClip;
			if (exit == null)
			{
				return;
			}

			TimelineAsset timeline = TimelineEditor.inspectedAsset;
			List<JumpTarget> candidates = CollectReturnTargets(timeline);

			int currentIndex = candidates.IndexOf(exit.Destination) + 1;
			int selectedIndex = EditorGUILayout.Popup("복귀 지점", currentIndex, BuildLabels(timeline, candidates));

			if (selectedIndex == currentIndex)
			{
				return;
			}

			// Ctrl+Z 로 되돌릴 수 있게 지금 상태를 Unity 에 등록해둔다. 이런 게 있길래 넣어봤다.
			// 바꾸기 직전에 불러야 바뀌기 전 값이 기록된다
			// TODO : 이거 나중에 클립같은데에도 쓸수있는지 봐야할듯?
			Undo.RecordObject(exit, "Set Return Target");
			exit.SetDestination(selectedIndex == 0 ? null : candidates[selectedIndex - 1]);
			EditorUtility.SetDirty(exit);

			RefreshClipName(timeline, exit);
		}

		/// <summary>
		/// 돌아갈 자리가 될 수 있는 클립들을 시간순으로 모은다.
		/// 분기 시작은 뺀다. 거기로 돌아가면 방금 재생한 분기를 다시 재생하게 된다
		/// 이렇게 하는 이유는 맨위 주석처럼 이게 인스펙터로 못끌어와서 이렇게하는거임
		/// </summary>
		private static List<JumpTarget> CollectReturnTargets(TimelineAsset timeline)
		{
			var targets = new List<JumpTarget>();

			if (timeline == null)
			{
				return targets;
			}

			var found = new List<TimelineClip>();

			// 처음엔 선택지 트랙만 하려다가 앞으로 어떻게 될지 모르기도하고, 우선 MainEndClip도 Jumptarget이니까 전부 긁기로..
			foreach (TrackAsset track in timeline.GetOutputTracks())
			{
				foreach (TimelineClip clip in track.GetClips())
				{
					if (clip.asset is ChoiceReturnClip || clip.asset is MainEndClip)
					{
						found.Add(clip);
					}
				}
			}

			// 순서 빠른순으로 한다.
			found.Sort((left, right) => left.start.CompareTo(right.start));

			foreach (TimelineClip clip in found)
			{
				targets.Add(clip.asset as JumpTarget);
			}

			return targets;
		}

		/// <summary>
		/// 목록에 뜰 문구들. 맨 앞은 연결을 끊는 항목이다
		/// </summary>
		private static string[] BuildLabels(TimelineAsset timeline, List<JumpTarget> candidates)
		{
			var labels = new string[candidates.Count + 1];
			labels[0] = NoneLabel;

			for (int i = 0; i < candidates.Count; i++)
			{
				labels[i + 1] = ChoiceClipNaming.DescribeJumpTarget(timeline, candidates[i]);
			}

			return labels;
		}

		/// <summary>
		/// 클립 이름에도 어디로 가는지가 들어가서 같이 갱신한다
		/// </summary>
		private static void RefreshClipName(TimelineAsset timeline, ChoiceExitClip exit)
		{
			TimelineClip clip = TimelineClipFinder.FindClipOf(timeline, exit);
			if (clip == null)
			{
				return;
			}

			clip.displayName = ChoiceClipNaming.BuildExitName(timeline, exit);
			TimelineEditor.Refresh(RefreshReason.ContentsModified);
		}
	}
}
