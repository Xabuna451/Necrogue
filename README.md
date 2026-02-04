# 🧟 Necrogue

**네크로맨서 로그라이크 액션 게임**

> 플레이어가 적을 처치하고 언데드로 부활시킨 후, 시너지 있는 퍼크를 조합하여 강력한 군대를 빌드하는 싱글플레이 로그라이크 액션 게임입니다.

<div align="center">

![Unity](https://img.shields.io/badge/Unity-6000.2.13f1-black?logo=unity)
![C#](https://img.shields.io/badge/Language-C%23-239120?logo=csharp)
![Status](https://img.shields.io/badge/Status-Active%20Development-green)

</div>

---

## 📑 목차

- [개요](#개요)
- [게임 소개](#게임-소개)
- [핵심 설계](#핵심-설계)
- [주요 시스템](#주요-시스템)
- [프로젝트 구조](#프로젝트-구조)
- [기술 스택](#기술-스택)
- [설치 및 실행](#설치-및-실행)
- [포트폴리오 포인트](#포트폴리오-포인트)

---

## 개요

**Necrogue**는 포트폴리오 목적의 게임 프로젝트로, 게임의 완성도보다는 **구조 설계 능력과 문제 해결 판단**을 보여주기 위해 개발되었습니다.

### 프로젝트 목표

| 목표 | 설명 |
|------|------|
| 🏗️ **데이터 드리븐 구조** | 콘텐츠 추가 시 기존 코드 수정 최소화 |
| ⚙️ **스탯 안정성** | 퍼크 중첩, 배율 계산의 구조적 해결 |
| 📊 **설명 가능한 설계** | 모든 아키텍처 결정을 명확히 설명 가능 |
| 🎮 **확장 가능성** | 새로운 콘텐츠 추가에 대한 구조적 준비 |

---

## 게임 소개

### 게임플레이

```
플레이어 (네크로맨서)
  ├─ WASD로 이동
  ├─ 자동으로 언데드 소환 (슬롯 제한)
  ├─ 적 처치 → 경험치/보상 획득
  ├─ 레벨업 → 3개 퍼크 중 선택
  └─ 퍼크 조합으로 강력해지기
```

### 핵심 메커닉

**1. 언데드 소환 시스템**
- 네크로맨서의 가장 핵심 능력
- 동시 소환 가능한 언데드 수 제한 (슬롯)
- 각 언데드는 자동으로 가까운 적 추적 및 공격
- 퍼크로 공격력, 체력, 수량 강화 가능

**2. 퍼크 합성 시스템**
- 레벨업 시 3개 퍼크 중 1개 선택
- 5개 Rarity 등급: Common → Uncommon → Rare → Epic → Legendary
- 같은 퍼크를 여러 번 선택하면 스택으로 강화
- 목표: 여러 퍼크의 시너지로 강력한 빌드 구성

**3. 동적 스탯 시스템**
- **공격력**: 즉시 반영 (기존 언데드에도 적용)
- **체력**: 스냅샷 (스폰 시점의 값으로 고정)
- **슬롯**: 즉시 반영 (즉시 소환 수 조정)

---

## 핵심 설계

### 문제: 퍼크 중첩 시 스탯 계산 복잡성

**기존 증분 방식의 문제점**
```
문제 1: 적용 순서 의존성
  Perk A: 공격력 × 1.2
  Perk B: 공격력 + 10
  
  순서 A→B: (100 × 1.2) + 10 = 130
  순서 B→A: (100 + 10) × 1.2 = 132
  
문제 2: 고정값 + 배율 조합의 예측 불가
  퍼크 C: 공격력 override to 100
  → 다른 퍼크의 배율을 먹어야 하는가?

문제 3: 퍼크 제거/비활성 처리 복잡도
  증분 방식은 빼는 로직이 추가 필요
```

### 해결: Stat Composer (전체 재합성 구조)

```csharp
// 모든 퍼크를 수집한 후 한 번에 계산
public void ComposeAllStats()
{
    List<StatMod> mods = CollectAllActivePerks();
    
    // Add → Mul → Override 순서로 명시적 분리
    float baseAtk = 100;
    float addAmount = 0;
    float mulAmount = 1f;
    float overrideValue = -1;
    
    foreach (var mod in mods)
    {
        switch (mod.operation)
        {
            case Operation.Add:
                addAmount += mod.value;
                break;
            case Operation.Mul:
                mulAmount *= (1 + mod.value);
                break;
            case Operation.Override:
                overrideValue = mod.value;
                break;
        }
    }
    
    // 최종 공격력 = override 있으면 그걸, 없으면 계산
    finalAttack = overrideValue >= 0 
        ? overrideValue 
        : (baseAtk + addAmount) * mulAmount;
}
```

**장점**
- ✅ 적용 순서 무관 (항상 같은 결과)
- ✅ 예측 가능한 계산
- ✅ 퍼크 추가/제거 간단
- ✅ 성능: 조합이 많아도 매 레벨업 1회만 실행

### 아키텍처 분리: Data vs Runtime

**ScriptableObject (데이터 정의)**
```
PerkDef
├─ ID, Name, Description
├─ Rarity
├─ StatModList[]
└─ Effect Script Reference
```

**Runtime Logic**
```
PerkSystem
├─ 퍼크 선택 UI 제어
├─ 스탯 재합성 (Compose)
├─ 이벤트 발행
└─ 플레이어 스탯 적용
```

**의도**
- 데이터 추가 시 코드 수정 불필요
- 신규 개발자가 코드 이해 없이 퍼크 추가 가능
- 스탯 계산 로직은 한 곳에 집중

---

## 주요 시스템

### 1. GameManager (게임 상태 관리)

| 기능 | 설명 |
|------|------|
| **GameState** | Runtime / Pause / GameOver |
| **RuntimeState** | Playing / LevelUp / Death |
| **ObjectPool** | 적, 총알, 보상 재사용 |
| **GameClock** | 게임 내 시간 (난이도 조절) |

```csharp
public class GameManager : MonoBehaviour
{
    public GameState State { get; private set; }
    public RuntimeState RuntimeState { get; private set; }
    
    public event Action<GameState, GameState> OnGameStateChanged;
    public event Action<RuntimeState, RuntimeState> OnRuntimeStateChanged;
}
```

### 2. PerkSystem (퍼크 시스템)

**플로우**
1. 플레이어 레벨업 → UI 오픈
2. 3개 랜덤 퍼크 제시 (Rarity 확률 반영)
3. 플레이어 선택 → 퍼크 인스턴스 생성
4. 스탯 재합성 (Compose) 실행
5. 플레이어 스탯 즉시 갱신

**다중 Rarity 선택 지원**
```csharp
private Rarity GetRandomRarity()
{
    // Legendary: 1%, Rare: 5%, Uncommon: 20%, Common: 74%
    float roll = Random.value;
    if (roll < 0.01f) return Rarity.Legendary;
    if (roll < 0.06f) return Rarity.Rare;
    // ...
}
```

### 3. Enemy / Undead System

**Enemy (적)**
- EnemyContext에서 상태 관리
- EnemyDef(ScriptableObject)에서 스탯 정의
- 플레이어 방향 추적, 자동 공격

**Undead (플레이어가 소환한 언데드)**
- Enemy와 동일한 구조
- Faction만 Ally로 설정
- 언데드 전용 퍼크 효과 적용

**설계 의도**
- 상속 대신 Composition 패턴
- Elite / Boss 확장 시 새로운 ScriptableObject만 필요
- 코드 수정 최소화

### 4. Event System

```csharp
// 느슨한 결합을 위한 이벤트 기반 통신
public event Action<int> OnPlayerTakeDamage;
public event Action<PerkDef> OnPerkAcquired;
public event Action<int> OnEnemySpawned;
```

**장점**
- 게임 시스템 간 직접 의존성 제거
- 새로운 반응 로직 추가 시 기존 코드 수정 불필요
- 테스트 및 디버그 용이

---

## 프로젝트 구조

```
Assets/
├─ 01. Scenes/
│  ├─ Title.unity           ← 타이틀 화면
│  ├─ Necrogue.unity        ← 메인 게임
│  └─ GameOver.unity        ← 게임오버 화면
│
├─ 02. Scripts/
│  ├─ Common/               ← 공용 유틸리티
│  │  ├─ Data/             (세이브 데이터, 스탯)
│  │  ├─ Input/            (입력 처리)
│  │  ├─ Domain/           (핵심 도메인 로직)
│  │  ├─ Debug/            (디버그 도구)
│  │  └─ Interfaces/       (계약 인터페이스)
│  │
│  ├─ Player/               ← 플레이어 관련
│  │  ├─ Runtime/          (이동, 공격, 경험치)
│  │  └─ Data/             (플레이어 스탯)
│  │
│  ├─ Enemy/                ← 적 관련
│  │  ├─ Runtime/          (행동, 데미지)
│  │  └─ Data/             (적 프로필)
│  │
│  ├─ Perk/                 ← 퍼크 시스템 ⭐
│  │  ├─ Runtime/          (PerkSystem)
│  │  ├─ Data/             (PerkDef, Rarity)
│  │  ├─ Effect/           (퍼크 효과)
│  │  └─ UI/               (선택 화면)
│  │
│  ├─ Game/                 ← 게임 관리
│  │  ├─ Systems/          (GameManager, GameClock)
│  │  └─ Sounds/           (효과음)
│  │
│  ├─ Spawn/                ← 적 스폰
│  ├─ Shop/                 ← 상점 시스템
│  ├─ StateMachine/         ← 상태 관리
│  └─ UI/                   ← UI 시스템
│
├─ 03. ScriptableObjects/   ← 데이터 에셋
│  ├─ Perk/                (퍼크 정의)
│  ├─ Enemy/               (적 프로필)
│  ├─ Player/              (플레이어 초기 스탯)
│  └─ Spawner/             (난이도 곡선)
│
├─ 04. Prefabs/             ← 프리팹
│  ├─ Player.prefab
│  ├─ Enemy/
│  ├─ Undead/
│  └─ UI/
│
└─ 05. Art/                 ← 그래픽 리소스
   ├─ Animation/
   ├─ Images/
   ├─ Sounds/
   └─ Tile/
```

### 주요 스크립트

| 스크립트 | 역할 |
|---------|------|
| `GameManager.cs` | 게임 상태, 풀 관리 |
| `PerkSystem.cs` | 퍼크 선택, 스탯 합성 |
| `Player.cs` | 플레이어 위치, 생명주기 |
| `EnemyContext.cs` | 적 상태 관리 |
| `PlayerNecroController.cs` | 언데드 소환/관리 |
| `StatComposer.cs` | 스탯 재합성 로직 |

---

## 기술 스택

```
┌─ Engine & Language ─┐
│  • Unity 6000.2.13f1
│  • C# 9.0+
│  • .NET 4.x
└─────────────────────┘

┌─ Architecture ──────┐
│  • Singleton Pattern
│  • Object Pool Pattern
│  • Event-based System
│  • ScriptableObject (Data-driven)
└─────────────────────┘

┌─ Tools & Features ──┐
│  • Input System (New)
│  • Physics 2D
│  • Coroutines
│  • Animation
└─────────────────────┘

┌─ Performance ───────┐
│  • Object Pooling
│  • Lazy Initialization
│  • Event Aggregation
│  • Batch Stat Composition
└─────────────────────┘
```

---

## 설치 및 실행

### 필수 사항
- **Unity**: 6000.2.13f1 (정확한 버전)
- **OS**: Windows 10 이상 (또는 macOS / Linux)
- **메모리**: 4GB 이상

### 1단계: 저장소 클론
```bash
git clone https://github.com/your-username/Necrogue.git
cd Necrogue
```

### 2단계: Unity에서 열기
```
Unity Hub
  → Open Project
  → 이 폴더 선택
  → 버전 6000.2.13f1로 열기
```

### 3단계: 신나게 플레이!
```
Unity Editor에서 ▶ (Play) 버튼 클릭
또는 WASD로 이동, 적을 처치하고 퍼크를 선택하세요!
```

### 조작법

| 입력 | 동작 |
|------|------|
| **WASD** | 플레이어 이동 |
| **마우스 클릭** | UI 상호작용 |
| **ESC** | 일시정지 / 퍼크 확인 |
| **SPACE** | 선택 확인 |

---

## 포트폴리오 포인트

### ✨ 강점

**1. 문제 인식 & 구조적 해결**
- 퍼크 중첩 시 스탯 계산 문제를 명확히 인식
- Stat Composer 구조로 수학적으로 우아한 해결
- "왜 이렇게?" 설명 가능

**2. 데이터 드리븐 설계**
- ScriptableObject 중심의 콘텐츠 확장
- 프로그래머가 아닌 디자이너도 콘텐츠 추가 가능
- 실무 수준의 구조

**3. 아키텍처 분리**
- Runtime 로직과 Data 정의의 명확한 분리
- 향후 단위 테스트 작성 용이
- SOLID 원칙 준수

**4. 확장 가능성**
- 새로운 Rarity 등급 추가? 1개 enum값만 필요
- 새로운 퍼크? ScriptableObject 1개만 필요
- 새로운 Enemy 타입? ScriptableObject 1개만 필요
- 기존 코드 수정 거의 없음

### 📌 의도적 제외

다음 항목은 포트폴리오 가치 대비 개발 비용이 높다고 판단하여 제외했습니다:

| 항목 | 이유 |
|------|------|
| 보스 몬스터 | 콘텐츠 제작 비용 높음, 구조 검증 불필요 |
| 엘리트 특수 패턴 | ScriptableObject 1개로 확장 가능함을 증명 |
| 엔딩/클리어 조건 | 게임플레이 메커닉 검증에 불필요 |
| 복잡한 비주얼 | 프로그래밍 능력과 무관 |

**핵심 메시지**: 게임의 완성도가 아닌 **설계 판단과 구조 설명**이 목표

---

## 개발 상태

### ✅ 완료
- [x] 게임 루프 (스폰 → 전투 → 레벨업)
- [x] 퍼크 시스템 및 Rarity 등급
- [x] 플레이어/적/언데드 기본 메커닉
- [x] UI 및 퍼크 선택 화면
- [x] 게임 상태 관리
- [x] 효과음 및 기본 애니메이션

### 🔄 진행 중
- [ ] 세이브/로드 시스템
- [ ] 고급 밸런싱 패스

### 📋 향후 계획
- [ ] 보스 전투 (구조 확장 예시)
- [ ] 통계/성취 시스템
- [ ] 난이도 선택

---

## 더 알아보기

### 자세한 설계 문서
- [퍼크 합성 규칙](Assets/necromancer_perk_composition_rules.md) - 퍼크 밸런스 철학

---

## 라이선스 & 출처

- **엔진**: Unity Technologies
- **아트**: [출처 기재 예정]
- **음향**: [출처 기재 예정]

---

## 피드백 & 연락처

```
📧 이메일: your-email@example.com
💼 포트폴리오: https://your-portfolio.com
🔗 LinkedIn: https://linkedin.com/in/your-profile
```

---

<div align="center">

**Necrogue** · v0.5.0-dev · 2026년 2월

"구조 설계 능력을 보여주기 위한 포트폴리오 프로젝트"

</div>
