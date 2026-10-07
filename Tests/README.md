# Fork validation

Run `python -m unittest discover -s Tests -p 'test_*.py'` for archive layout,
manifest, release URL, checksum, deterministic packaging, and version retention checks.

For Unity validation, create a disposable Unity 2022.3 project with this repository as
a local UPM package, uGUI, and Unity's animation, IMGUI, physics and physics2d modules.
Copy `ForkValidation.cs.txt` to `Assets/Editor/ForkValidation.cs` in that project.
The integration checks use type-shape fixtures (not real SDK assemblies). Copy the
installed VRCFury `VrcfResources/logo.png` to the same path in an embedded test package
named `com.vrcfury.vrcfury` with a minimal package.json, and copy the installed Worlds SDK
`Integrations/UdonSharp/Editor/Resources/UdonSharpProgramAsset icon.png` to
`Assets/Editor/Resources/UdonSharpProgramAsset icon.png`. Keep real SDKs out of
this synthetic fixture because its stand-in types would conflict.

Run Unity with `-batchmode -nographics -quit -projectPath <fixture> -executeMethod
ForkValidation.Run -logFile <log>`. Success emits `HIERARCHY_FORK_CHECKS_PASSED=14`.
The test exercises rig references, attachments, inactive objects, reparent/removal,
root bones, default icon fallback, integration icon selection, generated bone pixels,
and keeping resolved icons out of serialized settings.

Validated on 2026-10-07: Unity 2022.3.22f1, 14 checks passed, process exit 0;
Python packaging suite passed. A prior SDK-free run also compiled and passed 9 checks.

Before releasing, manually verify in actual Avatar and World projects: VRCFury and
Udon icons, stacked/unstacked icons, light/dark themes, bone toggle, humanoid rigs,
prefab mode, Undo/Redo, and clean VCC installation from the published listing.
Those GUI/SDK and online installation checks are not covered by the synthetic run.
