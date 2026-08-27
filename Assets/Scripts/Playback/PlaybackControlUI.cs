using TMPro;
using UnityEngine;

namespace TimelineVN.Playback
{
	/// <summary>
	/// 자동재생과 배속 버튼을 받아 VisualNovelDirector에게 넘기고, 켜짐 상태를 라벨 색상으로 알려줌
	/// </summary>
	public class PlaybackControlUI : MonoBehaviour
	{
		/// <summary>
		/// 버튼을 눌렀을 때 알려줄 재생기
		/// </summary>
		[SerializeField]
		private VisualNovelDirector director;

		/// <summary>
		/// 자동재생 버튼의 글자
		/// </summary>
		[SerializeField]
		private TMP_Text autoLabel;

		/// <summary>
		/// 배속 버튼의 글자
		/// </summary>
		[SerializeField]
		private TMP_Text speedLabel;

		/// <summary>
		/// 켜져 있을 때의 글자 색
		/// </summary>
		[SerializeField]
		private Color enabledColor = Color.yellow;

		/// <summary>
		/// 꺼져 있을 때의 글자 색
		/// </summary>
		[SerializeField]
		private Color disabledColor = Color.white;

		/// <summary>
		/// 연결이 빠진 것을 알린다. 버튼은 멀쩡히 눌리는데 아무 일도 안 일어나는 고장이라
		/// 화면에 단서가 안 남는다
		/// </summary>
		private void Awake()
		{
			if (director == null)
			{
				Debug.LogError("Director 가 연결되지 않아 버튼을 눌러도 아무 일도 안 일어난다", this);
			}
		}

		/// <summary>
		/// 처음 화면에 뜰 때 라벨을 지금 상태에 맞춘다.
		/// NOTE : Awake 가 아닌 이유는 재생기가 Awake 에서 하위 객체를 만들기 때문임. 이거 바뀌면 고쳐야함!!
		/// </summary>
		private void Start()
		{
			RefreshLabels();
		}

		/// <summary>
		/// 자동재생 버튼이 눌렸을 때
		/// </summary>
		public void OnAutoButtonClicked()
		{
			if (director == null)
			{
				return;
			}

			director.ToggleAutoAdvance();
			RefreshLabels();
		}

		/// <summary>
		/// 배속 버튼이 눌렸을 때
		/// </summary>
		public void OnSpeedButtonClicked()
		{
			if (director == null)
			{
				return;
			}

			director.ToggleSpeedBoost();
			RefreshLabels();
		}

		/// <summary>
		/// 두 버튼의 글자 색을 지금 켜짐 상태에 맞춘다.
		/// </summary>
		private void RefreshLabels()
		{
			if (director == null)
			{
				return;
			}

			if (autoLabel != null)
			{
				autoLabel.color = director.IsAutoAdvanceEnabled ? enabledColor : disabledColor;
			}

			if (speedLabel != null)
			{
				speedLabel.color = director.IsSpeedBoosted ? enabledColor : disabledColor;
			}
		}
	}
}
