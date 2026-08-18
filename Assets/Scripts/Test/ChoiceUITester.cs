using System.Collections.Generic;
using TimelineVN.Choice;
using UnityEngine;

namespace TimelineVN.Test
{
	/// <summary>
	/// 선택지 UI 를 눈으로 확인하려고 만든 임시 컴포넌트.
	/// 선택지 트랙이 붙으면 이 폴더 통째로 삭제할 것!!! 꼭 지워야 함!!! 안 지우면
	/// Play 할 때마다 샘플 선택지가 튀어나온다
	/// </summary>
	public class ChoiceUITester : MonoBehaviour
	{
		/// <summary>
		/// 확인할 선택지 UI
		/// </summary>
		[SerializeField]
		private ChoiceUI choiceUI;

		/// <summary>
		/// Play 하면 띄울 샘플 선택지 목록
		/// </summary>
		[SerializeField]
		private List<ChoiceOption> sampleOptions;

		/// <summary>
		/// 샘플 선택지를 띄운다
		/// </summary>
		private void Start()
		{
			choiceUI.Show(sampleOptions);

			Debug.Log($"선택지 띄움. 대기 중인가 = {choiceUI.IsWaitingForSelection}");
		}

		/// <summary>
		/// 재생 제어가 나중에 할 일을 대신한다.
		/// 결과를 가져가 로그를 찍고 선택지를 내린다
		/// </summary>
		private void Update()
		{
			if (!choiceUI.HasSelection)
			{
				return;
			}

			ChoiceOption selected = choiceUI.TakeSelection();

			Debug.Log($"고른 선택지 = {selected.Text}");

			choiceUI.Hide();

			Debug.Log($"선택지 내림. 대기 중인가 = {choiceUI.IsWaitingForSelection}");
		}
	}
}
