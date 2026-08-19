using TimelineVN.Choice;
using TimelineVN.Timeline;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineVN.Playback
{
	/// <summary>
	/// 비주얼노벨 재생의 진입점. 이녀석이 사실상 타임라인을 VN 처럼 사용하도록 관장하는 매니저임.
	/// 하위 객체를 만들고 연결하며, 매 프레임 부르는 순서를 정한다
	/// </summary>
	[RequireComponent(typeof(PlayableDirector))]
	public class VisualNovelDirector : MonoBehaviour
	{
		/// <summary>
		/// 다음 대사로 진행하는 입력. 스페이스, 마우스 좌클릭, 터치가 묶여 있다
		/// Dialogue Advance 쓸거임.
		/// </summary>
		[SerializeField]
		private InputActionReference advanceAction;

		/// <summary>
		/// 선택지 화면. 플레이어가 고른 결과를 여기서 가져간다.
		/// </summary>
		[SerializeField]
		private ChoiceUI choiceUI;

		/// <summary>
		/// 이 오브젝트에 붙은 타임라인 재생기. 재생할 타임라인이 바뀌었는지도 여기서 읽는다
		/// </summary>
		private PlayableDirector director;

		/// <summary>
		/// 재생 시각과 속도를 조작한다. 재생 그래프를 직접 만지는 유일한 곳이다
		/// </summary>
		private DirectorTimeControl timeControl;

		/// <summary>
		/// 멈출 자리, 옮겨갈 자리, 끝나는 자리를 들고 매 프레임 그 중 하나를 지나쳤는지 판정한다
		/// </summary>
		private VNTimeScanner scanner;

		/// <summary>
		/// 지금 스캐너를 만들 때 쓴 타임라인(=시나리오).
		/// director 의 것과 달라지면 타임라인이 교체된 것이므로 스캐너를 새로 만든다
		/// </summary>
		private TimelineAsset cachedTimeline;

		/// <summary>
		/// 관장할 하위 객체를 만든다.
		/// 스캐너는 재생할 타임라인을 알아야 만들 수 있어서 여기가 아니라 OnEnable 에서 만든다
		/// </summary>
		private void Awake()
		{
			director = GetComponent<PlayableDirector>();
			timeControl = new DirectorTimeControl(director);
		}

		/// <summary>
		/// 지금 물려 있는 타임라인으로 스캐너를 만들고 입력과 종료 알림을 켠다.
		/// 컴포넌트를 껐다 켜도 그 시점 타임라인 기준으로 다시 잡힌다
		/// </summary>
		private void OnEnable()
		{
			RebuildScanner();
			SetAdvanceActionEnabled(true);
			WarnIfChoiceUIMissing();

			director.stopped += OnDirectorStopped;
		}

		/// <summary>
		/// 입력과 종료 알림을 끄고, 우리가 걸어둔 정지를 푼다
		/// </summary>
		private void OnDisable()
		{
			director.stopped -= OnDirectorStopped;

			SetAdvanceActionEnabled(false);

			// 우리가 건 속도는 우리가 푼다. 안 그러면 이 컴포넌트를 끈 뒤에 다른 코드가
			// 재생시켜도 속도가 0 인 채로 남아 시간이 안 흐름
			timeControl.Resume();
		}

		/// <summary>
		/// 매 프레임 시간축의 어느 점을 지났는지 판정하고 그에 맞게 처리한다.
		/// 판정할 필요가 없는 경우를 위에서부터 걷어내고 마지막에 진짜 판정만 남긴다
		///
		/// 1. 타임라인이 교체됐으면 스캐너를 새로 만든다
		/// 2. 재생 중이 아니면 대기만 풀고 나간다
		/// 3. 고른 선택지가 있으면 그 분기로 옮겨가고 나간다
		/// 4. 대기 중이면 기준 시각만 맞추고, 입력이 오면 재개한다
		/// 5. 연출 도중 입력이면 지금 대사의 대기 지점으로 건너뛴다
		/// 6. 점을 지났으면 멈추거나 옮겨가거나 끝낸다
		///
		/// Update 가 아니라 LateUpdate 인 이유는 Timeline 이 그 둘 사이에서 평가되기 때문이다.
		/// Update 에서 읽으면 직전 프레임 시각이 나와서, 지나친 걸 알아채도 정정이 한 프레임
		/// 늦는다. 그 한 프레임 동안 다음 대사가 화면에 번쩍인다 그래서 개고생함..
		/// </summary>
		/// <example>
		/// 정지 시각이 [0.999999, 1.999999] 이고 재생 할때
		/// 시각이 0.99 -> 1.01 로 넘어간 프레임  ->  0.999999 로 되돌리고 속도 0
		/// 그 상태에서 재개되면                  ->  속도를 되돌려 다시 흐른다
		/// 대기 중에 Timeline 창에서 5초로 끌면  ->  판정 없이 기준만 옮겨 그 자리에 머문다
		/// 0.5 초 재생 중에 재개 되면            ->  0.999999 로 건너뛰고 거기서 멈춘다
		/// </example>
		private void LateUpdate()
		{
			// 분기로 타임라인이 교체되면 시각 목록도 통째로 달라진다
			if (director.playableAsset as TimelineAsset != cachedTimeline)
			{
				RebuildScanner();
			}

			// 재생 전이거나 이미 끝났으면 판정할 것이 없다.
			// 걸어둔 대기 상태만 풀어서 다음 재생이 깨끗하게 시작하도록 한다
			if (!timeControl.IsPlaying)
			{
				timeControl.Resume();

				return;
			}

			// 대기 판정보다 먼저다. 고르는 시점엔 선택지 클립 정지 지점에 이미 멈춰 있어서
			// 순서가 반대면 거기서 나가버리고 결과를 영영 못 가져감.. 순서 조심
			if (TryConsumeSelection())
			{
				return;
			}

			// 선택지가 떠 있는 동안에는 시간을 옮기지 않는다. 옮기면 선택지 클립 구간을
			// 벗어나 슬롯이 꺼지고, 누르는 중이던 클릭이 취소된다
			bool waitingForSelection = choiceUI != null && choiceUI.IsWaitingForSelection;

			if (timeControl.IsWaitingForInput)
			{
				// 대기 중에도 편집자가 Timeline 창에서 재생 헤드를 끌 수 있다. 판정은 하지 않고
				// 기준 시각만 따라가야 재개했을 때 끌어다 놓은 자리에서 이어진다
				scanner.ForceMoveTo(timeControl.CurrentTime);

				if (!waitingForSelection && WasAdvancePressed())
				{
					timeControl.Resume();
				}

				return;
			}

			// 연출 도중 입력하면 기다리지 않고 지금 대사의 대기 지점으로 건너뛴다. 모든 트랙이
			// 상태형이라 건너뛰어도 그 시점의 표정과 배경이 정확히 나온다.
			// 선택지 클립도 정지 지점을 만드니 여기도 막아야 한다. 안 그러면 배경 클릭에
			// 선택지 등장 연출이 날아감
			if (!waitingForSelection && WasAdvancePressed()
				&& scanner.TryGetSkipTarget(timeControl.CurrentTime, out double skipTarget))
			{
				timeControl.StopAt(skipTarget);

				return;
			}

			// 정상 재생이면 이번 프레임에 이만큼 흘렀어야 한다.
			// 스캐너가 이 값과 실제를 견줘서 스크럽이나 점프를 걸러낸다
			double expectedDelta = Time.deltaTime * timeControl.CurrentSpeed;

			// 여기는 게이트를 걸지 않는다. 막으면 선택지 클립의 정지 지점을 못 잡아서
			// 선택지가 뜬 채로 시간이 그냥 지나가버림..
			if (scanner.TryGetPassedPoint(timeControl.CurrentTime, expectedDelta, out VNTimePoint point))
			{
				ApplyPoint(point);
			}
		}

		/// <summary>
		/// 플레이어가 고른 선택지가 있으면 가져가서 그 분기로 옮겨간다.
		/// 가져갈 것이 있었으면 참이다. 옮겼는지와는 상관없다
		/// </summary>
		private bool TryConsumeSelection()
		{
			if (choiceUI == null || !choiceUI.HasSelection)
			{
				return false;
			}

			ChoiceOption selected = choiceUI.TakeSelection();

			// 편집 중에는 분기를 아직 안 이은 항목이 흔하다. 재생이 죽지 않게 재개만 한다
			if (selected == null || !TryJumpTo(selected.Entry))
			{
				timeControl.Resume();
			}

			return true;
		}

		/// <summary>
		/// 지나친 점을 종류대로 처리한다.
		/// 셋 중 옮겨가기만 실패할 수 있고, 그때는 아무 일 없이 계속 흐른다
		/// </summary>
		private void ApplyPoint(VNTimePoint point)
		{
			switch (point.Kind)
			{
				case VNTimePointKind.Stop:
					timeControl.StopAt(point.Time);
					break;

				case VNTimePointKind.Jump:
					// 목적지 클립이 지워졌으면 옮겨가지 않고 그대로 흘려보낸다
					TryJumpTo(point.Destination);
					break;

				case VNTimePointKind.SceneEnd:
					timeControl.EndScene();
					break;
			}
		}

		/// <summary>
		/// 도착지 클립이 있는 자리로 옮겨간다. 그 클립을 못 찾으면 아무것도 안 하고 거짓이다
		/// </summary>
		private bool TryJumpTo(JumpTarget target)
		{
			if (!scanner.TryGetJumpTime(target, out double jumpTime))
			{
				return false;
			}

			timeControl.JumpTo(jumpTime);

			// 우리가 건너뛴 구간은 재생으로 지나온 게 아니다. 기준을 맞춰야 그 사이가 안 걸린다
			scanner.ForceMoveTo(jumpTime);

			return true;
		}

		/// <summary>
		/// 지금 물려 있는 타임라인으로 스캐너를 다시 만든다.
		/// 시각 목록은 타임라인에 박혀 있는 데이터라, 타임라인이 바뀌면 통째로 다시 뽑아야 한다.
		/// 분기로 타임라인을 갈아끼울 때도 이 경로를 그대로 탄다
		/// </summary>
		private void RebuildScanner()
		{
			cachedTimeline = director.playableAsset as TimelineAsset;
			scanner = new VNTimeScanner(cachedTimeline, director.time);
		}

		/// <summary>
		/// 장면이 끝났을 때 불린다. 우리가 장면 끝 클립에서 끝낸 경우와 타임라인이 자연히
		/// 끝난 경우가 둘 다 여기로 온다. 그래서 듣는 쪽은 이 자리 하나만 보면 된다
		/// TODO : 게임플레이를 재개시킬 메시지를 여기서 보낸다. 아직 받는 쪽이 없어서 비워 뒀다
		/// </summary>
		private void OnDirectorStopped(PlayableDirector stoppedDirector)
		{
		}

		/// <summary>
		/// 이번 프레임에 진행 입력이 눌렸는지
		/// </summary>
		private bool WasAdvancePressed()
		{
			if (advanceAction == null)
			{
				return false;
			}

			// 눌려 있는 동안 계속 인정하면 짧은 클립이 이어진 구간에서 대사를 통째로 놓친다.
			// 빠르게 넘기는 것은 나중에 스킵 기능이 맡는다
			if (!advanceAction.action.WasPressedThisFrame())
			{
				return false;
			}

#if UNITY_EDITOR
			// 인스펙터 빈 곳을 클릭했는데 대사가 넘어가는 걸 막음. 일단 에디터는 마우스만 쓰는중이어서 마우스만 처리..
			if (advanceAction.action.activeControl?.device is Mouse && !IsPointerInsideGameView())
			{
				return false;
			}
#endif

			return true;
		}

#if UNITY_EDITOR
		/// <summary>
		/// 마우스 포인터가 게임 화면 안에 있는지.
		/// 빌드에서는 화면 밖을 누를 방법이 없어서 에디터에서만 본다
		/// </summary>
		private static bool IsPointerInsideGameView()
		{
			if (Mouse.current == null)
			{
				return true;
			}

			Vector2 position = Mouse.current.position.ReadValue();

			return position.x >= 0f && position.y >= 0f
				&& position.x <= Screen.width && position.y <= Screen.height;
		}
#endif

		/// <summary>
		/// 진행 입력을 켜거나 끈다
		/// </summary>
		private void SetAdvanceActionEnabled(bool enabled)
		{
			if (advanceAction == null)
			{
				Debug.LogWarning("Advance Action 이 연결되지 않아 대사를 넘길 수 없다", this);

				return;
			}

			if (enabled)
			{
				advanceAction.action.Enable();
			}
			else
			{
				advanceAction.action.Disable();
			}
		}

		/// <summary>
		/// 선택지 화면이 연결되지 않았으면 알린다.
		/// 트랙 바인딩에만 걸고 여기를 빠뜨리면 선택지는 뜨는데 골라도 반응이 없다.
		/// 화면에 단서가 안 남는 고장이라 미리 짚어준다
		/// </summary>
		private void WarnIfChoiceUIMissing()
		{
			if (choiceUI != null)
			{
				return;
			}

			Debug.LogWarning("Choice UI 가 연결되지 않아 선택지를 골라도 분기하지 않는다", this);
		}
	}
}
