using UnityEngine;
using System.Collections;
using UnityEditor;

public class RefreshMaterialShader 
{


//    [MenuItem("builtin/Refresh Material Shader", false, 501)]
    public static void RefreshMat() {
        var guids = AssetDatabase.FindAssets("t:Material");
        foreach (var guid in guids) {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.ToLower().EndsWith("mat")) {
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat && mat.shader) {
                    Debug.LogFormat( "{0}\n{1}\n{2}\n{3}\n", path, mat.shader.name,
                        mat.shader.GetInstanceID(),
                        Shader.Find(mat.shader.name).GetInstanceID());
                    mat.shader = Shader.Find(mat.shader.name);
                }
            }
        }
    }
}