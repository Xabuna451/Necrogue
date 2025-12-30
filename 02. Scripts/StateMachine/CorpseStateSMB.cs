using UnityEngine;

public class CorpseStateSMB : StateMachineBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Material outlineMaterial;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        spriteRenderer = animator.GetComponent<SpriteRenderer>();

        originalMaterial = spriteRenderer.material;

        // Outline Material (Assets/Resources/Materials/OutlineMaterial.asset 에 미리 만들기)
        outlineMaterial = Resources.Load<Material>("Materials/OutlineMaterial");

        // 시체 기본 색상/투명도
        spriteRenderer.color = new Color(0.8f, 0.8f, 0.8f, 0.7f);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        spriteRenderer.color = Color.white;
    }

    // 외부에서 호출: 마우스 오버 시
    public void SetHighlight(bool active)
    {
        Debug.Log("하이라이응으"); // 지울것
        if (spriteRenderer == null) return;

        if (active && outlineMaterial != null)
        {
            spriteRenderer.material = outlineMaterial;
            spriteRenderer.color = Color.white; // Outline이 잘 보이게
        }
        else
        {
            spriteRenderer.material = originalMaterial;
            //spriteRenderer.color = new Color(0.8f, 0.8f, 0.8f, 0.7f);
        }
    }
}