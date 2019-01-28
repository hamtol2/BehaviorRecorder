using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using NPOI.SS.UserModel;
using System.Linq;
using System;

public class MotionSODataCreator : Editor
{
    public static string csvFolderPath = "Assets/Data/Editor/MotionCSV";
    public static string soOutputPath = "Assets/Data/MotionSOData";

    [MenuItem("MOCCA Motion/Create SO Data Files")]
    static void CreateMotionSOData()
    {
        string[] files = Directory.GetFiles(csvFolderPath, "*.csv");
        foreach (string file in files)
        {
            //Debug.Log(Path.GetFileNameWithoutExtension(file));

            string finalPath = soOutputPath + "/" + Path.GetFileNameWithoutExtension(file) + ".asset";

            
            MotionSequence motionSequence = AssetDatabase.LoadAssetAtPath<MotionSequence>(finalPath);
            if (motionSequence == null)
            {
                motionSequence = ScriptableObject.CreateInstance<MotionSequence>();
                AssetDatabase.CreateAsset(motionSequence, finalPath);
            }

            motionSequence.motionName = Path.GetFileNameWithoutExtension(file);
            motionSequence.sequence = new List<MotionFrameData>();

            string csvData = File.ReadAllText(file);
            string[] lines = csvData.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            List<float[]> frameDatas = new List<float[]>();
            foreach (string line in lines)
            {
                string[] csv = line.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                Debug.Log(motionSequence.motionName + ", " + csv[1]);
                float[] csvFloats = new float[]
                {
                    Convert.ToSingle(csv[0]),
                    Convert.ToSingle(csv[1]), Convert.ToSingle(csv[2]), Convert.ToSingle(csv[3]),
                    Convert.ToSingle(csv[4]), Convert.ToSingle(csv[5]), Convert.ToSingle(csv[6]),
                    Convert.ToSingle(csv[7]), Convert.ToSingle(csv[8]),
                };
                motionSequence.AddSequenceData(csvFloats);
            }

            
            //AssetDatabase.CreateAsset(motionSequence, finalPath);
            EditorUtility.SetDirty(motionSequence);
            AssetDatabase.SaveAssets();
        }
    }
}