using TimelineVN.Timeline;
using UnityEngine.Playables;

namespace TimelineVN.Dialogue
{
	/// <summary>
	/// 대사 트랙의 클립들을 받아 지금 시점에 표시할 대사를 정한다
	/// </summary>
	public class DialogueTrackMixer : PlayableBehaviour
	{
		/// <summary>
		/// 대사창을 비울 때 넘기는 빈 대사. 그냥 empty 해도되지만 편의상..
		/// </summary>
		private static readonly DialogueLine EmptyLine = new DialogueLine();

		/// <summary>
		/// 재생이 시작된 뒤 아직 대사창을 비우지 않았는지 여부
		/// </summary>
		private bool needsClear;

		/// <summary>
		/// 재생이 시작될 때 대사창을 비우도록 예약한다
		/// </summary>
		public override void OnGraphStart(Playable playable)
		{
			// 여기선 바인딩을 못 받아서 실제로 지우는 건 ProcessFrame 에 맡긴다.
			needsClear = true;
		}

		/// <summary>
		/// 지금 활성인 클립을 골라 대사창에 적용시킨다
		/// </summary>
		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			if (playerData is not DialogueUI dialogueUI)
			{
				return;
			}

			int activeInput = FindActiveInput(playable);
			if (activeInput >= 0)
			{
				needsClear = false;

				ScriptPlayable<DialogueClipBehaviour> input = GetClipPlayable(playable, activeInput);

				// 클립 안에서 얼마나 흘렀는지를 넘겨야 타이핑이 어디까지 찍혔는지 나온다
				// 유니태스크나 코루틴으로 안한이유는 ProcessFrame이 어차피 매프레임 불리기도하고,
				// 스크럽에서 이게더 낫다고 판단했음 코루틴이나 유니태스크는 플레이중에만 도니까..
				input.GetBehaviour().Apply(dialogueUI, new ClipTime(input.GetTime(), input.GetDuration()));

				return;
			}

			// 클립을 지우거나 사이를 벌렸을 때 대사창이 깜빡이지 않도록 손대지 않는다
			if (!needsClear)
			{
				return;
			}

			needsClear = false;
			dialogueUI.Show(EmptyLine, 0);
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
		/// 입력에 연결된 클립 노드를 꺼낸다.
		/// behaviour 만이 아니라 노드째로 받는 건 여기서 시간이랑 길이도 읽어야 해서임
		/// </summary>
		private static ScriptPlayable<DialogueClipBehaviour> GetClipPlayable(Playable playable, int inputIndex)
		{
			return (ScriptPlayable<DialogueClipBehaviour>)playable.GetInput(inputIndex);
		}
	}
}
