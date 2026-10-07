# HierarchyDecorator - Neko's Fork

**[Install with VCC / VPM](https://nekocoaster.github.io/HierarchyDecorator/)** · [Repository JSON](https://nekocoaster.github.io/HierarchyDecorator/index.json)

Open the install page and click **Add to VCC**, then choose **Manage Project** and install **HierarchyDecorator - Neko's Fork**. Alternatively, paste the repository JSON URL into VCC **Settings → Packages → Add Repository**.

These links become available after the first release workflow deploys successfully. Before publication, add this checkout through VCC's **User Packages** settings.

This fork adds VRCFury logos, U# icons for Udon Behaviours and UdonSharp scripts, and a bone indicator for rig bones and objects parented beneath them. It reads skinned-mesh and humanoid rig references, including inactive rigs; simply naming an object `Armature` does not mark it as a bone. Existing component filters and the overall icon toggle apply; **Show Bone Icons** independently toggles the bone indicator in icon settings. Neither VRChat nor VRCFury is a required dependency.

VPM migrates the upstream UPM package (`com.wooshii.hierarchydecorator`). For an old Assets-based installation, remove its scripts before installing this fork, preserving your Settings asset. Keeping both copies causes duplicate classes. Automatic folder deletion is avoided because upstream stores user settings alongside its scripts.

## Headers

Right-click a scene object in the Hierarchy and choose **Hierarchy Decorator →
Header / Subheader / Mini Header** to insert a label immediately above it at the
same level. With no context object, the label is created at the current stage's
root. The new object is selected for renaming, and creation supports Undo.
Commands use the corresponding saved style's prefix; a command is disabled if
that named style was removed, renamed, or changed to a regular expression.

Create an empty GameObject and name it `=== ENVIRONMENT` for a centered header,
`--- Lighting` for a left-aligned subheader, or `+++ Props` for a small centered
header. Include the space after the prefix, then move the object to the desired
position in the hierarchy. Single-character prefixes no longer match the defaults.

Existing settings receive a one-time upgrade of the original named built-in styles:
`=` becomes `===`, `-` becomes `---`, and `+` becomes `+++`. Custom prefixes,
renamed styles, regex styles, and appearance settings are preserved. You can change
the prefixes afterward under **Edit → Preferences → Hierarchy Decorator → Visual**.
Existing single-prefix header GameObjects must be renamed to use the new prefixes;
the upgrade does not rename scene objects.

## Custom component icons

Place your own licensed textures in `Assets/HierarchyDecorator/CustomIcons/`.
Name each image after the component class, such as `AudioLink.png`, `LTCGI_Screen.png`,
or `VRCPhysBone.png`. A fully qualified class name can disambiguate matching names.
Subfolders are supported. Project icons override matching bundled filenames.
`Default.png` replaces generic script icons; `Missing.png` represents missing scripts.
The U# icon still takes precedence for Udon and UdonSharp behaviours when available.
Icons refresh after project changes, or through **Tools → HierarchyDecorator → Refresh Custom Icons**.

The public package currently includes the MIT-licensed icon loader, without
HierarchyPlus artwork. Separately supplied artwork retains its own terms.

**Icons8 artwork attribution:** Icons by [Icons8](https://icons8.com), when installed.
**Reusing Icons8 icons requires an active Icons8 license.** The MIT license for
HierarchyDecorator code does not cover Icons8 artwork. The same credit and link
are available through **Tools → HierarchyDecorator → Icon Artwork Credits**.

Icons8 support has asked us to wait for the team's final confirmation before
publishing artwork in the repository or installation ZIPs. The open-source
application and account/download setup are pending. Once approved, artwork for
the public bundle will be sourced directly from Icons8 under the finalized terms.

## Publishing updates

1. Enable Actions on the fork. In GitHub **Settings → Pages**, select **GitHub Actions** as the source.
2. Update `package.json` version and its release ZIP `url`, then update the changelog.
3. Push a matching tag, e.g. `v0.13.0`. The **Release VPM package** workflow builds the ZIP, publishes a release, and deploys the installation page and VPM listing. Existing listed versions are retained.
4. Check the workflow, the JSON URL, and a clean VCC install before announcing the release. A failed run can be rerun on the same tag.

Local packaging: `python Tools/build_vpm.py`. Validation: `python -m unittest discover -s Tests -p 'test_*.py'`. Generated files are in `dist/`.

Original project and MIT attribution follow.

---

<h1 align="center">  
 <img width="824" alt="HierarchyDecoratorNew" src="https://user-images.githubusercontent.com/31889435/226486126-009081e1-44de-465c-8ff7-5641870fdcae.png">
 
 Hierarchy Decorator
</h1>

<h4 align="center"> Unity Editor plugin giving the Hierarchy a lick of paint.<br><br>
 
 Fully Customisable.<br>
 Toggle Everything.</h4>

<p align="center">
 <a href="https://unity3d.com/get-unity/download">
 <img src="https://img.shields.io/badge/unity-2018.4%2B-blue.svg" alt="Unity Download Link">
 <a href="https://github.com/WooshiiDev/HierarchyDecorator/blob/master/LICENSE">
 <img src="https://img.shields.io/badge/License-MIT-brightgreen.svg" alt="License MIT">
</p>
  

<p align="center">
  <a href="#about">About</a> •
  <a href="#installation">Installation</a> •
  <a href="#features">Features</a> •
  <a href="#support">Support</a> •
  <a href="#donate">Donate</a>
</p>

## About

Hierarchy Decorator is an extension for Unity 2018.4 and higher that extends Unity's hierarchy and takes it to the next level. With headers, component information and other features, it transforms the window into more than a plain list of objects. This can turn scene structures easier to read, understand and provide information on what is going on.

Everything is optional, and can be modified to the requirements of the project.

<p align="center">
<img width="372" alt="Unity_k2yhUSLugm" src="https://user-images.githubusercontent.com/31889435/226486583-8ad71e2b-1051-46b3-b00a-3880acffc413.png">
<img width="372" alt="Unity_myS52drEnl" src="https://user-images.githubusercontent.com/31889435/226486476-768a99ad-ae6f-4609-b8f8-8537c1f2393d.png">
</p>

## Installation

Use the VCC/VPM installation instructions at the top of this README for Neko's fork.
For Unity Package Manager's **Add package from git URL**, use:

```text
https://github.com/NekoCoaster/HierarchyDecorator.git
```

The original upstream package and this fork must not be installed together.

## Features

### Current

| Feature                    | Hierarchy Decorator  | Other Hierachy Extensions |
| -------------------------- | :----------------: | :-------------:   |
| Hierarchy Headers/Styles   |         ✔️         |        ✔️        |
| Tag/Layer Selector         |         ✔️         |        ✔️        |
| Breadcrumbs                |         ✔️         |        ✔️        |
| Component Icons            |         ✔️         |        ❌        |

### Planned 

| Feature                    | Hierarchy Decorator  | Other Hierachy Extensions |
| -------------------------- | :----------------: | :-------------:   |
| GameObject Icons           |         Planned     |        ✔️        |
| Folders                    |         Planned     |         ✔️       |
| Script Error/Warning Popup |         Planned     |        ❌       |
| Editor Flags Selector      |         Planned     |        ❌       |
| <a href="https://github.com/WooshiiDev/HierarchyDecorator/issues/25">Team/Individual Settings Mode</a>   |         Planned     |        ❌        |
  
## Settings
  
<p align="center">
 <img align="center" width="929" alt="chrome_hzlst44Z1X" src="https://user-images.githubusercontent.com/31889435/226493547-3ec3db89-bcdf-4412-b000-c90aa9ee30b7.png">
</p>

There is a scriptable object that is required for hierarchy decorator to run. If it is deleted, another will be created in `Assets/HierarchyDecorator/`. These settings are also accessible from `Preferences`.

Setting design may change over time with development to support more features, or keep things looking consistent & clean.

### General
  
<details>
 <summary><b>Toggles</b></summary>
  
 <p align="center">
  <img width="915" alt="chrome_aClIcjH3wq" src="https://user-images.githubusercontent.com/31889435/226558578-78287342-711c-4b4b-acf3-18b316f3216b.gif">
  <img width="778" height="145" alt="Unity_GYpPOVFRrv" src="https://github.com/user-attachments/assets/fac34b54-684e-4499-b68b-a572d312bbd2" />
 </p>

  Toggles will simply display the state of the instance, can be clicked to toggle the instance active state.

  ```
  Show Active Toggles     Enable the toggles.
  Active Toggle Type      Choose between a checkbox or dot for the toggle icon.
  Active Swiping          Click and drag over check boxes to toggle them.
  Swipe Same State        Only toggle the instances with the same state as the first selected.
  Swipe Selection Only    If a selection exists, only toggle the selected instances.
  Depth Mode              The accepted criteria for selecting instances when swiping.
  ```
</details>

<details>
 <summary><b>Layers</b></summary>

<p align="center">
  <img width="913" alt="chrome_szO7gPHVZ4" src="https://user-images.githubusercontent.com/31889435/226493749-b30ebd4a-bf89-4841-bde8-b78159ec6068.png">
</p>
  
 Display the current layer the instance is assigned to.
  
  ```
  Show Layers             Enable the toggles.
  Click To Select Layer   Clicking the layer label will display a layer dropdown to update it.
  Apply Child Layers      Change the child gameobjects when updating the layer above.
  ```
</details>

<details>
 <summary><b>Breadcrumbs</b></summary>

<p align="center">
   <img width="922" alt="chrome_DtNbO5Mimi" src="https://user-images.githubusercontent.com/31889435/226493794-e45fbb59-ec38-430a-a3ba-2cd137251f46.png">
</p>
  
  Breadcrumbs will show line trails in the hierarchy, between objects to help visualise the tree. 
  
  _Instance_ settings are related to breadcrumbs drwan for the instance and it's siblings.<br>
 _Hierarchy_ settings will modify how breadcrumbs are displayed for higher depths.
  
  ```
  Show                    Show the breadcrumbs.
  Color                   The colour of the drawn lines.
  Style                   The line style - Solid, Dash, Dotted.
  Display Horizontal      Draw a horizontal line, from left to right, towards the instance.
  ```
</details>
 
### Visual

<details>
 <summary><b>Background</b></summary>
 
<p align="center">
 <img width="612" alt="chrome_Y3lak6Q0Dm" src="https://user-images.githubusercontent.com/31889435/226495114-e578f1c4-60d4-473e-8b42-b2c09c0fdeb1.png">
</p>

  The background can be enabled to alternate background colour between each hierarchy row. 

  ```
  Alternate Background    Show the breadcrumbs.
  Color One               The first colour for the theme.
  Color Two               The second colour for the theme.
  ```
</details>

<details>
 <summary><b>Styles</b></summary>
   
  The Style tab controls the design of the headers and seperators for the hierarchy. Colours are individual for light and dark mode providing accessibility. The **prefix** is the string to specify at the start of an instance name to apply the style.
  
  Layers and icons can be specifically disables on styles instances to remove clutter and information that is not required.

 <p align="center">
  <img width="510" height="450" alt="image" src="https://github.com/user-attachments/assets/1d727086-ea26-4ea9-ad66-26873b39d6b2" alt="Style Settings"/>
 </p>
</details>

  ### Icons

 Icons can be displayed that represent components that exist on gameobjects. This tab will provide the flexibility to specify what components can and cannot be displayed, and also allow you to automatically show all. 

<p align="center">
 <img width="132" height="327" alt="Unity_emGzaT8YHM" src="https://user-images.githubusercontent.com/31889435/226554415-8bd0be96-6eb2-4217-8d56-e39d23dd7ffd.png"><img width="627" alt="Unity_iWzNrNwYKa" src="https://user-images.githubusercontent.com/31889435/226494920-6b78be6e-686d-42ac-a11f-629f270cb5bc.png">
</p>

<details>
 <summary><b>Settings</b></summary>
 
 - Enable Icons: Will toggle the icons on.
 - Stack Mono Behaviours - If there are any MonoBehaviour derived components, show only one script icon on the instance.
 - Show Missing Script Warning - Show an indicator if there's an invalid script or "missing" script that cannot find the source file.

</details>

<details>
 <summary><b>Icon Panel</b></summary>
 
**Show All**

Below show all are two labels - Unity & Custom. Both of these can be enabled to automatically show the respective components automatically on all instances.
Unity components refer to built in types, while custom are custom MonoBehaviour's outside of Unity's code base.

**Groups**

Unity components have been categorized into related groups to make it easier to filter through all of them that exist. The search can be used to extend this further.

Any component toggled on in Excluded will disable them completely even if show all Unity components is enabled. This is primarily to make it easier to remove types not required when **Show All** is on.

**Custom**

Custom components are for scripts created in the project, that are not a part of Unity's engine. Here scripts can be grouped together and enabled if **Show All** for custom components is not on. 

Scripts can also be dragged in from the project view and will be added to the group highlighted for easy organisation.

<p align="center">
 <img width="40%" alt="Unity_emGzaT8YHM" src="https://user-images.githubusercontent.com/31889435/226555645-85954060-c25e-4ae8-bf5b-c7313d1188ee.gif">
 <img width="40%" alt="Unity_emGzaT8YHM" src="https://user-images.githubusercontent.com/31889435/226556916-73888fe0-b8fa-4365-88ab-18d786aa7c37.gif">
</p>

</details>

## Support
Please submit any queries, bugs or issues, to the [Issues](https://github.com/WooshiiDev/HierarchyDecorator/issues) page on this repository. All feedback is appreciated as it not just helps myself find problems I didn't otherwise see, but also helps improves Hierarchy Decorator as a whole.

A GitHub Project [Board](https://github.com/users/WooshiiDev/projects/1) for this also exists showing current development goals and future features.

Reach out to me or see my other work through:

 - Website: https://wooshii.dev/
 - Email: wooshiidev@gmail.com;

## Donate
HierarchyDecorator will be and always has been developed in my free time, and there are many more features I'd like to include. If you would to support me, you can do so below:

[![PayPal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://paypal.me/Wooshii?locale.x=en_GB)
<p href="https://ko-fi.com/L3L026UOE"><img src="https://ko-fi.com/img/githubbutton_sm.svg">

Development will be continued with this and will forever stay public and free.

Copyright (c) 2020-2025 Damian Slocombe
