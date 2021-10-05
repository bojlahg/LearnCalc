using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    public Color m_SelectedColor, m_DefaultColor;
    public GameObject m_SettingsScreen, m_CalcScreen, m_ResultScreen;
    public Toggle m_AddToggle, m_SubToggle, m_MulToggle, m_DivToggle;
    public Text m_CountText, m_TitleText, m_QuestionText;
    public Button m_NextButton;
    public Image[] m_ButtonImages;
    public Text[] m_ButtonTexts, m_ResultTexts;
    public GameObject[] m_Results;
    public string[] m_OperatorStrings;
    //
    private int[] m_ResultGood = new int[5], m_ResultCount = new int[5];
    private int m_IterValue = 10, m_TotalCount = 10, m_RightAnswerIndex = 0, m_AnswerIndex = -1, m_Index = 0, m_OperIndex = 0;
    private List<int> m_Operations = new List<int>();

    private void Awake()
    {
    }

    private void Start()
    {
        m_SettingsScreen.SetActive(true);
        m_CalcScreen.SetActive(false);
        m_ResultScreen.SetActive(false);
    }

    public void Settings_Start_Button()
    {
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

        if (m_Operations.Count > 0)
        {
            for (int i = 0; i < 5; ++i)
            {
                m_ResultGood[i] = 0;
                m_ResultCount[i] = 0;
            }

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
        m_AnswerIndex = idx;
        for (int i = 0; i < 9; ++i)
        {
            m_ButtonImages[i].color = i == m_AnswerIndex ? m_SelectedColor : m_DefaultColor;
        }
        m_NextButton.interactable = true;
    }

    public void Calc_Next_Button()
    {
        if (m_RightAnswerIndex == m_AnswerIndex)
        {
            ++m_ResultGood[m_OperIndex];
            ++m_ResultGood[4];
        }
        ++m_ResultCount[m_OperIndex];
        ++m_ResultCount[4];
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
    }

    private void SubGen(int oper, out int l, out int r, out int a)
    {
        int tmp;
        l = 0; r = 0; a = 0;
        switch (oper)
        {
            case 0: // Add
                l = Random.Range(1, 99);
                r = Random.Range(1, 99);
                a = l + r;
                break;

            case 1: // Sub
                l = Random.Range(1, 99);
                r = Random.Range(1, 99);
                if(l < r)
                {
                    tmp = l;
                    l = r;
                    r = tmp;
                }
                a = l - r;
                break;

            case 2: // Mul
                l = Random.Range(0, 10);
                r = Random.Range(0, 10);
                a = l * r;
                break;

            case 3: // Div
                l = Random.Range(2, 10);
                r = Random.Range(2, 10);
                a = l / r;
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

        Shuffle(answers);

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
    }

    private static void Shuffle(List<int> list)
    {
        int cnt = 0;
        while(cnt < 20)
        {
            int l = Random.Range(0, list.Count), r = Random.Range(0, list.Count);
            if(l != r)
            {
                int tmp = list[l];
                list[l] = list[r];
                list[r] = tmp;
                ++cnt;
            }
        }
    }

    private static int Wrap(int val, int min, int max)
    {
        int size = max - min;
        val -= min;
        if (val < 0)
        {
            return min + (val % size) + size;
        }
        else
        {
            return min + val % size;
        }
    }
}
