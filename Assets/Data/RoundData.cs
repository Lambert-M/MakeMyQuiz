using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoundData {

    //manche 1 , blindtest ...
    public string Type { get; set; }
    public bool IsBuzzRound { get; set; }
    public List<TopicData> Topics { get; set; }

    public RoundData(string type, List<TopicData> topics,bool isBuzzRound)
    {
        IsBuzzRound = isBuzzRound;
        Type = type;
        Topics = topics;
    }

    public int SearchTopic(string theme)
    {
        int index=-1;
        for (int i = 0; i < Topics.Count; i++)
        {
            if (theme.Equals(Topics[i].Name))
            {
                index = i;
            }
        }
        return index;
    }
}
