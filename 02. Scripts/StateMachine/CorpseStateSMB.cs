// using UnityEngine;

// public class CorpseStateSMB : StateMachineBehaviour
// {
//     private EnemyVisual visual;
//     private Material originalMaterial;
//     private Material outlineMaterial;

//     public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//     {
//         visual = animator.GetComponent<EnemyVisual>();

//         originalMaterial = visual.SR.material;

//         // Outline Material (Assets/Resources/Materials/OutlineMaterial.asset 에 미리 만들기)
//         //outlineMaterial = Resources.Load<Material>("Materials/OutlineMaterial");

//         // 시체 기본 색상/투명도
//         visual.SetCorpse(true);
//     }

//     public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//     {
//         visual.SetCorpse(false);
//     }

//     // 외부에서 호출: 마우스 오버 시
//     public void SetHighlight(bool active)
//     {
//         if (visual == null) return;

//         if (active && outlineMaterial != null)
//         {
//             visual.SR.material = outlineMaterial;
//             visual.SetCorpse(true);
//         }
//         else
//         {
//             visual.SR.material = originalMaterial;
//             visual.SetCorpse(true);
//         }
//     }
// }