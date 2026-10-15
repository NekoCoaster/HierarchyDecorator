using System;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEditor;
using HierarchyDecorator;
public static class IconAssignmentsValidation {
 static BindingFlags Flags=BindingFlags.Static|BindingFlags.NonPublic;
 static void Check(bool value,string message){if(!value)throw new Exception(message);}
 public static void Run(){
  var assembly=typeof(Settings).Assembly;
  var registry=assembly.GetType("HierarchyDecorator.CustomComponentIcons");
  var find=registry.GetMethod("Find",Flags);
  Func<string,Texture2D> lookup=n=>(Texture2D)find.Invoke(null,new object[]{n});
  string[] names={"VRCSceneDescriptor","VRCContactSender","VRCContactReceiver","VRCAvatarDescriptor","PipelineManager","VRCPhysBone","VRCPhysBoneCollider","GestureManager","LyumaAv3Osc","AudioLink","AudioLinkController","AudioLinkControllerV0","AudioLinkZone","Missing"};
  foreach(var name in names){var image=lookup(name);Check(image!=null,"Missing mapping: "+name);Check(image.name.EndsWith("#60dae6"),"No tint: "+name);Check(ReferenceEquals(image,lookup(name)),"Not cached");}
  foreach(var name in new[]{"QvPen_PenManager","LTCGI_Controller","LTCGI_Screen","LyumaAv3Runtime","LyumaAv3Emulator","LyumaAv3Menu","GestureManagerAv3Menu"}) Check(lookup(name)==null,"Dropped mapping remains: "+name);
  var source=AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.nekocoaster.hierarchydecorator/HierarchyDecorator/CustomIcons/Icons8/icons8-questions-48.png");
  var before=source.GetPixels();var after=lookup("Missing").GetPixels();
  for(int i=0;i<before.Length;i++){
   Check(Mathf.Abs(after[i].a-before[i].a)<.005f,"Alpha changed");
   Check(Mathf.Abs(after[i].r-before[i].r*96/255f)<.005f && Mathf.Abs(after[i].g-before[i].g*218/255f)<.005f && Mathf.Abs(after[i].b-before[i].b*230/255f)<.005f,"Tint mismatch");
  }
  Check(lookup("VRCFury")==AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.nekocoaster.hierarchydecorator/HierarchyDecorator/CustomIcons/VRCFury/VRCFury.png"),"VRCFury tinted or changed");
  var hues=assembly.GetType("HierarchyDecorator.ConstraintIconHues");var resolveHue=hues.GetMethod("Resolve",Flags);
  var settings=new ComponentData();settings.constraintHueShifts[0]=90;
  var original=new GUIContent("Aim",Texture2D.whiteTexture,"tooltip");
  Check(ReferenceEquals(original,resolveHue.Invoke(null,new object[]{typeof(UnityEngine.Animations.AimConstraint),original,settings})),"Native Unity icon changed");
  var module=AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("IconMappingFixtures"),AssemblyBuilderAccess.Run).DefineDynamicModule("Fixtures");
  var vrc=module.DefineType("VRC.SDK3.Dynamics.Constraint.Components.VRCAimConstraint",TypeAttributes.Public,typeof(MonoBehaviour)).CreateType();
  var shifted=(GUIContent)resolveHue.Invoke(null,new object[]{vrc,original,settings});Check(shifted.image!=original.image && shifted.tooltip=="tooltip","VRC hue not applied");
  var udon=module.DefineType("UdonSharp.UdonSharpBehaviour",TypeAttributes.Public,typeof(MonoBehaviour)).CreateType();
  var audio=module.DefineType("Example.AudioLink",TypeAttributes.Public,udon).CreateType();
  var resolve=assembly.GetType("HierarchyDecorator.ComponentIconResolver").GetMethod("Resolve",Flags);
  Check(resolve.Invoke(null,new object[]{audio,null,Texture2D.whiteTexture})==lookup("AudioLink"),"AudioLink lost to U#");
  var held=lookup("Missing");registry.GetMethod("Refresh",Flags).Invoke(null,null);Check(held==null,"Tint cleanup failed");
  Debug.Log("ICON_ASSIGNMENTS_CHECKS_PASSED");
 }
}
