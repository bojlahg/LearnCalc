using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    public Color m_GoodColor, m_BadColor, m_DefaultColor;
    public GameObject m_SettingsScreen, m_CalcScreen, m_ResultScreen;
    public Toggle m_AddToggle, m_SubToggle, m_MulToggle, m_DivToggle, m_SndToggle;
    public Text m_CountText, m_TitleText, m_QuestionText, m_TimerText;
    public Button m_NextButton;
    public Image[] m_ButtonImages;
    public Text[] m_ButtonTexts, m_ResultTexts;
    public GameObject[] m_Results;
    public string[] m_OperatorStrings;
    public AudioSource m_AudioSource;
    public AudioClip[] m_GoodSounds, m_BadSounds;
    //
    private float m_Timer = 0;
    private int[] m_ResultGood = new int[5], m_ResultCount = new int[5];
    private int m_IterValue = 10, m_TotalCount = 10, m_RightAnswerIndex = 0, m_Index = 0, m_OperIndex = 0;
    private bool m_Answered = false, m_SoundEnabled = true;
    private List<int> m_Operations = new List<int>();

    private void Awake()
    {
    }

    private void Start()
    {
        m_SettingsScreen.SetActive(true);
        m_CalcScreen.SetActive(false);
        m_ResultScreen.SetActive(false);

        m_AddToggle.isOn = PlayerPrefs.GetInt("OpAdd", 1) == 1 ? true : false;
        m_SubToggle.isOn = PlayerPrefs.GetInt("OpSub", 1) == 1 ? true : false;
        m_MulToggle.isOn = PlayerPrefs.GetInt("OpMul", 1) == 1 ? true : false;
        m_DivToggle.isOn = PlayerPrefs.GetInt("OpDiv", 1) == 1 ? true : false;
        m_SndToggle.isOn = PlayerPrefs.GetInt("SndOn", 1) == 1 ? true : false;
        m_TotalCount = PlayerPrefs.GetInt("Cnt", 10);
        m_CountText.text = m_TotalCount.ToString();
    }

    private void Update()
    {
        if (!m_Answered)
        {
            m_Timer += Time.deltaTime;
        }
        m_TimerText.text = string.Format("{0:0.0}", m_Timer);
    }

    public void Settings_Start_Button()
    {
        m_Operations.Clear();
        if (m_AddToggle.isOn)
        {
            m_Operations.Add(0);
        }
        if (m_SubToggle.isOn)
        {
            m_Operations.Add(1);
        }
        if (m_MulToggle.isOn)
        {
            m_Operations.Add(2);
        }
        if (m_DivToggle.isOn)
        {
            m_Operations.Add(3);
        }

        m_SoundEnabled = m_SndToggle.isOn;

        if (m_Operations.Count > 0)
        {
            for (int i = 0; i < 5; ++i)
            {
                m_ResultGood[i] = 0;
                m_ResultCount[i] = 0;
            }

            PlayerPrefs.SetInt("OpAdd", m_AddToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt("OpSub", m_SubToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt("OpMul", m_MulToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt("OpDiv", m_DivToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt("SndOn", m_SndToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt("Cnt", m_TotalCount);
            PlayerPrefs.Save();

            m_Timer = 0;
            m_Index = 0;
            m_SettingsScreen.SetActive(false);
            m_CalcScreen.SetActive(true);
            m_ResultScreen.SetActive(false);
            Generate();
        }
    }

    public void Settings_Add_Button()
    {
        if (m_TotalCount + m_IterValue <= 100)
        {
            m_TotalCount += m_IterValue;
        }
        m_CountText.text = m_TotalCount.ToString();
    }

    public void Settings_Sub_Button()
    {
        if (m_TotalCount - m_IterValue >= m_IterValue)
        {
            m_TotalCount -= m_IterValue;
        }
        m_CountText.text = m_TotalCount.ToString();
    }

    public void Calc_Answer_Button(int idx)
    {
        if (!m_Answered)
        {
            m_Answered = true;
            if (m_RightAnswerIndex == idx)
            {
                ++m_ResultGood[m_OperIndex];
                ++m_ResultGood[4];

                if(m_SoundEnabled)
                {
                    m_AudioSource.clip = m_GoodSounds[Random.Range(0, m_GoodSounds.Length)];
                    m_AudioSource.Play();
                }
            }
            else 
            {
                if (m_SoundEnabled)
                {
                    m_AudioSource.clip = m_BadSounds[Random.Range(0, m_BadSounds.Length)];
                    m_AudioSource.Play();
                }
            }
            ++m_ResultCount[m_OperIndex];
            ++m_ResultCount[4];

            for (int i = 0; i < 9; ++i)
            {
                m_ButtonImages[i].color = i == m_RightAnswerIndex ? m_GoodColor : m_BadColor;
            }
            m_NextButton.interactable = true;
        }
    }

    public void Calc_Next_Button()
    {
        ++m_Index;
        if (m_Index == m_TotalCount)
        {
            ShowResults();            
        }
        else
        {
            Generate();
        }
    }

    public void Result_Continue_Button()
    {
        m_SettingsScreen.SetActive(true);
        m_CalcScreen.SetActive(false);
        m_ResultScreen.SetActive(false);
    }

    private void ShowResults()
    {
        m_SettingsScreen.SetActive(false);
        m_CalcScreen.SetActive(false);
        m_ResultScreen.SetActive(true);
        
        for(int i = 0; i < m_Results.Length; ++i)
        {
            m_Results[i].SetActive(false);
        }
        for (int i = 0; i < m_Operations.Count; ++i)
        {
            m_Results[m_Operations[i]].SetActive(true);
        }

        for(int i = 0; i < 5; ++i)
        {
            m_ResultTexts[i].text = string.Format("{0} / {1}", m_ResultGood[i], m_ResultCount[i]);
        }
        m_ResultTexts[5].text = string.Format("{0:0.0}", m_Timer);
    }

    private void SubGen(int oper, out int l, out int r, out int a)
    {
        int tmp;
        l = 0; r = 0; a = 0;
        switch (oper)
        {
            case 0: // Add
                l = Random.Range(1, 100);
                r = Random.Range(1, 100);
                a = l + r;
                break;

            case 1: // Sub
                l = Random.Range(1, 100);
                r = Random.Range(1, 100);
                if(l < r)
                {
                    tmp = l;
                    l = r;
                    r = tmp;
                }
                a = l - r;
                break;

            case 2: // Mul
                l = Random.Range(1, 11);
                r = Random.Range(1, 11);
                a = l * r;
                break;

            case 3: // Div
                l = Random.Range(1, 11);
                r = Random.Range(1, 11);
                a = l * r;
                tmp = l;
                l = a;
                a = tmp;
                break;
        }
    }

    private void Generate()
    {
        int idx = Random.Range(0, m_Operations.Count);
        m_OperIndex = m_Operations[idx];
        int left = 0, right = 0, answer = 0, l = 0, r = 0, a = 0;
        List<int> answers = new List<int>();

        while (answers.Count < 9)
        {
            SubGen(m_OperIndex, out l, out r, out a);
            
            if (answers.Count == 0)
            {
                left = l;
                right = r;
                answer = a;
                answers.Add(a);
            }
            else
            {
                bool found = false;
                for (int i = 0; i < answers.Count; ++i)
                {
                    if (answers[i] == a)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    answers.Add(a);
                }
            }
        }

        answers.Sort();
        //Shuffle(answers);

        for(int i = 0; i < answers.Count; ++i)
        {
            if (answers[i] == answer)
            {
                m_RightAnswerIndex = i;
            }
        }

        m_QuestionText.text = string.Format("{0} {1} {2}", left, m_OperatorStrings[m_OperIndex], right);
        for (int i = 0; i < 9; ++i)
        {
            m_ButtonTexts[i].text = answers[i].ToString();
            m_ButtonImages[i].color = m_DefaultColor;
        }
        m_TitleText.text = string.Format("{0} / {1}", m_Index + 1, m_TotalCount);

        m_NextButton.interactable = false;
        m_Answered = false;
    }
}
