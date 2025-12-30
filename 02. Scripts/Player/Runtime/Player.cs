using UnityEngine;

/// <summary>
/// 플레이어 캐릭터의 주요 컴포넌트들을 관리하는 클래스
/// 각 컴포넌트들은 Player 클래스의 참조를 받아 초기화됨
/// 
/// 기본 적으로 외부에서 플레이어 컴포넌트를 받아야할 때는 GameManager클래스에서 받아오지만
/// 플레이어 내부 컴포넌트는 직접 초기화
/// </summary>
public class Player : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private InputManager inputManager;
    public PlayerStatAsset Stats;

    [Header("플레이어 컴포넌트")]
    public PlayerMovement playerMovement;
    public PlayerHp playerHp;
    public PlayerExp playerExp;
    public PlayerAttack playerAttack;
    public PlayerHit playerHit;
    public PlayerAnimation playerAnimation;
    public PlayerWeapon playerWeapon;

    [Header("네크로맨서")]
    [SerializeField] private PlayerNecroController necroController;

    [Header("기본 무기")]
    [SerializeField] private WeaponProfile defaultWeapon;


    // ==================================================
    public PlayerMovement Move => playerMovement;
    public PlayerHp Hp => playerHp;
    public PlayerExp Exp => playerExp;
    public PlayerAttack Attack => playerAttack;
    public PlayerHit Hit => playerHit;
    public PlayerWeapon Weapon => playerWeapon;
    public PlayerAnimation Animation => playerAnimation;

    public InputManager InputManager => inputManager;
    // ==================================================

    void Awake()
    {
        if (!inputManager) inputManager = FindFirstObjectByType<InputManager>();


        // Init, InitFrombase
        if (Stats)
        {
            playerMovement.Init(this);
            playerMovement.InitFromBase(Stats);
            playerHp.Init(this);
            playerHp.InitFromBase(Stats);
            playerHit.Init(this);
            playerWeapon.Init(this);
            playerWeapon.SetWeapon(defaultWeapon);
            necroController.Init(this, inputManager);

            //playerHp.InitFromBase(Stats);
            //playerAttack.InitFromBase(Stats);
            playerAnimation.Init(this);

        }
    }

    [System.Obsolete]
    void FixedUpdate()
    {
        playerMovement?.PhysicsMove();
        playerHit?.CheckHit();
    }
}
