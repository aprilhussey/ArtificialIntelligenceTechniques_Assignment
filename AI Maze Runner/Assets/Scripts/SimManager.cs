using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimManager : MonoBehaviour
{
    public static SimManager Instance;

    private FiniteStateMachine currentFSM;
    private BehaviourTreeScript currentBT;

    private List<int> fsmDecisionsTaken = new List<int>();
    private List<float> fsmTimeTaken = new List<float>();
    private List<int> btDecisionsTaken = new List<int>();
    private List<float> btTimeTaken = new List<float>();

    [SerializeField]
    private UnityEngine.Object loadingScene;

    private bool done = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        GetCurrentFSMAndBT();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GetCurrentFSMAndBT();
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFSMAndBT();
        if (GoalReached() && !done)
        {
            if (SceneManager.GetActiveScene().name == "Level1")
            {
                OutputAndStoreCurrentData();
                LoadScene("Level2");
            }
            else if (SceneManager.GetActiveScene().name == "Level2")
            {
                OutputAndStoreCurrentData();
                LoadScene("Level3");
            }
            else if (SceneManager.GetActiveScene().name == "Level3")
            {
                // Print Level 3
                OutputAndStoreCurrentData();
                // Print all final data
                OutputAllData();
                done = true;
            }
        }
    }

    private void GetCurrentFSMAndBT()
    {
        GameObject fsmObject = GameObject.Find("FSM NPC");
        if (fsmObject != null)
        {
            currentFSM = fsmObject.GetComponent<FiniteStateMachine>();
        }
        GameObject btObject = GameObject.Find("BT NPC");
        if (btObject != null)
        {
            currentBT = btObject.GetComponent<BehaviourTreeScript>();
        }
    }

    private void OutputAllData()
    {
        Debug.Log("All FSM Decisions: " + "[" + string.Join(",", fsmDecisionsTaken) + "]");
        Debug.Log("All FSM Time taken: " + "[" + string.Join(",", fsmTimeTaken) + "]");
        Debug.Log("All BT Decisions: " + "[" + string.Join(",", btDecisionsTaken) + "]");
        Debug.Log("All BT Time taken: " + "[" + string.Join(",", btTimeTaken) + "]");
    }

    private void OutputAndStoreCurrentData()
    {
        Debug.Log(SceneManager.GetActiveScene().name + " FSM Decisions: " + currentFSM.decisionsMade);
        Debug.Log(SceneManager.GetActiveScene().name + " FSM Time taken: " + currentFSM.finalTime);
        Debug.Log(SceneManager.GetActiveScene().name + " BT Decisions: " + currentBT.decisionsMade);
        Debug.Log(SceneManager.GetActiveScene().name + " BT Time taken: " + currentBT.finalTime);
        fsmDecisionsTaken.Add(currentFSM.decisionsMade);
        fsmTimeTaken.Add(currentFSM.finalTime);
        btDecisionsTaken.Add(currentBT.decisionsMade);
        btTimeTaken.Add(currentBT.finalTime);
    }

    private bool GoalReached()
    {
        return currentFSM.currentState == FiniteStateMachine.State.GoalReached && currentBT.EnoughCoinsCollected();
    }

    private void LoadScene(string sceneName)
    {
        if (loadingScene != null)
        {
            SceneManager.LoadSceneAsync(loadingScene.name);
        }
        StartCoroutine(LoadNextScene(sceneName));
    }

    private IEnumerator LoadNextScene(string nextScene)
    {
        AsyncOperation AsyncLoading = SceneManager.LoadSceneAsync(nextScene);

        while (!AsyncLoading.isDone) // Add extra check is you want to wait for certain animations or feature to be done before the new scene loads
        {
            yield return null;
        }
    }
}
