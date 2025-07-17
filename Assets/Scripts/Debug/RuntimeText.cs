using UnityEngine;
using System.IO;
public class RuntimeText : MonoBehaviour
{
    public static void WriteString(string s)
    {
        string path = Application.persistentDataPath + "/usertesting.txt";
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(s);
        writer.Close();
        StreamReader reader = new StreamReader(path);
        //Print the text from the file
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }
    public static void ReadString()
    {
        string path = Application.persistentDataPath + "/usertesting.txt";
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }
}