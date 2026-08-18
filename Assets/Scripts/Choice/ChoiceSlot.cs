using TMPro;
using UnityEngine;

namespace TimelineVN.Choice
{
	/// <summary>
	/// 선택지 버튼 하나. 맡은 항목을 화면에 표시하고, 눌리면 그 항목을 선택지 UI 에 넘긴다.
	/// </summary>
	public class ChoiceSlot : MonoBehaviour
	{
		/// <summary>
		/// 선택지 문구를 표시할 텍스트
		/// </summary>
		[SerializeField]
		private TMP_Text label;

		/// <summary>
		/// 지금 이 슬롯이 맡고 있는 선택지 항목
		/// </summary>
		private ChoiceOption option;

		/// <summary>
		/// 선택 결과를 넘길 대상
		/// </summary>
		private ChoiceUI owner;

		/// <summary>
		/// 결과를 넘길 대상을 계층에서 찾는다.
		/// 슬롯을 복제해 늘려도 연결할 것이 없도록 인스펙터 참조를 쓰지 않는다
		/// </summary>
		private void Awake()
		{
			owner = GetComponentInParent<ChoiceUI>();

			if (owner == null)
			{
				Debug.LogError("ChoiceUI 아래에 있지 않아 선택 결과를 넘길 곳이 없음", this);
			}
		}

		/// <summary>
		/// 선택지 항목 하나를 맡아 화면에 띄운다
		/// </summary>
		public void SetOption(ChoiceOption option)
		{
			this.option = option;
			label.text = option.Text;

			gameObject.SetActive(true);
		}

		/// <summary>
		/// 맡은 항목을 비우고 화면에서 내린다
		/// </summary>
		public void Clear()
		{
			option = null;

			gameObject.SetActive(false);
		}

		/// <summary>
		/// 맡은 항목을 선택 결과로 넘긴다.
		/// </summary>
		public void OnClicked()
		{
			if (option == null)
			{
				return;
			}

			owner.Select(option);
		}
	}
}
