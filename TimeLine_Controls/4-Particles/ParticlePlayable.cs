using System;
using UnityEngine;
using UnityEngine.Playables;

public class ParticlePlayable : PlayableBehaviour
{
    private ParticleSystem _particles;
    private ParticleSystem.EmissionModule _particlesEmission;

    private bool _startEnabled;
    private bool _endEnabled;
    private Color _particleColour;

    public void Initialize(ParticleSystem particles, bool startEnabled, bool endEnabled, Color particleColour)
    {
        _particles = particles;
        if (_particles != null)
            _particlesEmission = _particles.emission;
        _startEnabled = startEnabled;
        _endEnabled = endEnabled;
        _particleColour = particleColour;
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if (_particles == null)
            return;
        
        _particlesEmission.enabled = _startEnabled;

        ParticleSystem.MainModule mainModule = _particles.main;
        ParticleSystem.MinMaxGradient colourGradient = new ParticleSystem.MinMaxGradient(_particleColour);
        mainModule.startColor = colourGradient;
    }
    
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (_particles == null)
            return;
            
        _particlesEmission.enabled = _endEnabled;
    }
}