using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpellingMinigameScript : MonoBehaviour
{
    private List<string> easySpellingList = new List<string> { "CAT", "DOG", "MOUSE", "KID", "BIRD", "FISH", "TREE", "HOUSE", "SCHOOL", "BOOK", "APPLE", "BANANA",
        "GRAPE", "BALL", "GAME", "PENCIL", "CHAIR", "TABLE", "RIVER", 
        "BRIDGE", "CLOUD", "RAIN", "SUN", "MOON", "STAR" };
    private List<string> intermediateSpellingList = new List<string> { "ANALYZE", "COMPLEX", "HYPOTHESIS", "VARIABLE", "FUNCTION", "EQUATION", "STRATEGY", "ARGUMENT",
        "EVIDENCE", "PREDICT", "CONCLUDE", "RESEARCH", "SCIENCE", "HISTORY", "GEOGRAPHY", "LITERATURE", "SYMBOLIC", "MYSTERY", "ECONOMY", "POLITICS", "CULTURE", 
        "COMMUNITY", "INNOVATE", "DISCOVER", "ABSTRACT" };
    private List<string> hardSpellingList = new List<string> { "ANALYZE", "ARTICULATE", "CONCEPTUAL", "CRITICAL", "DIVERSITY", "EVIDENCE", "HYPOTHESIS", "INTERPRET",
        "LOGIC", "NUANCED", "PARADIGM", "PERSPECTIVE", "PHENOMENON", "REFINE", "RELEVANCE", "RESEARCH", "SYNTHESIS", "THEORETICAL", "VALIDATE", "CONTEXT", "IMPLICATION",
        "RATIONAL", "SIGNIFICANT", "SYMBOLISM", "CRITIQUE" };
    private List<string> extremelyHardSpellingList = new List<string> { "ABSTRACT", "ANALYTICAL", "EPISTEMOLOGY", "METHODOLOGY", "ONTOLOGY", "HEURISTIC", "DIALECTIC", "HERMENEUTICS", "INTERDISCIPLINARY", "EXTRAPOLATE",
        "DISCOURSE", "SEMANTICS", "SYSTEMATIC", "SYNTHESIS", "PARADIGMATIC", "RATIONALITY", "CONTINGENCY", "NORMATIVE", "SUBSTANTIVE", "EXPERIMENTATION", "DISAMBIGUATION", "RECONCEPTUALIZE", "CRITIQUE", "INTERTEXTUALITY",
        "METAANALYSIS" };

    private List<List<string>> spellingLists;

    private List<string> currentSpellingList;

    [SerializeField] private TextMeshProUGUI chosenWord;
    // Change this to TMP_InputField for user input.
    [SerializeField] private TMP_InputField typedWordInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI timerTM;
    [SerializeField] private float secondsPassed = -1;
    [SerializeField] private float minutesPassed = -1;

    private int lastRandomWord;
    private int secondToLastRandomWord;

    private int spellingLevel = 0;
    private int wordsSpelledCounter = 0;
    private int wordsSpelledIncorrectly = 0;

    [SerializeField] private GameObject progressBarGO;
    private Vector3 progressBarTransformOG;

    [SerializeField] private int progressBottom;
    [SerializeField] private int progressTop;

    [SerializeField] private int wordsNeededToWin;

    [SerializeField] private float progressbarIncrementPerWord;

    [SerializeField] private TextMeshProUGUI wordsSpelledText;

    [SerializeField] private TextMeshProUGUI levelText;


    //Procedural Text Animation
    private float ySway;
    private float xSway;
    private float textSpeed;
    private Vector2 ogTextPos;

    [SerializeField] private float maxYSway;
    [SerializeField] private float maxXSway;

    [SerializeField] private float minTextSpeed;
    [SerializeField] private float maxTextSpeed;

    [SerializeField] private float textAnimWaitTime;
    [SerializeField] private float textAnimTimeMax;
    [SerializeField] private float textAnimTimeMin;

    //Animator

    [SerializeField] private Animator animator;

    private enum CurrentLevel
    {
        Easy,
        Intermediate,
        Hard,
        Legendary
    }

    private CurrentLevel currentLevel;

    private bool haswon = false;

    [SerializeField] private Canvas gameCanvas;
    [SerializeField] private GameObject winScreen;



    private void Start()
    {
        progressBottom = -Display.main.systemHeight - 50;
        progressTop = Screen.height + 50;

        winScreen.SetActive(false);

        levelText.text = currentLevel.ToString();

        progressBarTransformOG = progressBarGO.transform.localPosition;

        ogTextPos = timerTM.rectTransform.localPosition;
        ySway = Random.Range(-maxYSway, maxYSway);
        xSway = Random.Range(-maxXSway, maxXSway);
        textSpeed = Random.Range(minTextSpeed, maxTextSpeed);
        textAnimWaitTime = Random.Range(textAnimTimeMin, textAnimTimeMax);


        currentSpellingList = easySpellingList;
        currentLevel = (CurrentLevel)(spellingLevel);

        spellingLists = new List<List<string>> { easySpellingList, intermediateSpellingList , hardSpellingList, extremelyHardSpellingList };


        progressBarGO.transform.localPosition = new Vector3(progressBarGO.transform.localPosition.x, progressBottom, progressBarGO.transform.localPosition.z);
        progressbarIncrementPerWord = ((Mathf.Abs(progressBottom) + Mathf.Abs(progressTop)) / wordsNeededToWin) ;


        wordsSpelledText.text = wordsSpelledCounter.ToString() + "/" + wordsNeededToWin.ToString();

        secondsPassed = 0;
        minutesPassed = 0;

        DisplayRandomWord();
        // Use a lambda to capture the current text from the input field when the button is clicked.
        submitButton.onClick.AddListener(() => CheckIfSpellingIsRight(typedWordInput.text));

        StartCoroutine(WaitAndSwitchRandomTextAnimSetting(textAnimWaitTime));
        
    }

    private void Update()
    {

        if(haswon)
        {
            return;
        }
        DisplayTheTime();
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CheckIfSpellingIsRight(typedWordInput.text);
        }

        TimerAnimation();

        

    }

    private void DisplayTheTime()
    {
        secondsPassed += Time.deltaTime;
        minutesPassed = secondsPassed / 60;
        minutesPassed = Mathf.FloorToInt(minutesPassed);


        string minutesPassedText = minutesPassed.ToString();
        string secondsPassedText = secondsPassed.ToString();

        minutesPassedText = minutesPassed.ToString();
        secondsPassedText = (secondsPassed % 60).ToString();


        string[] timePassedString = secondsPassedText.Split(".");

        int secondpassedInt = int.Parse(timePassedString[0]);
        string timerText = "";
        if (secondpassedInt < 10)
        {
            timerText = minutesPassedText + ":0" + timePassedString[0];
        }
        else
        {
            timerText = minutesPassedText + ":" + timePassedString[0];
        }


        timerTM.text = timerText;
    }

    private void CheckIfSpellingIsRight(string userInput)
    {
        userInput = userInput.ToUpper();
        string chosen = chosenWord.text.ToUpper();
        userInput = userInput.Trim();

        if (userInput == chosen)
        {
            
            Debug.Log("true");
            chosenWord.color = Color.green;
            //backgroundImage.color = Color.green;
            progressBarGO.transform.localPosition += progressBarGO.transform.up * progressbarIncrementPerWord;
            wordsSpelledCounter++;
            wordsSpelledText.text = wordsSpelledCounter.ToString() + "/" + wordsNeededToWin.ToString();
            animator.SetTrigger("WasRight");
            StartCoroutine(WaitAndSwitchWord(.75f));
            

        }
        else
        {
            Debug.Log("false");
            animator.SetTrigger("WasWrong");
            chosenWord.color = Color.red;
            typedWordInput.Select();
            typedWordInput.ActivateInputField();
            //if you want caret at the end
            //typedWordInput.MoveTextEnd(true);
            //backgroundImage.color = Color.red;
        }
    }

    private void ChangeLevel()
    {
        progressBarGO.transform.localPosition = progressBarTransformOG;
        wordsSpelledCounter = 0;
        wordsSpelledText.text = wordsSpelledCounter.ToString() + "/" + wordsNeededToWin.ToString();
        spellingLevel++;
        currentLevel = (CurrentLevel)(spellingLevel);
        currentSpellingList = spellingLists[spellingLevel];
        levelText.text = currentLevel.ToString();

    }

    private void DisplayRandomWord()
    {
        typedWordInput.text = string.Empty;
        typedWordInput.ActivateInputField();
        int randomWord = Random.Range(0, currentSpellingList.Count);
        
        while (lastRandomWord == randomWord || secondToLastRandomWord == randomWord)
        {
            randomWord = Random.Range(0, currentSpellingList.Count);
        }
        Debug.Log(randomWord + " " + lastRandomWord + " " + secondToLastRandomWord);
        chosenWord.text = currentSpellingList[randomWord];

        secondToLastRandomWord = lastRandomWord;
        lastRandomWord = randomWord;
        }
    IEnumerator WaitAndSwitchWord(float waitTime)
    {
        
        yield return new WaitForSeconds(waitTime);
        if (wordsNeededToWin == wordsSpelledCounter)
        {
            if(currentLevel == CurrentLevel.Legendary)
            {
                winScreen.SetActive(true);
                yield return new WaitForSeconds(waitTime);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                yield break;
            }
            ChangeLevel();

        }
        DisplayRandomWord();
        //backgroundImage.color = Color.white;
        chosenWord.color = Color.black;

    }

    private void OnWin()
    {

    }

    private void TimerAnimation()
    {
        
        Vector2 nextTextPos = new Vector2(ogTextPos.x + xSway, ogTextPos.y + ySway);
        Vector2 newTextPos = Vector2.Lerp(timerTM.rectTransform.localPosition, nextTextPos, textSpeed * Time.deltaTime);
        if(Mathf.Abs(nextTextPos.x - timerTM.rectTransform.localPosition.x) > .5f && 
            Mathf.Abs(nextTextPos.y - timerTM.rectTransform.localPosition.y) > .5f)
        {
            timerTM.rectTransform.localPosition = newTextPos;
            
        }
        else
        {
            Debug.Log("stop");
            StopAllCoroutines();
            ySway = Random.Range(-maxYSway, maxYSway);
            xSway = Random.Range(-maxXSway, maxXSway);
            textSpeed = Random.Range(minTextSpeed, maxTextSpeed);
            textAnimWaitTime = Random.Range(textAnimTimeMin, textAnimTimeMax);
            StartCoroutine(WaitAndSwitchRandomTextAnimSetting(textAnimWaitTime));
        }
    }

    IEnumerator WaitAndSwitchRandomTextAnimSetting(float waitTime)
    {
        
        yield return new WaitForSeconds(waitTime);
        ySway = Random.Range(-maxYSway, maxYSway);
        xSway = Random.Range(-maxXSway, maxXSway);
        textSpeed = Random.Range(minTextSpeed, maxTextSpeed);
        textAnimWaitTime = Random.Range(textAnimTimeMin, textAnimTimeMax);
        StartCoroutine(WaitAndSwitchRandomTextAnimSetting(textAnimWaitTime));
    }

}
