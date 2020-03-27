using System.Collections;using System.Collections.Generic;
using UnityEngine;using UnityEngine.UI;
using System;using SimpleJSON;
using UnityEngine.Networking;using UnityEngine.SceneManagement;

public class Spisok : MonoBehaviour {

    public RectTransform prefab;
    public RectTransform content;

    void Start()
    {
        StartCoroutine(GetJson("0", results => OnReceivedModels(results)));
    }

    public IEnumerator GetJson(string id,System.Action<TestItemModel[]> callback){
        WWWForm form = new WWWForm();form.AddField("id", id);
        using (UnityWebRequest www = UnityWebRequest.Post("https://playklin.000webhostapp.com/flow/GetALLspisok.php",form)){
        yield return www.SendWebRequest();if (www.isNetworkError || www.isHttpError) { Debug.Log(www.error); }else{
            //Debug.Log("WWW: " + www.downloadHandler.text);
            TestItemModel[] mList = JsonHelper.getJsonArray<TestItemModel>(www.downloadHandler.text);
            callback(mList);
            }
        }
    }

    void OnReceivedModels (TestItemModel[] models)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (var model in models)
        {
            var instance = GameObject.Instantiate(prefab.gameObject) as GameObject;
            instance.transform.SetParent(content, false);
            InitializeItemView(instance, model);
        }
    }

    void InitializeItemView (GameObject viewGameObject, TestItemModel model)
    {
        TestItemView view = new TestItemView(viewGameObject.transform);
        
        view.name.text = model.name;


        // ВОТ ЗДЕСЬ ВСЯ БЕДА или в JsonHELPER

        //if(model.img.Length > 100){
                 //byte[] Bytes = System.Convert.FromBase64String (model.img);
                 //Texture2D texture = new Texture2D(1,1);
                 //texture.LoadImage (Bytes);
                 //view.img.texture = texture;}
        
    }

    public class TestItemView // view Привязываем данные из таблицы к префабу
    {
        public Text name;
        public RawImage img;

        public TestItemView (Transform rootView)
        {
            name = rootView.Find("Name").GetComponent<Text>();
            img = rootView.Find("RawImage").GetComponent<RawImage>();

        }
    }

    [System.Serializable]
    public class TestItemModel // model колонки забираем из таблицы базы данных 
    {
        public string name;
        public string img;
    }

  #region JSONHELPER -----

    public class JsonHelper
    {
        public static T[] getJsonArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        public static string arrayToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.array = array;
            return JsonUtility.ToJson(wrapper);
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }
  #endregion
}

/**
<?php

$sql ="SELECT name,img FROM flow";

$result = $conn->query($sql);

if ($result->num_rows > 0) {
    $rows = array();
    while($row = $result->fetch_assoc()){
        $rows[] = $row;
    }
    echo json_encode($rows);
        
} else {
    echo "нет данных";
}
$conn->close();
?>
*/







// это так на всякий случай
/**
    public RawImage ttyy;
    public GameObject butImage;
    private String imageString = "";
   
// выводим картинку из БД
    IEnumerator GetPIC(string picphp){ WWWForm form = new WWWForm();
        form.AddField("pic", picphp); // correct
        using (UnityWebRequest www = UnityWebRequest.Post("https://playklin.000webhostapp.com/yk/GetIMG.php",form))
        {yield return www.SendWebRequest(); if (www.isNetworkError || www.isHttpError) { Debug.Log(www.error); }
         else{ //Debug.Log(www.downloadHandler.text);
             imageString = (www.downloadHandler.text);
             if(imageString.Length > 100){
                 byte[] Bytes = System.Convert.FromBase64String (imageString);
                 Texture2D texture = new Texture2D(1,1);
                 texture.LoadImage (Bytes);
                 //GUI.DrawTexture(new Rect(200,20,440,440), texture, ScaleMode.ScaleToFit, true, 1f);
                 //ttyy.material.mainTexture = texture;
                 ttyy.texture = texture;
                 //butImage.SetActive(true);
             }else{
                 //butImage.SetActive(false);
             }
            }
        } // correct
    }
*/

