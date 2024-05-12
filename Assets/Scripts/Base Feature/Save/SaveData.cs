namespace Save.Data
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SaveData
    {
        public int Iteration = 1; 
        public float PlayerExp = 0;
        public float CompanionExp = 0;
        public Dictionary<string, float> Memory = new();
        public BigFivePersonality CompanionPersonalities = new();
    }
}