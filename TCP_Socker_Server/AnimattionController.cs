using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AnimattionController : MonoBehaviour
{
    
    // Start is called before the first frame update
    public Animator sign;
    void Start()
    {
        int aniRandom = Random.Range(1, 9);
        string result =aniRandom.ToString();
        Dictionary<string, string> dic = new Dictionary<string, string>() {
        { "1","LTR"},
        { "2","LTMslow"},
        { "3","LTMfast"},
        { "4","LTRslow"},
        { "5","LTRfast"},
        { "6","static"},
        { "7","RTM"},
        { "8","LTMpoint"}


    };
        sign.Play(dic[result]);
        
    
    }

    // Update is called once per frame
    void Update()
    {
       

    }
}
