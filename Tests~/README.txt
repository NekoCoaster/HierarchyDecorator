Constraint hue checks (Unity 2022.3)

Copy Editor/ConstraintHueValidation.cs into Assets/Editor of a disposable Unity
project that references this checkout as a local package. Then run Unity with:

-batchmode -quit -projectPath <project> -executeMethod ConstraintHueValidation.Run -logFile <log>

Requires a graphics device (do not add -nographics). Success logs
CONSTRAINT_HUE_CHECKS_PASSED. Tests cover hue/alpha, greyscale, native mappings,
all six built-in textures, saturation/value preservation, texture reuse/reset,
settings serialization, disable, render-target restoration, and cache cleanup.

The Tests~ folder is ignored by Unity and excluded from release ZIPs.

IconAssignmentsValidation.Run checks bundled mappings, dropped mappings, exact
#60dae6 tint and alpha, original VRCFury colours, VRC-only hue shifts, explicit
AudioLink priority over UdonSharp fallback, and generated-texture cleanup. Copy
Editor/IconAssignmentsValidation.cs into the same disposable project to run it.
DismayIconsValidation.Run checks first-install hue defaults, preservation of saved
values, generic script fallback, PCS inheritance, provisional LMS namespaces,
installation-folder matching, missing icons, and cache invalidation. Copy its
Editor script into the disposable project. Before compiling, create
Assets/!Dismay Custom/Lovestick Massages SFX/Scripts/Runtime/DismayFolderFixture.cs
with: public class DismayFolderFixture : UnityEngine.MonoBehaviour {}
The test creates synthetic PCS/LMS PNGs only when those paths are absent and
removes them afterward. Never run this fixture in a production project.
