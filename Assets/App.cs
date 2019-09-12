using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class App : MonoBehaviour {
    private void Awake(){
        print("START");
        StartCoroutine(UpdateWeb());
    }
    private Dictionary<int,GameObject> ens = new Dictionary<int,GameObject>();
    public GameObject[] prefabs;
    public float scale = 10f;
    private IEnumerator UpdateWeb(){
        while(enabled){
            var rq = UnityWebRequest.Get("http://localhost:58000");
            yield return rq.SendWebRequest();
            if(rq.isNetworkError || rq.isHttpError){
                print("error");
                continue;
            }
            var jObject = JObject.Parse(rq.downloadHandler.text);
            foreach(var l in (JObject)jObject["Entities"]){
                int key = int.Parse(l.Key);
                JObject j = (JObject)l.Value;
                float x = (float) j["Pos"]["X"];
                float y = (float) j["Pos"]["Y"];
                float z = (float) j["Pos"]["Z"];
                Vector3 pos = new Vector3(x,z,y);
                pos *= scale;
                int id = (int) j["Id"];  
                GameObject en;
                if(id >= 0){
                    int typ = (int) j["Type"];
                    if(!ens.TryGetValue(id,out en)){                    
                        en = Instantiate(prefabs[typ],pos,Quaternion.identity);
                        en.name = (string)j["Id"];
                        ens[id] = en;
                    }
                    en.transform.position = pos;
                    // foreach(var n in j["Neighbors"]){  
                    //     Debug.DrawLine(pos,ens[(int)n].transform.position,Color.red,0.5f);
                    // }
                }else{
                    if(ens.TryGetValue(id,out en)){                    
                        Destroy(en);
                        ens.Remove(id);
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
}