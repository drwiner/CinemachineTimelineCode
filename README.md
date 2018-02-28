Project: IceBolt - A Narrative Instruction Scheduler for Unity
---

Author: David R. Winer
---

Testing with Unity2017.3.1f1 Code
---


IceBolt Scheduler (system overview):
---

IceBolt is attached to a GameObject in a scene. The scene contains two tagged timeline objects: a fabula timeline (which corresponds to the storyworld) and a discourse timeline (which corresponds to the timeline of camera shots). These timelines contain PlayableDirector components with no timeline asset set.
At runtime (on awake), the timeline assets are generated and populated with specialized clips. IceBolt reads the clips as either json nodes or xml elements with specialized attributes. IceBolt interprets these attributes to generate tracks on the timelines and clips on the appropriate tracks.


Process Overview:
---

1. IceBolt.onAwake() --> 
	* Read Fabula (from input)

		* For each unit, create new Fabula Clip object using fabula Timeline and PlayableDirector
			
			* Assign GameObjects to variables in FabulaClip
			
			* Calculate and add Clips to Tracks in FabulaClip: AnimateBind, TransformBind, TeleportBind, etc. 

	* Read Discourse (from input)

		* For each unit, create new Discourse Clip object using discourse Timeline. Also use overarching Cinemachine Track (ftrack) for all camera actions

			* Add Timeline Trigger --> _when_ in storyworld/fabula timeline to film. 

			* Add Cinemachine Component and set parameters --> calculate and define the cinemachine clip to add to ftrack

			* Calculate camera position and orientation based on shot scale and cinematic constraints defined on input clip

			* CamBind

		* Find Main Camera and assign to ftrack (cinemachine track)

2. IceBolt onStart() -->

	* Play Fabula Timeline from playabledirector.

	* Play Discourse Timeline from playabledirector


Current Status:
---

Project is under development. TODO: accurate shot scales, Leverage Mecanim for walking system (right now, it's a lerp and animate situation).
