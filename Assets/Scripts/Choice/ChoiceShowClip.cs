using System.Collections.Generic;
using System.ComponentModel;
using TimelineVN.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineVN.Choice
{
	/// <summary>
	/// 선택지를 화면에 띄우는 클립. 이 트랙에서 UI 를 실제로 만지는 건 이것뿐이다.
	/// 편집자가 분기를 시작하는 자리이기도 하다. 하나 놓으면 복귀 지점이 딸려 나오고,
	/// 분기는 인스펙터 버튼으로 늘린다(자동으로 return 클립하나랑, entry,exit클립이 하나씩 만들어짐!)
	/// </summary>
	[DisplayName("Choice Show")]
	public class ChoiceShowClip : PlayableAsset, ITimelineClipAsset, IStopPointClip
	{
		/// <summary>
		/// 새 클립을 만들 때의 길이. 선택지가 떠오르는 연출이 들어갈 만큼 넉넉해야함.
		/// </summary>
		private const double DefaultDuration = 1.0;

		/// <summary>
		/// 이 선택지의 번호. 타임라인 안에서 고유하며 클립 보고 아 이게 몇번째 선택분기구나 알수있게 구분용임
		/// </summary>
		[SerializeField]
		private int choiceId;

		/// <summary>
		/// 화면에 띄울 선택지들. 위에서부터 순서대로 뜬다
		/// </summary>
		[SerializeField]
		private List<ChoiceOption> options = new List<ChoiceOption>();

		/// <summary>
		/// 이 선택지의 분기들이 기본으로 돌아갈 자리.
		/// </summary>
		[SerializeField, HideInInspector]
		private ChoiceReturnClip defaultReturn;

		/// <summary>
		/// 이 선택지의 번호
		/// </summary>
		public int ChoiceId => choiceId;

		/// <summary>
		/// 화면에 띄울 선택지들
		/// </summary>
		public List<ChoiceOption> Options => options;

		/// <summary>
		/// 이 선택지의 분기들이 기본으로 돌아갈 자리
		/// </summary>
		public ChoiceReturnClip DefaultReturn => defaultReturn;

		/// <summary>
		/// 띄울 선택지가 하나라도 있는지 여부
		/// </summary>
		public bool HasOptions => options.Count > 0;

		/// <summary>
		/// 선택지는 겹치지 않으므로 블렌딩을 막는다
		/// </summary>
		public ClipCaps clipCaps => ClipCaps.None;

		/// <summary>
		/// 새 클립이 생성될 때의 길이
		/// </summary>
		public override double duration => DefaultDuration;

		/// <summary>
		/// 이 클립 끝에서 멈추고 플레이어를 기다릴지 여부.
		/// 항목이 없으면 안 멈춘다. 화면은 비었는데 멈추기만 하면 원인을 알 수 없어서 문제될수있음.
		/// 보통은 쓸일이 없고 그냥 작업할때 놓치기 쉬우니까..
		/// </summary>
		/// <example>
		/// 문구를 적은 선택지  ->  클립 끝에서 멈추고 플레이어를 기다린다
		/// 방금 만든 빈 클립   ->  안 멈추고 지나간다
		/// </example>
		public bool CreatesStopPoint => HasOptions;

		/// <summary>
		/// 이 선택지의 번호를 정한다. 클립을 만들 때 트랙에서 발급받는다
		/// </summary>
		public void SetChoiceId(int choiceId)
		{
			this.choiceId = choiceId;
		}

		/// <summary>
		/// 분기들이 기본으로 돌아갈 자리를 정한다. 클립을 만들 때 이어진다.
		/// </summary>
		public void SetDefaultReturn(ChoiceReturnClip defaultReturn)
		{
			this.defaultReturn = defaultReturn;
		}

		/// <summary>
		/// 이 클립의 선택지들을 실어 나를 재생 노드를 만든다
		/// </summary>
		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			var playable = ScriptPlayable<ChoiceShowClipBehaviour>.Create(graph);
			playable.GetBehaviour().SetData(options);

			return playable;
		}
	}
}
