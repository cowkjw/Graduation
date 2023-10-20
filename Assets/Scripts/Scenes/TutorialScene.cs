using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialScene : MonoBehaviour
{
    [Serializable]
    public class TutorialDialogue
    {
        public string CharacterName;
        public string DialogueText;
    }

    public List<TutorialDialogue> dialogues;
    public Dictionary<String,List<TutorialDialogue>> Test;

    [SerializeField]
    private Text _text;
    [SerializeField]
    private Text _npcNameText;
    private string currentText;
    public float textSpeed;
    int currentDialogueIndex = 0;
    int tutrorialProgress = 0;
    public bool result;
    string key;
    List<string> keys = new List<string>() {"Move","Attack","Skill"};

    void Start()
    {
        textSpeed = 0.1f;
        currentText = "";
        string filePath = Path.Combine(Application.dataPath, $"StreamingAssets/Data/TestDict.json");
        //string filePath = Path.Combine(Application.dataPath, $"StreamingAssets/Data/TutorialDialogue.json");
        string jsonText = File.ReadAllText(filePath);
        //dialogues = JsonConvert.DeserializeObject<List<TutorialDialogue>>(jsonText);
        Test = JsonConvert.DeserializeObject<Dictionary<string, List<TutorialDialogue>>>(jsonText);
        Debug.Log(Test.Count);

       // Managers.Input.OnTutorialResult += ShowNextDialogue;
       // WriteToJson();

    }

    public void ShowDialogue(string text)
    {
        StopAllCoroutines();
        StartCoroutine(TypeText(text));
    }

  

    public void ShowNextDialogue(bool result)
    {
            if (tutrorialProgress == 3)
            {
                SceneManager.LoadScene(0);
            }
        // 현재 다이아로그 인덱스가 진행중인 해당 튜토리얼의 전체 사이즈보다 크다면 return
        if (currentDialogueIndex >= Test[keys[tutrorialProgress]]?.Count)
        {
            // 모든 대사가 끝났을 때의 처리
            tutrorialProgress++;
            currentDialogueIndex = 0;
            return;
        }
        if(!result)
        {
            return;
        }
        
        TutorialDialogue dialogue = Test[keys[tutrorialProgress]][currentDialogueIndex];
        _npcNameText.text = dialogue.CharacterName;
        
        
        ShowDialogue(dialogue.DialogueText);

        currentDialogueIndex++;
    }
    private IEnumerator TypeText(string text)
    {
        currentText = "";
        foreach (char c in text)
        {
            currentText += c;
            _text.text = currentText;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    //void WriteToJson()
    //{
    //    dialogues.Add(new TutorialDialogue { CharacterName = "NPC", DialogueText = "Hello, adventurer!" });
    //    dialogues.Add(new TutorialDialogue { CharacterName = "NPC", DialogueText = "Welcome to the tutorial." });
    //    Test = new Dictionary<string, List<TutorialDialogue>>();
    //    Test.Add(0,dialogues);
    //    //dialogues.Add(new TutorialDialogue { CharacterName = "NPC", DialogueText = "Welcome to the tutorial." });

    //    string jsonText = JsonConvert.SerializeObject(Test);
    //    string filePath = Path.Combine(Application.dataPath, $"StreamingAssets/Data/TestDict.json");
    //    if (!File.Exists(filePath))
    //    {
    //        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
    //    }
    //    File.WriteAllText(filePath, jsonText);
    //}

}
