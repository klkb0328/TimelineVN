using TimelineVN.Timeline.Editor;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

namespace TimelineVN.Choice.Editor
{
	/// <summary>
	/// 선택지 클립을 골랐을 때 Inspector 에 뜨는 화면.
	/// 문구를 적고 분기 추가를 누르면 분기 클립이 생기고 연결까지 끝난다
	/// </summary>
	[CustomEditor(typeof(ChoiceShowClip))]
	public class ChoiceShowClipInspector : UnityEditor.Editor
	{
		/// <summary>
		/// 인스펙터에 뜰 것들을 위에서부터 그린다
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			// 번호와 복귀 지점은 시스템이 정한 값이라 보여주기만 한다
			using (new EditorGUI.DisabledScope(true))
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty(ChoiceShowClip.ChoiceIdFieldName));
			}

			EditorGUILayout.PropertyField(serializedObject.FindProperty(ChoiceShowClip.OptionsFieldName), true);

			using (new EditorGUI.DisabledScope(true))
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty(ChoiceShowClip.DefaultReturnFieldName));
			}

			serializedObject.ApplyModifiedProperties();

			EditorGUILayout.Space();
			// 분기 추가 "버튼" 그리기
			DrawAddBranchButton();
		}

		/// <summary>
		/// 맨 아래 분기 추가 버튼. 누르면 분기 한 벌이 생기고 연결까지 끝난다
		/// </summary>
		private void DrawAddBranchButton()
		{
			var showClip = target as ChoiceShowClip;
			if (showClip == null)
			{
				return;
			}

			// 클립 에셋은 자기가 어느 트랙에 얹혀 있는지 모른다. 지금 열린 타임라인을 훑어서 찾는다
			TimelineAsset timeline = TimelineEditor.inspectedAsset;
			TimelineClip clip = TimelineClipFinder.FindClipOf(timeline, showClip);
			TrackAsset track = clip == null ? null : clip.GetParentTrack();

			// 트랙을 못 찾으면 분기를 놓을 자리가 없다. 버튼을 회색으로 만들어 못 누르게 한다
			using (new EditorGUI.DisabledScope(track == null))
			{
				if (!GUILayout.Button("분기 추가"))
				{
					return;
				}
				
				// 이게 실제 분기 추가하는거임
				ChoiceClipHelper.AddBranch(track, showClip);

				// 클립이 늘었으니 Timeline 창을 다시 그리게 한다
				TimelineEditor.Refresh(RefreshReason.ContentsAddedOrRemoved);
			}
		}
	}
}
