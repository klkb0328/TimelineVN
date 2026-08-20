using System.Collections.Generic;
using TimelineVN.Timeline.Editor;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineVN.Choice.Editor
{
	/// <summary>
	/// 이 타임라인의 분기를 목록으로 띄우고, 누르면 그 자리로 데려간다.
	/// 분기 구간이 메인 뒤 시간축에 흩어져 있어서 스크롤로는 찾아가기 어려운것 해소용이고
	/// Timeline 창 옆에 탭으로 붙여두고 쓰는 것을 추천함
	/// </summary>
	public class ChoiceBranchNavigatorWindow : EditorWindow
	{
		/// <summary>
		/// 창 제목이자 메뉴에 뜰 이름
		/// </summary>
		private const string WindowTitle = "분기 내비게이터";

		/// <summary>
		/// 타임라인이 안 열려 있을 때 대신 띄울 안내
		/// </summary>
		private const string NoTimelineMessage = "Timeline 창에 타임라인을 열어 주세요";

		/// <summary>
		/// 분기가 하나도 없을 때 대신 띄울 안내
		/// </summary>
		private const string NoBranchMessage = "이 타임라인에는 선택지가 없습니다";

		/// <summary>
		/// Timeline 창에 화면을 맞추라고 보낼 커맨드.
		/// </summary>
		private const string FrameSelectedCommand = "FrameSelected";

		/// <summary>
		/// 그 자리로 데려가는 버튼에 적을 글자
		/// </summary>
		private const string MoveButtonLabel = "이동";

		/// <summary>
		/// 분기 색 사각형의 너비
		/// </summary>
		private const float ColorMarkWidth = 14f;

		/// <summary>
		/// 이동 버튼의 너비. 줄마다 같은 자리에 놓이게 고정한다
		/// </summary>
		private const float MoveButtonWidth = 46f;

		/// <summary>
		/// 분기 줄을 선택지 줄보다 안으로 들여쓸 폭
		/// </summary>
		private const float BranchIndent = 14f;

		/// <summary>
		/// 목록이 창보다 길 때의 스크롤 위치
		/// </summary>
		private Vector2 scrollPosition;

		/// <summary>
		/// 창을 연다
		/// </summary>
		[MenuItem("Window/TimelineVN/분기 내비게이터")]
		public static void Open()
		{
			GetWindow<ChoiceBranchNavigatorWindow>(WindowTitle);
		}

		/// <summary>
		/// 계속 다시 그린다. 색이나 클립 이름이 바뀌었다는 신호가 이 창에는 안 온다
		/// </summary>
		private void Update()
		{
			Repaint();
		}

		/// <summary>
		/// 선택지별로 묶어서 그 아래 분기들을 늘어놓는다
		/// </summary>
		private void OnGUI()
		{
			TimelineAsset timeline = TimelineEditor.inspectedAsset;
			if (timeline == null)
			{
				EditorGUILayout.HelpBox(NoTimelineMessage, MessageType.Info);

				return;
			}

			List<ChoiceShowClip> showClips = CollectShowClips(timeline);
			if (showClips.Count == 0)
			{
				EditorGUILayout.HelpBox(NoBranchMessage, MessageType.Info);

				return;
			}

			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

			foreach (ChoiceShowClip showClip in showClips)
			{
				DrawShowClip(timeline, showClip);
			}

			EditorGUILayout.EndScrollView();
		}

		/// <summary>
		/// 선택지 한 벌. 선택지 자신이 한 줄, 그 아래 분기가 한 줄씩이다
		/// </summary>
		private static void DrawShowClip(TimelineAsset timeline, ChoiceShowClip showClip)
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.LabelField(ChoiceClipNaming.BuildShowName(showClip), EditorStyles.boldLabel);
				DrawMoveButton(timeline, showClip);
			}

			foreach (ChoiceOption option in showClip.Options)
			{
				DrawOption(timeline, option);
			}

			EditorGUILayout.Space();
		}

		/// <summary>
		/// 분기 한 줄. 분기 색, 항목 문구와 목적지, 이동 버튼이 나란히 놓인다
		/// </summary>
		private static void DrawOption(TimelineAsset timeline, ChoiceOption option)
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.Space(BranchIndent);
				DrawColorMark(option.Entry);
				EditorGUILayout.LabelField(BuildOptionLabel(option));

				// 시작만 고르면 0.5초짜리 표식 클립에 화면이 맞춰져 크게 당겨진다.
				// 끝까지 함께 골라야 구간 전체가 들어오는 크기가 된다
				DrawMoveButton(timeline, option.Entry, option.HasEntry ? option.Entry.Exit : null);
			}
		}

		/// <summary>
		/// 분기 색 사각형. 색을 안 정한 분기는 자리만 비워둔다
		/// </summary>
		private static void DrawColorMark(ChoiceEntryClip entry)
		{
			Rect markRect = GUILayoutUtility.GetRect(ColorMarkWidth, EditorGUIUtility.singleLineHeight, GUILayout.Width(ColorMarkWidth));

			if (entry == null || !entry.HasBranchColor)
			{
				return;
			}

			// 목록에서는 글자를 덮지 않아서 클립 몸통처럼 옅게 깔 이유가 없다
			EditorGUI.DrawRect(markRect, entry.BranchColor);
		}

		/// <summary>
		/// 그 자리로 데려가는 버튼. 화면은 넘긴 클립들을 다 담는 크기로 맞춰진다.
		/// 하나도 못 찾으면 눌리지 않는다
		/// </summary>
		private static void DrawMoveButton(TimelineAsset timeline, PlayableAsset first, PlayableAsset second = null)
		{
			var clips = new List<TimelineClip>();
			AddClip(clips, timeline, first);
			AddClip(clips, timeline, second);

			using (new EditorGUI.DisabledScope(clips.Count == 0))
			{
				if (GUILayout.Button(MoveButtonLabel, GUILayout.Width(MoveButtonWidth)))
				{
					MoveTo(clips);
				}
			}
		}

		/// <summary>
		/// 이 에셋을 담고 있는 클립을 목록에 더한다. 없으면 아무것도 안 한다
		/// </summary>
		private static void AddClip(List<TimelineClip> clips, TimelineAsset timeline, PlayableAsset asset)
		{
			TimelineClip clip = TimelineClipFinder.FindClipOf(timeline, asset);

			if (clip != null)
			{
				clips.Add(clip);
			}
		}

		/// <summary>
		/// 그 클립들을 고른 상태로 만들고 재생 헤드를 옮긴 뒤, 화면을 맞추게 한다
		/// </summary>
		private static void MoveTo(List<TimelineClip> clips)
		{
			TimelineEditor.selectedClips = clips.ToArray();

			TimelineEditorWindow timelineWindow = TimelineEditor.GetWindow();
			if (timelineWindow == null)
			{
				return;
			}

			timelineWindow.playbackControls.SetCurrentTime(clips[0].start);

			// 그리는 도중에 다른 창으로 이벤트를 보내지 않는다. 화면을 맞추는 쪽이
			// 그 클립의 화면 사각형을 읽는데, 그 값은 Timeline 창이 그 줄을 한 번
			// 그린 뒤에야 생긴다
			EditorApplication.delayCall += () => timelineWindow.SendEvent(EditorGUIUtility.CommandEvent(FrameSelectedCommand));
		}

		/// <summary>
		/// 항목 문구와 그 분기가 끝나면 어디로 가는지
		/// </summary>
		/// <example>
		/// 따라간다  ->  장면 끝
		/// 모른척한다  ->  복귀
		/// 분기를 아직 안 만든 항목  ->  따라간다
		/// </example>
		private static string BuildOptionLabel(ChoiceOption option)
		{
			string text = ChoiceClipNaming.DescribeOption(option);

			if (!option.HasEntry || !option.Entry.HasExit)
			{
				return text;
			}

			return $"{text}  ->  {ChoiceClipNaming.DescribeTargetKind(option.Entry.Exit.Destination)}";
		}

		/// <summary>
		/// 타임라인에 놓여 있는 선택지 클립들
		/// </summary>
		private static List<ChoiceShowClip> CollectShowClips(TimelineAsset timeline)
		{
			var showClips = new List<ChoiceShowClip>();

			foreach (TrackAsset track in timeline.GetOutputTracks())
			{
				foreach (TimelineClip clip in track.GetClips())
				{
					var showClip = clip.asset as ChoiceShowClip;
					if (showClip != null)
					{
						showClips.Add(showClip);
					}
				}
			}

			return showClips;
		}
	}
}
