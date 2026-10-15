using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using HierarchyDecorator;
public static class IconTintValidation {
 static void Check(bool b,string m){if(!b)throw new Exception(m);}
 public static void Run(){
  var a=typeof(Settings).Assembly;var registry=a.GetType("HierarchyDecorator.CustomComponentIcons");
  var find=registry.GetMethod("Find",BindingFlags.NonPublic|BindingFlags.Static);var refresh=registry.GetMethod("Refresh",BindingFlags.NonPublic|BindingFlags.Static);
  var settings=(Settings)a.GetType("HierarchyDecorator.HierarchyDecorator").GetProperty("Settings").GetValue(null);
  var saved=(Color[])settings.Components.icons8Tints.Clone();
  Func<string,Texture2D> get=n=>(Texture2D)find.Invoke(null,new object[]{n});
  var temp=ScriptableObject.CreateInstance<Settings>();
  try{
   Check(temp.Components.icons8Tints.Length==11,"Tint count");
   foreach(var c in temp.Components.icons8Tints)Check(Mathf.Abs(c.r-96/255f)<.001f && c.a==1,"Default tint");
   settings.Components.icons8Tints[0]=Color.red;settings.Components.icons8Tints[1]=Color.green;refresh.Invoke(null,null);
   var red=get("VRCSceneDescriptor");var green=get("VRCContactSender");
   Check(red!=green,"Shared tint state");
   foreach(var c in red.GetPixels())Check(c.g<.005f && c.b<.005f,"Wrong red tint");
   foreach(var c in green.GetPixels())Check(c.r<.005f && c.b<.005f,"Wrong green tint");
   Check(ReferenceEquals(red,get("VRCSceneDescriptor")),"Cache reuse");
   EditorJsonUtility.FromJsonOverwrite(EditorJsonUtility.ToJson(settings),temp);Check(temp.Components.icons8Tints[0]==Color.red && temp.Components.icons8Tints[1]==Color.green,"Tint persistence");
   settings.Components.icons8Tints[0]=Color.white;refresh.Invoke(null,null);Check(red==null,"Old cache survived");
   var white=get("VRCSceneDescriptor");Check(white!=null,"Refresh failed");
   Undo.RecordObject(settings,"Tint test");settings.Components.icons8Tints[0]=Color.blue;Undo.FlushUndoRecordObjects();Undo.PerformUndo();Check(settings.Components.icons8Tints[0]==Color.white,"Undo failed");
   Debug.Log("ICON_TINT_CHECKS_PASSED");
  }finally{settings.Components.icons8Tints=saved;refresh.Invoke(null,null);UnityEngine.Object.DestroyImmediate(temp);}
 }
}
