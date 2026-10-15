using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;
using HierarchyDecorator;
public static class DismayIconsValidation {
 static void Check(bool ok,string label){if(!ok)throw new Exception(label);}
 public static void Run(){
  var settings=ScriptableObject.CreateInstance<Settings>();
  foreach(var h in settings.Components.constraintHueShifts)Check(h==-130,"New defaults");
  settings.Components.constraintHueShifts=new[]{0f,30f,60f,90f,120f,150f};
  var loaded=ScriptableObject.CreateInstance<Settings>();EditorJsonUtility.FromJsonOverwrite(EditorJsonUtility.ToJson(settings),loaded);
  for(int i=0;i<6;i++)Check(loaded.Components.constraintHueShifts[i]==i*30f,"Existing values overwritten");
  UnityEngine.Object.DestroyImmediate(settings);UnityEngine.Object.DestroyImmediate(loaded);
  var assembly=typeof(Settings).Assembly;var resolver=assembly.GetType("HierarchyDecorator.ComponentIconResolver");
  var resolve=resolver.GetMethod("Resolve",BindingFlags.Static|BindingFlags.NonPublic);
  Action clear=()=>resolver.GetMethod("ClearCache",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,null);
  var module=AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DismayTestFixtures"),AssemblyBuilderAccess.Run).DefineDynamicModule("Fixtures");
  var baseP=module.DefineType("DismayCustom.PCS.PCSComponentBase",TypeAttributes.Public,typeof(MonoBehaviour)).CreateType();
  var pcs=module.DefineType("OtherNamespace.InheritedPCS",TypeAttributes.Public,baseP).CreateType();
  var lms=module.DefineType("DismayCustom.LMS.LMSInstaller",TypeAttributes.Public,typeof(MonoBehaviour)).CreateType();
  var legacy=module.DefineType("DMCustom.LMS.Runtime.Installer",TypeAttributes.Public,typeof(MonoBehaviour)).CreateType();
  var unrelated=module.DefineType("Unrelated.LMSInstaller",TypeAttributes.Public,typeof(MonoBehaviour)).CreateType();
  var near=module.DefineType("DismayCustom.LMSOther.Installer",TypeAttributes.Public,typeof(MonoBehaviour)).CreateType();
  var fallback=EditorGUIUtility.IconContent("cs Script Icon").image;
  Func<Type,Texture> icon=t=>(Texture)resolve.Invoke(null,new object[]{t,null,fallback});
  string[] paths={"Assets/!Dismay Custom/Penetration Contact System/Assets/Icons/PCS Icon.png","Assets/!Dismay Custom/Lovestick Massages SFX/Assets/Icons/LMS Icon.png"};
  foreach(var p in paths)Check(!File.Exists(p),"Fixture asset already exists");
  var texture=new Texture2D(2,2);texture.SetPixels(new[]{Color.red,Color.blue,Color.white,Color.green});texture.Apply();
  try{
   Check(icon(pcs)==fallback && icon(lms)==fallback,"Absent artwork fallback");
   foreach(var p in paths){Directory.CreateDirectory(Path.GetDirectoryName(p));File.WriteAllBytes(p,texture.EncodeToPNG());}
   AssetDatabase.Refresh();clear();
   var pi=AssetDatabase.LoadAssetAtPath<Texture2D>(paths[0]);var li=AssetDatabase.LoadAssetAtPath<Texture2D>(paths[1]);
   Check(icon(pcs)==pi,"PCS base recognition");Check(icon(lms)==li && icon(legacy)==li,"LMS namespaces");
   Check(icon(unrelated)==fallback && icon(near)==fallback,"Unrelated or generic script overwritten");
   var script=AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/!Dismay Custom/Lovestick Massages SFX/Scripts/Runtime/DismayFolderFixture.cs");
   Check(script!=null && script.GetClass()!=null,"Folder fixture unavailable");
   Check((Texture)resolve.Invoke(null,new object[]{script.GetClass(),script,fallback})==li,"Installation-folder recognition");
  }finally{foreach(var p in paths)AssetDatabase.DeleteAsset(p);UnityEngine.Object.DestroyImmediate(texture);clear();}
  Check(icon(pcs)==fallback && icon(lms)==fallback,"Removal fallback");
  Debug.Log("DISMAY_ICON_CHECKS_PASSED");
 }
}
