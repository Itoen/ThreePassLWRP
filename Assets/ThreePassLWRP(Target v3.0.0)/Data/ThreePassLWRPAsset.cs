using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.LightweightPipeline;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#endif

namespace ThreePassLWRP
{
    public class ThreePassLWRPAsset : LightweightRenderPipelineAsset
    {
        protected override IRenderPipeline InternalCreatePipeline ()
        {
            var pipeline = new LightweightRenderPipeline(this);
            var fieldInfo = typeof(LightweightRenderPipeline).GetField("s_DefaultRendererSetup", BindingFlags.Static | BindingFlags.NonPublic);
            fieldInfo.SetValue(pipeline, new ThreePassLWRPSetup());
            return pipeline;
        }


#if UNITY_EDITOR
        [MenuItem("Assets/Create/Rendering/ThreePass LWRP Asset", priority = CoreUtils.assetCreateMenuPriority1)]
        static void CreateLightweightPipeline ()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, CreateInstance<CreateThirdPassLWRPAsset>(),
                "ThreePassLWRPAsset.asset", null, null);
        }

        class CreateThirdPassLWRPAsset : EndNameEditAction
        {
            public override void Action (int instanceId, string pathName, string resourceFile)
            {
                var instance = CreateInstance<ThreePassLWRPAsset>();
                var editorResources = LoadResourceFile<LightweightRenderPipelineEditorResources>();
                var editorResourcesFieldInfo = typeof(LightweightRenderPipelineAsset).GetField("m_EditorResourcesAsset", BindingFlags.NonPublic | BindingFlags.Instance);
                editorResourcesFieldInfo.SetValue(instance, editorResources);

                var resourcesAsset = LoadResourceFile<LightweightRenderPipelineResources>();
                var resourcesAssetFieldInfo = typeof(LightweightRenderPipelineAsset).GetField("m_ResourcesAsset", BindingFlags.NonPublic | BindingFlags.Instance);
                resourcesAssetFieldInfo.SetValue(instance, resourcesAsset);
                AssetDatabase.CreateAsset(instance, pathName);
            }
        }

        static T LoadResourceFile<T> () where T : ScriptableObject
        {
            T resourceAsset = null;
            var fieldInfoPathProject = typeof(LightweightRenderPipelineAsset).GetField("s_SearchPathProject", BindingFlags.Static | BindingFlags.NonPublic);
            var searchPathProject = (string)fieldInfoPathProject.GetValue(null);
            var guids = AssetDatabase.FindAssets(typeof(T).Name + " t:scriptableobject", new[] { searchPathProject });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                resourceAsset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (resourceAsset != null)
                    break;
            }

            // There's currently an issue that prevents FindAssets from find resources withing the package folder.
            if (resourceAsset == null)
            {
                var fieldInfoPathPackage = typeof(LightweightRenderPipelineAsset).GetField("s_SearchPathPackage", BindingFlags.Static | BindingFlags.NonPublic);
                var searchPathPackage = (string)fieldInfoPathPackage.GetValue(null);
                string path = searchPathPackage + "/LWRP/Data/" + typeof(T).Name + ".asset";
                resourceAsset = AssetDatabase.LoadAssetAtPath<T>(path);
            }
            return resourceAsset;
        }
#endif
    }
}