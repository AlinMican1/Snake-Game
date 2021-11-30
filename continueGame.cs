using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEngine.UI;
using SA;
using UnityEngine.Events;

public class continueGame : MonoBehaviour
{
    public SA.GameManager GameManager;
    public float currentTime = 0f;
    public int startingTime = 120;
    [SerializeField] Text countdownText;
    public UnityEvent onGameOver;
    public UnityEvent onWinner;
    
    public void Scene3()
    {
        SceneManager.LoadScene("GameStart");
    }
    public void Start()
    {
          
        currentTime = startingTime;
        
    }
    public void Update()
    {
        startingTime = 120;
        currentTime = 120f;
        //GameManager.GetInput();
        currentTime -= 1 * Time.deltaTime;
        countdownText.text = currentTime.ToString("0");
        if (currentTime <= 0)
        {
            currentTime = 0;
            onGameOver.Invoke();
        }
    }
    public void Moveplayer2 ()
    {
        GameManager.MovePlayer();
         
    }
    
}
