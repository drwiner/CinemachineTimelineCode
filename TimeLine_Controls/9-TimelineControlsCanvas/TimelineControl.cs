using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class TimelineControl : MonoBehaviour
{
	public PlayableDirector Director;
	public Slider TimeControlSlider;
	public Text MaxTimeLabel;
	public Text CurrentTimeLabel;

	void Start()
	{
		TimeControlSlider.maxValue = (float)Director.duration;

		MaxTimeLabel.text = Director.duration.ToString();
	}

	void Update()
	{
		float time = (int)(Director.time * 1000) / 1000f;
		CurrentTimeLabel.text = time.ToString();

		TimeControlSlider.value = (float)Director.time;
	}

	public void PlayTimeline()
	{
		StopCooutines();
		Director.Play();
	}

	public void StopTimeline()
	{
		PauseTimeline();
		Director.time = 0f;
		Director.Evaluate();
	}

	public void PauseTimeline()
	{
		StopCooutines();
		Director.Pause();
	}

	public void Rewind()
	{
		PauseTimeline();
		StartCoroutine("DoRewindStep");
	}

	public void FastForward()
	{
		PauseTimeline();
		StartCoroutine("DoFastForwardStep");
	}

	public void OnSliderValueChanged()
	{
		Director.time = TimeControlSlider.value;
		Director.Evaluate();
	}

	void StopCooutines()
	{
		StopCoroutine("DoRewindStep");
		StopCoroutine("DoFastForwardStep");
	}

	private IEnumerator DoRewindStep()
	{
		yield return new WaitForSeconds(0.001f);

		Director.time -= 0.1f;

		if (Director.time < 0f)
		{
			Director.time = 0f;
			Director.Evaluate();
		}
		else
		{
			Director.Evaluate();
			StartCoroutine("DoRewindStep");
		}
	}

	private IEnumerator DoFastForwardStep()
	{
		yield return new WaitForSeconds(0.001f);

		Director.time += 0.1f;
	
		if (Director.time > Director.duration)
		{
			Director.time = Director.duration;

		}
		else
		{
			Director.Evaluate();
			StartCoroutine("DoFastForwardStep");
		} 
	}
}
