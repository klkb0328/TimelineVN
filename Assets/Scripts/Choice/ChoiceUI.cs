using System.Collections.Generic;
using UnityEngine;

namespace TimelineVN.Choice
{
	/// <summary>
	/// 선택지 목록을 화면에 띄우고, 플레이어가 고른 항목을 들고 있는다.
	/// 고른 뒤에 무엇을 할지는 결과를 가져가는 쪽이 정한다.
	/// TODO: ChoiceClip에서 활성화 시킬거고, 일단 ChoiceUITester로 처리..
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
		/// </summary>
		[SerializeField]
		private ChoiceSlot[] slots;

		/// <summary>
		/// 플레이어가 고른 항목. 가져가면 비워진다
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

		/// <summary>
		/// 편집 편의상 씬에 선택지를 보이게 둬도 첫 프레임에 번쩍이지 않도록 감춘다
		/// </summary>
		private void Awake()
		{
			Hide();
		}

		/// <summary>
		/// 선택지 목록을 화면에 띄운다. 슬롯보다 많으면 슬롯 수까지만 띄움. 현재는 5개가 최대
		/// </summary>
		public void Show(List<ChoiceOption> options)
		{
			if (options == null || options.Count == 0)
			{
				Debug.LogError("띄울 선택지가 없다", this);
				Hide();

				return;
			}

			int shownCount = Mathf.Min(options.Count, slots.Length);

			for (int i = 0; i < slots.Length; i++)
			{
				if (i < shownCount)
				{
					slots[i].SetOption(options[i]);
				}
				else
				{
					slots[i].Clear();
				}
			}

			SetVisible(true);

			IsWaitingForSelection = true;
		}

		/// <summary>
		/// 선택지를 화면에서 내리고 남은 선택 결과도 비운다
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
		/// 즉 실제 처리는 VisualNovelDirection에서 이거 가져다가 점프처리함
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

			// 감춘 뒤에도 켜져 있으면 보이지 않는 버튼이 클릭을 먹는다
			canvasGroup.interactable = visible;
			canvasGroup.blocksRaycasts = visible;
		}
	}
}
