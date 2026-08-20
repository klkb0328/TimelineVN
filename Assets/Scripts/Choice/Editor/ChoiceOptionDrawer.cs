using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;

namespace TimelineVN.Choice.Editor
{
	/// <summary>
	/// 선택지 항목 한 줄을 그린다. 문구 옆에 그 항목이 데려갈 분기의 색을 놓아,
	/// 목록에서 바로 색을 바꿀 수 있게 한다
	/// </summary>
	[CustomPropertyDrawer(typeof(ChoiceOption))]
	public class ChoiceOptionDrawer : PropertyDrawer
	{
		/// <summary>
		/// 색 칸의 너비
		/// </summary>
		private const float ColorWidth = 56f;

		/// <summary>
		/// 문구와 색 칸 사이 간격
		/// </summary>
		private const float FieldGap = 4f;

		/// <summary>
		/// 항목 한 줄을 문구와 색으로 나눠 그린다
		/// </summary>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var textProperty = property.FindPropertyRelative(ChoiceOption.TextFieldName);
			var entryProperty = property.FindPropertyRelative(ChoiceOption.EntryFieldName);

			var textRect = new Rect(position.x, position.y, position.width - ColorWidth - FieldGap, position.height);
			var colorRect = new Rect(textRect.xMax + FieldGap, position.y, ColorWidth, position.height);

			EditorGUI.PropertyField(textRect, textProperty, label);
			DrawBranchColor(colorRect, entryProperty);
		}

		/// <summary>
		/// 항목을 한 줄에 담는다. 문구와 색뿐이라 접었다 펴게 할 이유가 없다
		/// </summary>
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		/// <summary>
		/// 이 항목이 데려갈 분기의 색을 그린다.
		/// </summary>
		private static void DrawBranchColor(Rect position, SerializedProperty entryProperty)
		{
			var entry = entryProperty.objectReferenceValue as ChoiceEntryClip;

			// 분기가 아직 없는 항목은 색을 담아둘 자리가 없다
			if (entry == null)
			{
				using (new EditorGUI.DisabledScope(true))
				{
					EditorGUI.ColorField(position, Color.clear);
				}

				return;
			}

			EditorGUI.BeginChangeCheck();
			var picked = EditorGUI.ColorField(position, GUIContent.none, entry.BranchColor, true, false, false);

			if (!EditorGUI.EndChangeCheck())
			{
				return;
			}

			Undo.RecordObject(entry, "Set Branch Color");

			// 색을 고른 것 자체가 색을 정했다는 뜻이라 알파는 코드가 채운다.
			entry.SetBranchColor(new Color(picked.r, picked.g, picked.b, 1f));
			EditorUtility.SetDirty(entry);

			// 몸통 색이 바뀌었으니 Timeline 창도 다시 그리게 한다
			TimelineEditor.Refresh(RefreshReason.WindowNeedsRedraw);
		}
	}
}
