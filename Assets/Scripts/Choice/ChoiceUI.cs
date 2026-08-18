using System.Collections.Generic;
using UnityEngine;

namespace TimelineVN.Choice
{
	/// <summary>
	/// 선택지 목록을 화면에 띄우고 플레이어가 고른 항목을 들고 있는다.
	/// </summary>
	public class ChoiceUI : MonoBehaviour
	{
		/// <summary>
		/// 선택지 전체의 표시를 제어하는 캔버스 그룹
		/// </summary>
		[SerializeField]
		private CanvasGroup canvasGroup;

		/// <summary>
		/// 화면에 미리 놓아둔 선택지 버튼들. 이 개수가 한 번에 띄울 수 있는 최대임.
		/// 일단 데모씬은 5개로 해둠.
		/// </summary>
		[SerializeField]
		private ChoiceSlot[] slots;

		/// <summary>
		/// 플레이어가 고른 항목의 Data이며 가져가면 비워진다
		/// </summary>
		private ChoiceOption selected;

		/// <summary>
		/// 선택지가 떠서 고르기를 기다리는 중인지 여부
		/// </summary>
		public bool IsWaitingForSelection { get; private set; }

		/// <summary>
		/// 아직 가져가지 않은 선택 결과가 있는지 여부
		/// </summary>
		public bool HasSelection => selected != null;

		private void Awake()
		{
			// 첫프레임에 튀는거 방지용
			Hide();
		}

		/// <summary>
		/// 선택지 목록을 화면에 띄운다. 슬롯보다 많으면 슬롯 수까지만 띄움. 현재는 5개가 최대
		/// </summary>
		public void Show(List<ChoiceOption> options)
		{
			// 띄울 항목이 있는지는 부르는 쪽이 이미 걸러서 온다. 여기서는 터지지 않게만 막는다
			if (options == null || options.Count == 0)
			{
				Hide();

				return;
			}

			// 슬롯을 켜기 전에 그룹을 먼저 살린다. 순서가 반대면 버튼이 잠긴 채로 켜졌다가
			// 풀리면서 Unity 가 0.1초짜리 색 전이를 돌려서 깜빡이는 것처럼 보이는 문제가 있었음
			SetVisible(true);

			int shownCount = Mathf.Min(options.Count, slots.Length);

			for (int i = 0; i < slots.Length; i++)
			{
				if (i < shownCount)
				{
					slots[i].SetOption(options[i]);
				}
				else
				{
					// 보여줄 필요없는건 비움처리
					slots[i].Clear();
				}
			}

			IsWaitingForSelection = true;
		}

		/// <summary>
		/// 모든 선택지를 화면에서 끄고 남은 선택 결과도 비운다
		/// </summary>
		public void Hide()
		{
			foreach (ChoiceSlot slot in slots)
			{
				slot.Clear();
			}

			SetVisible(false);

			IsWaitingForSelection = false;
			selected = null;
		}

		/// <summary>
		/// 슬롯이 눌렸을 때 그 항목을 선택 결과로 받아둔다
		/// 체크만 해두고 HasSelection 을통해 체크후, TakeSelection으로 가져간다.
		/// 즉 실제 처리는 VisualNovelDirector에서 이거 가져다가 점프처리함
		/// </summary>
		public void Select(ChoiceOption option)
		{
			selected = option;
		}

		/// <summary>
		/// 선택 결과를 꺼낸다. 꺼내면 비워지고, 고른 것이 없으면 null 이다
		/// </summary>
		public ChoiceOption TakeSelection()
		{
			if (!HasSelection)
			{
				Debug.LogError("선택된 결과가 남은게 없음.");
				return null;
			} 

			ChoiceOption taken = selected;
			selected = null;

			return taken;
		}

		/// <summary>
		/// 선택지 전체를 보이거나 감춘다
		/// </summary>
		private void SetVisible(bool visible)
		{
			canvasGroup.alpha = visible ? 1f : 0f;

			// hide 상태일때 버튼이 클릭 안되게 처리
			canvasGroup.interactable = visible;
			canvasGroup.blocksRaycasts = visible;
		}
	}
}
