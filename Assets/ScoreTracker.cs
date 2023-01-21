using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] string _textPrefix;
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] long _overallScore = 0;   
    // Start is called before the first frame update
    void Start()
    {
        _text.text= _textPrefix;
        Events.SubscriebeToOnDiceRolled(OnDiceRolled);
    }

    void OnDiceRolled(long score) 
    {
        _overallScore += score;
        _text.text = _textPrefix + _overallScore;
    }
}
