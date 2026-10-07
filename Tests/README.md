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

For custom icon lookup tests, also copy `CustomIconsValidation.cs.txt` to the
fixture's `Assets/Editor/CustomIconsValidation.cs` and run
`-executeMethod CustomIconsValidation.Run`. This first runs the 14 existing checks,
then seven additional checks using generated test textures: nested project folders,
full-name precedence, short-name associations, missing/default artwork, U# precedence,
and cache invalidation after removing artwork. The generated test folder is cleaned up.
Validated on 2026-10-07: all 21 checks passed in Unity 2022.3.22f1, process exit 0.

For headers, copy `HeaderPrefixValidation.cs.txt` and `HeaderMenuValidation.cs.txt`
to matching `.cs` files under the fixture's `Assets/Editor`, then run
`-executeMethod HeaderMenuValidation.Run`. On 2026-10-07, Unity 2022.3.22f1 passed
19 prefix checks and 10 menu checks (exit 0): triple-character defaults, legacy
name exclusion, prefix boundaries, custom prefix compatibility, insertion above
the target, selection, Undo/Redo, all three menu variants, custom spacing, regex
rejection, and root creation. This does not reproduce or diagnose a historical
editor crash, nor visually verify the context menu or prefab-stage behavior.

For saved-settings migration, copy HeaderMigrationValidation.cs.txt into the fixture as an editor C# file and run HeaderMigrationValidation.Run. Validated in Unity 2022.3.22f1: legacy prefixes upgraded, custom/regex styles and font settings preserved, serialized migration version prevents repeat upgrades; all 29 header/menu checks also passed.
