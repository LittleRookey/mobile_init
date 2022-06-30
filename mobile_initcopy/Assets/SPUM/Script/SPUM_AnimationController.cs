using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SPUM_AnimationController : MonoBehaviour
{
    [Header("Character Animator")]
    [SerializeField] SPUM_Prefabs unit;
    [SerializeField] Animator animator => unit._anim;

    [Header("Animation Button Prefab")]
    [SerializeField] Button animationButtonPrefab;
    [SerializeField] Transform animationButtonParent;

    [Header("Animation Play Controller")]
    [SerializeField] Slider timeLineSlider;
    [SerializeField] Slider playSpeedSlider;
    [SerializeField] Text slidertimeLineInfo;
    [SerializeField] Text timeLineText;
    [SerializeField] Text playSpeedText;
    private void Start() 
    {
        if(unit && timeLineSlider && playSpeedSlider){
            Init();
            unit.UnitTypeChanged.AddListener( ()=> {
                RemoveAllAnimationButtons();
                InitAnimationButtons();
            });
        }else{
            Debug.LogError("Animator or Slider Component Not Setup!");
        }
    }

    private void Init(){
        timeLineSlider.minValue = 0f;
        timeLineSlider.maxValue = 1f;
        timeLineText = timeLineSlider.transform.GetComponentInChildren<Text>();
        timeLineSlider.onValueChanged.AddListener( x => {
            SetAnimationNormailzedTime(x);
            timeLineText.text = string.Format("{0:P0}", x);
        });

        playSpeedSlider.minValue = 0;
        playSpeedSlider.maxValue = 20;
        playSpeedSlider.wholeNumbers = true;
        playSpeedText = playSpeedSlider.transform.GetComponentInChildren<Text>();
        playSpeedSlider.onValueChanged.AddListener( x => {
            var AnimationSpeed = x * .1f;
            unit._anim.speed = AnimationSpeed;
            playSpeedText.text = string.Format("Speed x{0:0.0}", AnimationSpeed);
        });
        playSpeedSlider.value = 10f;
        
        InitAnimationButtons();
    }
    private void RemoveAllAnimationButtons(){
        var animationButtons = animationButtonParent.GetComponentsInChildren<Button>();
        foreach (var button in animationButtons)
        {
            Destroy(button.gameObject);
        }
    }
    private void InitAnimationButtons(){
        var clips = animator.runtimeAnimatorController.animationClips;

        foreach (var clip in clips)
        {
            int hash = Animator.StringToHash(clip.name);
            CreateAnimationButton( animationButtonParent, clip.name, () => { 
                    slidertimeLineInfo.text = clip.name;

                    timeLineSlider.SetValueWithoutNotify(0f);
                    timeLineText.text = "Progress";
                    animator.speed = playSpeedSlider.value * .1f;
                    animator.Play(hash, 0);
                });
        }
    }


    private void SetAnimationNormailzedTime(float progress){
        var state = animator.GetCurrentAnimatorStateInfo(0);
        animator.speed = 0;
        animator.Play(state.shortNameHash, 0, progress);
        animator.Update(0f);
    }

    private void CreateAnimationButton(Transform parent, string animationClip, UnityAction Action){
        var btn = Instantiate(animationButtonPrefab, Vector3.zero, Quaternion.identity, parent);
        btn.transform.GetChild(0).GetComponent<Text>().text = animationClip;
        btn.onClick.AddListener( ()=> {
            Action.Invoke();
        }); 
    }
}
