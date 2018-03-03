using UnityEngine;
using UnityEngine.Playables;

public class ParticleAsset : PlayableAsset
{
    public ExposedReference<ParticleSystem> Particles;

    public bool StartEnabled;
    public bool EndEnabled;
    public Color ParticleColour = Color.white;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ParticlePlayable>.Create(graph);
        var particlePlayable = playable.GetBehaviour();
        
        var particles = Particles.Resolve (playable.GetGraph().GetResolver());
        
        particlePlayable.Initialize(particles, StartEnabled, EndEnabled, ParticleColour);

        return playable;
    }
}