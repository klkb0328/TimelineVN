using UnityEngine.Playables;

namespace TimelineVN.Playback
{
	/// <summary>
	/// PlayableDirector 의 시간과 속도를 관리한다.
	/// 재생 그래프를 직접 만지는 것은 이 클래스뿐이다
	/// VisualNovelDirector 자식으로 존재하고 시간 기능 제공할뿐 실제 호출은 VND에서 해야함
	/// 어디서 멈출지는 VNTimeScanner 가 정한다. 여기는 시키는 자리에서 멈추기만 한다
	/// </summary>
	public class DirectorTimeControl
	{
		/// <summary>
		/// 제어할 타임라인 재생기
		/// </summary>
		private readonly PlayableDirector director;

		/// <summary>
		/// 멈춰 있지 않을 때 흘러야 하는 속도.
		/// 정지는 이 값을 건드리지 않고 그래프 속도만 0 으로 만들었다가 여기서 복원한다
		/// </summary>
		private double playbackSpeed = 1;

		/// <summary>
		/// 정지 지점에서 멈춰 입력을 기다리는 중인지
		/// </summary>
		private bool isWaitingForInput;

		/// <summary>
		/// 타임라인이 재생 중인지. 시작 전이거나 이미 끝난 뒤에는 거짓이다
		/// </summary>
		public bool IsPlaying => director.playableGraph.IsValid() && director.state == PlayState.Playing;

		/// <summary>
		/// 정지 지점에서 입력을 기다리는 중인지
		/// </summary>
		public bool IsWaitingForInput => isWaitingForInput;

		/// <summary>
		/// 지금 실제로 시간이 흐르는 속도. 입력을 기다리는 중에는 0 이다
		/// </summary>
		public double CurrentSpeed => isWaitingForInput ? 0 : playbackSpeed;

		/// <summary>
		/// 현재 재생 시각
		/// </summary>
		public double CurrentTime => director.time;

		/// <summary>
		/// 제어할 디렉터를 받는다
		/// </summary>
		public DirectorTimeControl(PlayableDirector director)
		{
			this.director = director;
		}

		/// <summary>
		/// 주어진 시각으로 되돌린 뒤 그 자리에서 멈춘다
		/// </summary>
		public void StopAt(double time)
		{
			// 프레임이 정지 지점에 딱 떨어지는 일은 없어서 이미 지나쳐 있다.
			// 그 자리로 되돌려야 멈춘 시점의 대사가 화면에 남는다
			director.time = time;

			// 시각을 넣는 것만으로는 그래프가 평가되지 않는다. 여기서 반영시켜야
			// 되돌린 결과가 화면에 그려지기 전에 정정된다!!!!
			director.Evaluate();

			ApplySpeed(0);
			isWaitingForInput = true;
		}

		/// <summary>
		/// 멈춰 있던 재생을 원래 속도로 되돌린다
		/// </summary>
		public void Resume()
		{
			if (!isWaitingForInput)
			{
				return;
			}

			isWaitingForInput = false;
			ApplySpeed(playbackSpeed);
		}

		/// <summary>
		/// 주어진 시각으로 옮기고 계속 흐르게 한다.
		/// 옮긴 자리에서 멈추지 않는다는 점이 StopAt 과 다르다
		/// </summary>
		/// <example>
		/// 선택지를 골랐을 때  ->  그 분기가 시작하는 시각으로 옮긴다
		/// 분기 끝에 닿았을 때 ->  복귀 지점 시각으로 옮긴다
		/// </example>
		public void JumpTo(double time)
		{
			director.time = time;

			// StopAt 과 같은 이유로 여기서 평가한다. 안 하면 옮기기 전 화면이 한 프레임 보인다.
			director.Evaluate();

			// 선택지에서 옮겨올 때는 정지 지점에 멈춰 있는 상태(속도 0)다.
			// 여기서 안 풀면 착지한 자리에서 그대로 얼어붙음..
			isWaitingForInput = false;
			ApplySpeed(playbackSpeed);
		}

		/// <summary>
		/// 재생을 끝낸다. 장면이 끝났을 때 부른다.
		/// 속도 0 과 다르다. 속도 0 은 그래프가 살아 있어서 트랙 믹서가 매 프레임 계속 돌고
		/// 대사창에 자기 값을 다시 써넣음. 끝난 게 아니라 얼어붙은 거라 게임플레이가
		/// 재개돼도 Timeline 이 화면을 안 놓는다.. Stop 은 그래프를 정리해서 손을 뗀다
		/// </summary>
		public void EndScene()
		{
			// 그래프가 사라지면 속도를 되돌릴 대상도 없어서 미리 정리한다
			isWaitingForInput = false;

			// 여기서 stopped 이벤트가 나간다. 타임라인이 자연히 끝났을 때와 같은 자리다
			director.Stop();
		}

		/// <summary>
		/// 재생 그래프의 모든 루트에 속도를 건다
		/// 일단 그래프가 보통 한개일텐데 혹시 몰라서 for문으로 돌림..
		/// </summary>
		private void ApplySpeed(double speed)
		{
			PlayableGraph graph = director.playableGraph;

			// 재생 전이거나 그래프가 이미 정리된 뒤에는 걸 대상이 없다
			if (!graph.IsValid())
			{
				return;
			}

			// 루트가 하나라고 가정하지 않는다. Timeline 자신도 속도를 맞출 때 루트 개수만큼 돈다
			int rootCount = graph.GetRootPlayableCount();
			for (int i = 0; i < rootCount; i++)
			{
				graph.GetRootPlayable(i).SetSpeed(speed);
			}
		}
	}
}
