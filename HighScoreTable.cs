using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreTable : MonoBehaviour
{
    public SA.GameManager GameManager;
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<Transform> HighScoreEntryTransformList;

    //setting a reference for the template and the container which will be passed on to the awake function.
    
    private void Awake()
    {
        entryContainer = transform.Find("HighScoreEntryContainer");
        entryTemplate = entryContainer.Find("HighScoreEntryTemplate");
        //These are the references
        entryTemplate.gameObject.SetActive(false);
        //Hides the template
        
        
        string jsonString = PlayerPrefs.GetString("HighScoreTable");
        HighScores HighScores = JsonUtility.FromJson<HighScores>(jsonString);
        
        //Sorting Algorithm-bubble sort.
        for (int i = 0; i < HighScores.HighScoreEntryList.Count; i++)
        {
            for (int j = i + 1;j < HighScores.HighScoreEntryList.Count; j++)
            {
                if (HighScores.HighScoreEntryList[j].score> HighScores.HighScoreEntryList[i].score)
                {
                    //swap
                    HighScoreEntry tmp = HighScores.HighScoreEntryList[i];
                    HighScores.HighScoreEntryList[i] = HighScores.HighScoreEntryList[j];
                    HighScores.HighScoreEntryList[j] = tmp;
                }
            }
        }
        
        HighScoreEntryTransformList = new List<Transform>();
        foreach (HighScoreEntry HighScoreEntry in HighScores.HighScoreEntryList)
        {
            CreateHighScoreEntryTransfrom(HighScoreEntry, entryContainer, HighScoreEntryTransformList);
        }
       
    }
    private void CreateHighScoreEntryTransfrom(HighScoreEntry HighScoreEntry,Transform container,List<Transform> transformList)
    {
        float templateHeight = 40f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
            default:
                rankString = rank + "TH"; break;

            case 1: rankString = "1ST"; break;
            case 2: rankString = "2ND"; break;
            case 3: rankString = "3RD"; break;

        }
        entryTransform.Find("positions").GetComponent<Text>().text = rankString;
        //Adding Score
        int score = HighScoreEntry.score;
        entryTransform.Find("Scores").GetComponent<Text>().text = score.ToString();
        //Adding username
        string name = HighScoreEntry.name;
        entryTransform.Find("Usernames").GetComponent<Text>().text = name;
        transformList.Add(entryTransform);
    }
    private void AddHighScoreEntry(int score,string name)
    {
        //Creating HighScore entry
        HighScoreEntry highScoreEntry = new HighScoreEntry { score = score, name = name };
        //Load saved HighScores.
        string jsonString = PlayerPrefs.GetString("HighScoreTable");
        HighScores HighScores = JsonUtility.FromJson<HighScores>(jsonString);
        //Add new Entry
        HighScores.HighScoreEntryList.Add(highScoreEntry);
        //Save updated HighSores
        string json = JsonUtility.ToJson(HighScores);
        PlayerPrefs.SetString("HighScoreTable", json);
        PlayerPrefs.Save();
    }
    private class HighScores
    {
        public List<HighScoreEntry> HighScoreEntryList;
    }
    [System.Serializable]
    private class HighScoreEntry
    {
        public int score;
        public string name;
    }
}


