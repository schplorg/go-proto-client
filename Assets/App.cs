using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class App : MonoBehaviour {
    private void Awake(){
        print("START");
        StartCoroutine(UpdateWeb());
    }
    private GameObject[] ens = new GameObject[0];
    public GameObject prefab;
    private IEnumerator UpdateWeb(){
        while(enabled){
            var rq = UnityWebRequest.Get("http://localhost:58000");
            yield return rq.SendWebRequest();
            if(rq.isNetworkError || rq.isHttpError){
                print("error");
                continue;
            }
            var jObject = JObject.Parse(rq.downloadHandler.text);
            int count = (int)jObject["EntityCount"];
            if(ens.Length != count){
                foreach(var e in ens){
                    Destroy(e);
                }
                ens = new GameObject[count];                
            }
            int i = 0;
            foreach(var j in jObject["Entities"]){
                float x = (float) j["Pos"]["X"];
                float y = (float) j["Pos"]["Y"];
                float z = (float) j["Pos"]["Z"];
                Vector3 pos = new Vector3(x,z,y);
                if(ens[i] == null){
                    ens[i] = Instantiate(prefab,pos,Quaternion.identity);
                    ens[i].name = (string)j["Id"];
                }
                ens[i].transform.position = pos;
                i++;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}