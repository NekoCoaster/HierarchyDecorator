using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using HierarchyDecorator;
public static class ConstraintHueValidation {
 static Type T=typeof(Settings).Assembly.GetType("HierarchyDecorator.ConstraintIconHues");
 static object Call(string n,params object[] a) => T.GetMethod(n,BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,a);
 static void Check(bool ok,string s){if(!ok)throw new Exception(s);}
 public static void Run(){
  var c=(Color)Call("Shift",new Color(1,0,0,.37f),120f);
  Check(c.g>.99f && c.r<.01f && c.b<.01f && Mathf.Abs(c.a-.37f)<.001f,"RGB hue or alpha");
  var grey=(Color)Call("Shift",new Color(.4f,.4f,.4f,.2f),70f);
  Check(Mathf.Abs(grey.r-.4f)<.001f && Mathf.Abs(grey.g-.4f)<.001f,"Grey changed");
  Type[] types={typeof(UnityEngine.Animations.AimConstraint),typeof(UnityEngine.Animations.LookAtConstraint),typeof(UnityEngine.Animations.ParentConstraint),typeof(UnityEngine.Animations.PositionConstraint),typeof(UnityEngine.Animations.RotationConstraint),typeof(UnityEngine.Animations.ScaleConstraint)};
  for(int i=0;i<6;i++){
   Check((int)Call("Index",types[i])==i,"Mapping");
   var original=(Texture)Call("Get",i,0f); Check(original!=null,"Original missing");
   var shifted=(Texture2D)Call("Get",i,90f); Check(shifted!=null && shifted!=original,"Shift missing");
   Check(ReferenceEquals(shifted,Call("Get",i,90f)),"Cache miss");
   Check(ReferenceEquals(shifted,Call("Get",i,120f)),"Slider allocated texture");
   Check(ReferenceEquals(original,Call("Get",i,0f)),"Reset failed");
   var cache=(Array)T.GetField("Cache",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null);
   var entry=cache.GetValue(i);
   var before=(Color[])entry.GetType().GetField("Pixels",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(entry);
   var after=shifted.GetPixels();
   for(int j=0;j<after.Length;j++){
    Color.RGBToHSV(before[j],out float h0,out float s0,out float v0);
    Color.RGBToHSV(after[j],out float h1,out float s1,out float v1);
    Check(Mathf.Abs(before[j].a-after[j].a)<.005f && Mathf.Abs(v0-v1)<.005f && Mathf.Abs(s0-s1)<.02f,"Icon SV/alpha changed");
   }
   Check(shifted.hideFlags==HideFlags.HideAndDontSave,"Persistence");
   var active=RenderTexture.active; Call("Get",i,-90f); Check(RenderTexture.active==active,"Render target leaked");
  }
  Check((int)Call("Index",typeof(Transform))==-1,"Unrelated component matched");
  var s=ScriptableObject.CreateInstance<Settings>(); s.Components.constraintHueShifts[2]=72;
  var loaded=ScriptableObject.CreateInstance<Settings>(); EditorJsonUtility.FromJsonOverwrite(EditorJsonUtility.ToJson(s),loaded);
  Check(loaded.Components.constraintHueShifts[2]==72,"Persistence failed");
  var content=new GUIContent("test"); s.Components.constraintHueEnabled=false;
  Check(ReferenceEquals(content,Call("Resolve",types[0],content,s.Components)),"Disable failed");
  UnityEngine.Object.DestroyImmediate(s);UnityEngine.Object.DestroyImmediate(loaded);
  var held=(Texture2D)Call("Get",0,80f); Call("Clear"); Check(held==null,"Cleanup failed");
  Debug.Log("CONSTRAINT_HUE_CHECKS_PASSED");
 }
}
