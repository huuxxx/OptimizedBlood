# OptimizedBlood

This is project provides code enhancements to [Volumetic Blood Fluids Unity asset pack by kripto289](https://assetstore.unity.com/packages/vfx/particles/volumetric-blood-fluids-173863)

The motivation behind this was [the implementation of the blood asset in my game EXON](https://store.steampowered.com/app/3356980/EXON/)
I'm sure this would also serve others well.

**Features:**
- Restructured code to allow prefabs to be **reused in a pool**
- Removed **all** Update() calls
- Added a persistent singleton class to control all blood prefabs
- Removed redundancy, magic numbers and improved all calculations
- Classes now serve a single responsibility
- Added validation and safety checks

The result is significantly higher performance and reduced overhead, and comprehensive (simplified) visual customization control.

You will also notice **more blood prefabs spawning** than with the standard implementation, due to the spawning calculations succeeding more frequently and reliably.

**Usage Instructions/Setup:**
1) Drag and drop the files into the blood scripts package folder (default directory: KriptoFX/VolumetricBloodFX/Scripts)
  (3x replacement) BFX_DecalSettings, BFX_ManualAnimationUpdate, BFX_ShaderProperties
  (1x new) BFX_GlobalSettings
2) Add the BFX_GlobalSettings to any object within your scene and configure the exposed inspector variables (ensure this script is in any scene you instantiate blood)
3) Optional - if you have implemented an object pool, replace line 49 in BFX_ShaderProperties with your call to return the object to your pool.
4) Optional - if you want to destroy/return a blood gameobject at an explicit time - configure autoDestroy to false/off in BFX_GlobalSettings and call BeginDecalFadeOutDynamic(float duration) in BFX_ShaderProperties on the respective blood gameobject.
5) Instantiate a blood prefab as usual

**WARNING!** only blood prefabs structured as a parent object with 2 child objects containing a decal and blood mesh are compatible with these scripts.
BloodX [prefab parent]
-> Decal [child] (BFX_ShaderProperties, BFX_DecalSettings, HDRP Decal Projector)
-> Blood Mesh [child] (BFX_ManualAnimationUpdate)

As of writing and the blood asset package v1.0.2 the default compatible prefabs are:
Blood1, Blood2, Blood3, Blood4, Blood6, Blood7, Blood8, Blood9 (NOT Blood5!)

**Notes:**
- BFX_BloodSettings.cs on the parent of each prefab serves no purpose and can be removed (but also quite harmless)
- This is **only for the HDRP version of the asset**, but I imagine it would probably take little effort to implement into other rendering piplines (feel free to make a pull request if you do)
- The class names still don't reflect their responsibilities or implementations, but I chose to keep it this way so that his project is a "drag and drop" solution for others
