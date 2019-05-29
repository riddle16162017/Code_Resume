using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateLevel : MonoBehaviour {

    public static CreateLevel instance;
    public GameObject StartText;
    public GameObject EndText;
    public GameObject PerfectText;
    public GameObject BasicBall;

    public List<List<Vector2>> PossiblePlace = new List<List<Vector2>>();
    public List<List<Vector2>> UsingPlace = new List<List<Vector2>>();
    public List<GameObject> BallList = new List<GameObject> {};

    Color startColor;
    Color endColor;
    int basicBallCount = 3;
    float colorDifference = 0.35f;

    private void Start()
    {
        instance = this;
        int tempCounter = 0;
        for (int i = (int)(-(GameScene.ScreenAspect / 2 - 50) / 250); i <= (int)((GameScene.ScreenAspect / 2 - 50) / 250); i++)
        {
            PossiblePlace.Add(new List<Vector2>());
            for (int j = (int)(-(Screen.height / 2 - 300) / 250); j <= (int)((Screen.height / 2 - 400) / 250); j++)
            {
                PossiblePlace[tempCounter].Add(new Vector2(i * 2.5f, j * 2.5f));
            }
            tempCounter++;
        }
    }

    public void StartNewLevel()
    {
        TechMgr.GameAnalytics.NewProgressionEvent_Start();
        StartCoroutine("timeTick");
    }

    public void CreateAPerfect()
    {
        StartCoroutine(PerfectAni());
    }

    public IEnumerator CreateOneLevel()
    {
        GameScene.ComboPiont = 0;
        UsingPlace.Clear();
        foreach (List<Vector2> obj in PossiblePlace)
        {
            UsingPlace.Add(new List<Vector2>(obj));
        }

        BallList = new List<GameObject> { };
        yield return new WaitForEndOfFrame();
        float tempStartH = Random.Range(0, 100) / 100f;
        float tempEndH;
        startColor = Random.ColorHSV(tempStartH, tempStartH, 0.8f, 0.8f, 1f, 1f, 1, 1);

        if (Random.Range(0, 2) == 0)
        {
            tempEndH = tempStartH + colorDifference + (float)Random.Range((GameScene.GameLevel > 60f ? 60f : (float)GameScene.GameLevel), (GameScene.GameLevel > 60f ? -60f : (float)-GameScene.GameLevel)) / 400f;
            if (tempEndH > 1)
            {
                tempEndH -= 1;
            }
        }
        else
        {
            tempEndH = tempStartH - (colorDifference + (float)Random.Range((GameScene.GameLevel > 60f ? 60f : (float)GameScene.GameLevel), (GameScene.GameLevel > 60f ? -60f : (float)-GameScene.GameLevel)) / 400f);
            if (tempEndH < 0)
            {
                tempEndH += 1;
            }
        }

        endColor = Random.ColorHSV(tempEndH, tempEndH, 0.5f, 0.5f, 1, 1, 1, 1);

        for (int i = 0; i <= (basicBallCount + (int)(GameScene.GameLevel / 5f)); i++)
        {
            if (UsingPlace.Count != 0)
            {
                int temp = Random.Range(0, UsingPlace.Count);
                int temp2 = Random.Range(0, UsingPlace[temp].Count);
                GameObject tmp = Instantiate(BasicBall, transform.parent.transform);
                BallList.Add(tmp);
                tmp.GetComponent<BasicBall>().ColorBall.GetComponent<Image>().color = new Color((startColor.r * (((float)(basicBallCount + (int)(GameScene.GameLevel / 10f)) - (float)i) / ((float)(basicBallCount + (int)(GameScene.GameLevel / 10f))))) + (endColor.r * ((float)i / ((float)(basicBallCount + (int)(GameScene.GameLevel / 10f))))), (startColor.g * (((float)(basicBallCount + (int)(GameScene.GameLevel / 10f)) - (float)i) / ((float)(basicBallCount + (int)(GameScene.GameLevel / 10f))))) + (endColor.g * ((float)i / ((float)(basicBallCount + (int)(GameScene.GameLevel / 10f))))), (startColor.b * (((float)(basicBallCount + (int)(GameScene.GameLevel / 10f)) - (float)i) / ((float)(basicBallCount + (int)(GameScene.GameLevel / 10f))))) + (endColor.b * ((float)i / ((float)(basicBallCount + (int)(GameScene.GameLevel / 10f))))), 0);
                tmp.transform.localPosition = UsingPlace[temp][temp2] * 100 + new Vector2(Random.Range(-30, 30), Random.Range(-30, 30));
                UsingPlace[temp].RemoveAt(temp2);
                if (UsingPlace[temp].Count == 0)
                {
                    UsingPlace.RemoveAt(temp);
                }
                LeanTween.alpha(tmp.GetComponent<BasicBall>().ColorBall.GetComponent<RectTransform>(), 1, 0.2f).setEase(LeanTweenType.easeInOutSine);
                if (i == basicBallCount)
                {
                    StartCoroutine(ShowStartEnd());
                }
            }
        }
        GameScene.BallCount = BallList.Count;
    }

    public IEnumerator ShowStartEnd()
    {
        yield return new WaitForEndOfFrame();
        StartText.SetActive(true);
        EndText.SetActive(true);
        LeanTween.cancel(StartText);
        LeanTween.cancel(EndText);
        StartText.transform.position = BallList[0].transform.position;
        EndText.transform.position = BallList[BallList.Count - 1].transform.position;
        LeanTween.scaleX(StartText, 1, 0.1f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.scaleX(EndText, 1, 0.1f).setEase(LeanTweenType.easeInOutSine);
    }

    public IEnumerator HideStartEnd()
    {
        yield return new WaitForEndOfFrame();
        LeanTween.cancel(StartText);
        LeanTween.cancel(EndText);
        LeanTween.scaleX(StartText, 0, 0.1f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.scaleX(EndText, 0, 0.1f).setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
        {
            StartText.SetActive(false);
            EndText.SetActive(false);
        });
    }

    public IEnumerator PerfectAni()
    {
        PerfectText.SetActive(true);
        LeanTween.cancel(PerfectText);
        PerfectText.transform.position = Vector3.zero;
        LeanTween.scale(PerfectText, new Vector2(3f, 3f), 0.7f).setEase(LeanTweenType.easeOutElastic);
        yield return new WaitForSeconds(0.6f);
        LeanTween.move(PerfectText, GameScene.instance.TimeText.transform.position, 0.3f).setEase(LeanTweenType.easeInOutSine);
        LeanTween.scale(PerfectText, Vector2.zero, 0.3f).setEase(LeanTweenType.easeInSine).setOnComplete(() =>
        {
            PerfectText.SetActive(false);
        });
        yield return new WaitForSeconds(0.3f);
        LeanTween.textColor(GameScene.instance.TimeText.GetComponent<RectTransform>(), new Color(0.21f, 0.97f, 0.11f), 0.5f).setEase(LeanTweenType.easeOutSine).setOnComplete(() =>
        {
            LeanTween.textColor(GameScene.instance.TimeText.GetComponent<RectTransform>(), new Color(0.196f, 0.196f, 0.196f), 0.5f).setEase(LeanTweenType.easeInSine);
        });

        for (int i = 0; i < GameScene.BonusTime; i++)
        {
            GameScene.LeftTime++;
            GameScene.instance.TimeText.text = GameScene.LeftTime.ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator timeTick()
    {
        StartCoroutine(CreateOneLevel());
        while (true)
        {
            yield return new WaitForSeconds(1);
            GameScene.LeftTime--;
            GameScene.instance.TimeText.text = GameScene.LeftTime.ToString();
            if (GameScene.LeftTime <= 0)
            {
                GameScene.instance.EndGame();
                StopCoroutine("timeTick");
            }
        }
    }
}
