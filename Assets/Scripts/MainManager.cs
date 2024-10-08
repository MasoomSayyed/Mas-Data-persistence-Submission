using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{

    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text highscoretext;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;

    private bool m_GameOver = false;

    [SerializeField] private TextMeshProUGUI currenPlayer;

    private void Awake()
    {
        LoadScore();
    }
    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(0);
        SaveScore();
    }

    [System.Serializable]
    class SaveData
    {
        public string score;
    }

    public void SaveScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData save = JsonUtility.FromJson<SaveData>(json);
            int currentHighScore;
            int newScore;

            // Try to parse scores
            if (int.TryParse(save.score.Split(':').Last().Trim(), out currentHighScore) && int.TryParse(ScoreText.text.Split(':').Last().Trim(), out newScore))
            {
                // Only save if new score is higher
                if (newScore > currentHighScore)
                {
                    SaveData newSave = new SaveData();
                    newSave.score = ScoreText.text;
                    string jsonNew = JsonUtility.ToJson(newSave);
                    File.WriteAllText(Application.persistentDataPath + "/savefile.json", jsonNew);
                }
            }
            else
            {
                // Handle invalid score format
            }
        }
        else
        {
            // File doesn't exist, save score directly
            SaveData save = new SaveData();
            save.score = ScoreText.text;
            string json = JsonUtility.ToJson(save);
            File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        }
    }

    public void LoadScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData save = JsonUtility.FromJson<SaveData>(json);
            highscoretext.text = save.score;
        }
        else
        {
            // File doesn't exist, initialize high score
            highscoretext.text = "Score : 0";
        }
    }
}


