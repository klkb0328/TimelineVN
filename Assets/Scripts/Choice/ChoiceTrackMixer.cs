using UnityEngine.Playables;

namespace TimelineVN.Choice
{
	/// <summary>
	/// 선택지 트랙에서 지금 띄울 선택지를 고른다. 즉 기존 ChoiceShowBehaviour는 말그대로 기능만 제공해주고
	/// 실제로 이거 호출하는건 여기임!
	/// 그리고 여기 후보로 들어오는 건 선택지 클립뿐이다.(일단은)
	/// </summary>
	public class ChoiceTrackMixer : PlayableBehaviour
	{
		/// <summary>
		/// 활성인 선택지 클립을 골라 화면에 띄운다. 없으면 선택지를 꺼버림
		/// </summary>
		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			if (playerData is not ChoiceUI choiceUI)
			{
				return;
			}

			int activeInput = FindActiveInput(playable);
			if (activeInput < 0)
			{
				HideIfShown(choiceUI);

				return;
			}

			ChoiceShowClipBehaviour clip = GetClip(playable, activeInput);

			// 방금 만들어서 아직 문구를 안 적은 클립일때 끄기(이건 말그대로 연출제작중에 Play눌럿을때 막히는거 막기위함임)
			if (clip.Data.Count == 0)
			{
				HideIfShown(choiceUI);

				return;
			}

			// 선택된 클립 behaviour 사용하기
			clip.Apply(choiceUI);
		}

		/// <summary>
		/// 떠 있을 때만 선택지를 꺼버린다.
		/// 대사창과 달리 선택지는 클립 사이 간격에 남아 있으면 안 된다.
		/// 매 프레임 Hide 를 부르지 않는 건 방금 고른 결과까지 지워지는 문제가 있음
		/// </summary>
		private static void HideIfShown(ChoiceUI choiceUI)
		{
			if (!choiceUI.IsWaitingForSelection)
			{
				return;
			}

			choiceUI.Hide();
		}

		/// <summary>
		/// 지금 활성인 입력의 인덱스를 찾는다. 없으면 -1
		/// </summary>
		private static int FindActiveInput(Playable playable)
		{
			int inputCount = playable.GetInputCount();
			
			for (int i = 0; i < inputCount; i++)
			{
				if (playable.GetInputWeight(i) > 0f)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// 입력에 연결된 클립을 꺼낸다
		/// </summary>
		private static ChoiceShowClipBehaviour GetClip(Playable playable, int inputIndex)
		{
			var input = (ScriptPlayable<ChoiceShowClipBehaviour>)playable.GetInput(inputIndex);
			return input.GetBehaviour();
		}
	}
}
