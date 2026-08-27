namespace TimelineVN.Playback
{
	/// <summary>
	/// 자동재생과 배속을 관리한다.
	/// 시간을 직접 만지지는 않으며, 지금 몇 배속인지와 자동으로 넘길 때가 되었는지 알려주고
	/// 실제로 옮기고 재개하는 것은 DirectorTimeControl 이다
	/// </summary>
	public class PlaybackModeControl
	{
		/// <summary>
		/// 배속을 켰을 때의 속도
		/// </summary>
		private const double BoostedSpeed = 2;

		/// <summary>
		/// 배속을 껐을 때의 속도
		/// </summary>
		private const double NormalSpeed = 1;

		/// <summary>
		/// 자동재생일 때 정지 지점에서 기다리는 시간
		/// </summary>
		private readonly float autoAdvanceWaitSeconds;

		/// <summary>
		/// 지금 정지 지점에서 기다린 시간
		/// </summary>
		private float waitedSeconds;

		/// <summary>
		/// 자동재생이 켜져 있는지
		/// </summary>
		public bool IsAutoAdvanceEnabled { get; private set; }

		/// <summary>
		/// 배속이 켜져 있는지
		/// </summary>
		public bool IsSpeedBoosted { get; private set; }

		/// <summary>
		/// 지금 걸어야 할 속도
		/// </summary>
		public double Speed => IsSpeedBoosted ? BoostedSpeed : NormalSpeed;

		/// <summary>
		/// 정지 지점에서 기다릴 시간을 받는다
		/// </summary>
		public PlaybackModeControl(float autoAdvanceWaitSeconds)
		{
			this.autoAdvanceWaitSeconds = autoAdvanceWaitSeconds;
		}

		/// <summary>
		/// 자동재생을 켜거나 끈다
		/// </summary>
		public void ToggleAutoAdvance()
		{
			IsAutoAdvanceEnabled = !IsAutoAdvanceEnabled;

			// 켠 순간부터 세야 한다. 끄기 전에 쌓인 것이 남아 있으면 다시 켤 때 바로 넘어감
			Reset();
		}

		/// <summary>
		/// 배속을 켜거나 끈다
		/// </summary>
		public void ToggleSpeedBoost()
		{
			IsSpeedBoosted = !IsSpeedBoosted;
		}

		/// <summary>
		/// 기다린 시간을 0 으로 되돌린다
		/// </summary>
		public void Reset()
		{
			waitedSeconds = 0f;
		}

		/// <summary>
		/// 기다린 시간을 누적하고, 자동으로 넘길 때가 됐으면 참이다.
		/// 참을 준 뒤에는 스스로 0 으로 되돌려서 다음 정지 지점부터 다시 센다
		/// </summary>
		/// <example>
		/// 대기 2초, 프레임마다 0.016초씩 들어온다면
		/// 1.98 까지  ->  거짓. 아직 덜 찼다
		/// 2.00 에서  ->  참. 그리고 0 으로 되돌아간다
		/// </example>
		public bool Tick(float deltaTime)
		{
			// 꺼져 있으면 세지 않는다. 켜는 순간부터 재는 것이라 여기서 쌓아두면 안 됨
			if (!IsAutoAdvanceEnabled)
			{
				return false;
			}

			waitedSeconds += deltaTime;

			if (waitedSeconds < autoAdvanceWaitSeconds)
			{
				return false;
			}

			Reset();

			return true;
		}
	}
}
