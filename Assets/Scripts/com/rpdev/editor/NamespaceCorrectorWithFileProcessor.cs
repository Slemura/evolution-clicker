//Assets/Editor/KeywordReplace.cs

using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniTools;

namespace com.rpdev.editor {

    
    public class NamespaceCorrector {

        public static void CorrectNamespace(FilePathProcessor.ComplexFilePaths paths) {

            string file = System.IO.File.ReadAllText(paths.full_path);

            

            /*if (file.IndexOf("editor", StringComparison.Ordinal) > -1) {

                file = file.Replace("editor", GetNamespaceForPath(paths.origin_path));

            } else*/ 
            
            if (file.IndexOf("namespace", StringComparison.Ordinal) > -1) {

                int start_of_namespace = file.IndexOf("namespace", StringComparison.Ordinal);
                int end_of_namespace   = file.IndexOf("{", StringComparison.Ordinal);
                string old_namespace   = file.Substring(start_of_namespace, end_of_namespace - start_of_namespace);

                Debug.Log("Old Namespace [" + old_namespace + "]");

                file = file.Replace(old_namespace,
                                    "namespace " + GetNamespaceForPath(paths.origin_path).ToLower() + " ");
            } else {
                var lines = file.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);

                string ready = "";

                for (var i = 0; i < lines.Length; i++) {
                    if (i < 3) {
                        ready += (i == 0 ? ""  : System.Environment.NewLine) + lines[i];
                    } else {
                        ready += System.Environment.NewLine + "\t" + lines[i];
                    }
                    if (i == 3) {
                        ready += System.Environment.NewLine + "namespace " + GetNamespaceForPath(paths.origin_path) + " {" + System.Environment.NewLine;
                    }
                }

                ready += System.Environment.NewLine + "}";
                file = ready;
            }

            System.IO.File.WriteAllText(paths.full_path, file);
            AssetDatabase.Refresh();
        }

        protected static string GetNamespaceForPath(string path) {
            
            string income = path;
            
            int lastIndex = income.LastIndexOf('/');
            income = income.Remove(lastIndex);

            string[] bit = income.Split('/');
            string result = "";

            for (int i = 2; i < bit.Length; i++) {
                result += "." + bit[i];
            }
            
            if(result.Length > 0)
                result = result.Remove(0, 1);

            return result;
        }
    }

    public class FilePathProcessor {

        public class ComplexFilePaths {
            public string origin_path;
            public string full_path;
        }

        public static ComplexFilePaths CheckAndGetPath(string path) {
            string origin_path    = path;
            string modified_path = path.Replace(".meta", "");

            int file_type_separator_index = modified_path.LastIndexOf(".", StringComparison.Ordinal);

            if (file_type_separator_index < 0) return null;

            string file_type = modified_path.Substring(file_type_separator_index);

            if (file_type != ".cs" && file_type != ".js" && file_type != ".boo") return null;

            int full_path_index = Application.dataPath.LastIndexOf("Assets", StringComparison.Ordinal);
            string full_path = Application.dataPath.Substring(0, full_path_index) + modified_path;

            //Debug.Log(" origin path " + origin_path);
            //Debug.Log(" full path " + full_path);

            return new ComplexFilePaths() { origin_path = origin_path, full_path = full_path};
        }
    }


    public class FileModificationPreProcessor : UnityEditor.AssetModificationProcessor {
        public static void OnWillCreateAsset(string path) {
            //Debug.Log("create asset " + path);
            if (path.Contains("Assets/Scripts/")) {
                //Debug.Log("Have Scripts " + path);
                FilePathProcessor.ComplexFilePaths paths = FilePathProcessor.CheckAndGetPath(path);
                if (paths != null) {
                    NamespaceCorrector.CorrectNamespace(paths);
                }
            }
        }
    }

    [InitializeOnLoad]
    public class FileModificationPostProcessor : AssetPostprocessor {
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            for (int i = 0; i < movedFromAssetPaths.Length; i++) {
                Debug.Log("Moved Asset from " + movedFromAssetPaths[i] + " to " + movedAssets[i]);

                if (movedAssets[i].Contains("Assets/Scripts/")) {
                    Debug.Log("Create Namespace");
                    FilePathProcessor.ComplexFilePaths paths = FilePathProcessor.CheckAndGetPath(movedAssets[i]);

                    if (paths != null) {
                        NamespaceCorrector.CorrectNamespace(paths);
                    }
                }
            }
        }
    }
}

