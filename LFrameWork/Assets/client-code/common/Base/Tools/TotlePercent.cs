namespace LFrameWork.Base.Tools
{
    using System;
    using System.Collections.Generic;

    public class TotlePercent : SingletonBase<TotlePercent>
    {
        private Dictionary<string, float> m_dict = new Dictionary<string, float>();
        private static float m_Percent;
        private Dictionary<string, float> m_totle = new Dictionary<string, float>();

        public float GetTotlePercent(string key, float percent)
        {
            foreach (float num in this.m_dict.Values)
            {
                if (this.m_dict.ContainsKey(key))
                {
                    float num2 = percent - this.m_totle[key];
                    Dictionary<string, float> totle = this.m_totle;
                    string str = key;
                    totle[str] += num2;
                    m_Percent += num * num2;
                }
            }
            return m_Percent;
        }

        public void Init(Dictionary<string, int> dict)
        {
            m_Percent = 0f;
            float num = 0f;
            this.m_dict.Clear();
            this.m_totle.Clear();
            foreach (int num2 in dict.Values)
            {
                num += num2;
            }
            foreach (string str in dict.Keys)
            {
                float num3 = ((float) dict[str]) / num;
                this.m_dict.Add(str, num3);
                this.m_totle.Add(str, 0f);
            }
        }
    }
}

