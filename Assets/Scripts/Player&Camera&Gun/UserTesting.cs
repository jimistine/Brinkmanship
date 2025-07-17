using System.Collections.Generic;
using UnityEngine;
using static GunManagerComponent;
using System.IO;
using System;


public class UserTesting : MonoBehaviour
{
    private static UserTesting instance;

    
    public List<EventInfo> eventInfo = new List<EventInfo>();
    //public List<GunEventInfo> gunStateInfo = new List<GunEventInfo>();
    public string fileName = "";

    [Serializable]
    public class EventInfo{
        public string eventType;
        public string eventName;
        public DateTime timeStamp;
    }
    // [Serializable]
    // public class GunEventInfo{
    //     public GunState gunState;
    //     public DateTime timeStamp;
    // }

   

    private void OnEnable()
    {
        OnGunStateChangeEvent += _OnGunFired;
    }

    private void OnDisable()
    {
        OnGunStateChangeEvent -= _OnGunFired;
    }

    private void OnApplicationQuit()
    {
        WriteData();
    }

    private void Awake(){
    }

    void Start(){
        fileName = Application.dataPath + "/BrGameplayInfo.csv";
    }

    private void Update()
    {
        
        // if(Input.GetKeyDown(KeyCode.Space)){
        //     WriteData();
        // }
    }

    private void _OnGunFired(GunState oldState, GunState newState)
    {
        
        // EventInfo gunInfo = new EventInfo();
        // if(newState == GunState.Raised)
        //     gunInfo.eventName = "raised";
        // else if(newState == GunState.Holstered){
        //     gunInfo.eventName = "holstered";
        // }
        // else{
        //     gunInfo.eventName = newState.ToString();
        // }

        // gunInfo.eventType = "GunState";
        // gunInfo.timeStamp = DateTime.Now;
        // eventInfo.Add(gunInfo);
        // WriteData();
    }

    

    public void AddNodeInfo(string nodeName, DateTime time){
        EventInfo newInfo = new EventInfo();
        newInfo.eventType = "NodeStart";
        newInfo.eventName = nodeName;
        newInfo.timeStamp = time;
        eventInfo.Add(newInfo);
        //WriteData();
    }

    private void WriteData()
    {

        // TextWriter tw = new StreamWriter(fileName, false);
        // tw.WriteLine("Event Type, Event Name, Timestamp");
        
        // tw.Close();
        TextWriter tw = new StreamWriter(fileName, true);

        foreach(EventInfo e in eventInfo){
            //Debug.Log(e.eventType + ", " + e.eventName + ", " + e.timeStamp);
            tw.WriteLine(e.eventType + ", " + e.eventName + ", " + e.timeStamp);
        }
        tw.Close();

//  MAC
        if(SystemInfo.operatingSystem.Contains("Mac")){
            string path = Path.Combine(Application.persistentDataPath);
            path = Path.Combine(path, "NodesVisited.csv");

            //Create Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            using (StreamWriter writer = new StreamWriter(path, true))
            {
                //writer.WriteLine("testfile");
                foreach(EventInfo e in eventInfo){
                //Debug.Log(e.eventType + ", " + e.eventName + ", " + e.timeStamp);
                writer.WriteLine(e.eventType + ", " + e.eventName + ", " + e.timeStamp);
                }
            }
        }
    }
}
