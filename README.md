# AR기반 등하교 서포트 서비스 (iOS AR 모듈)

## 📋 프로젝트 개요

AR기반 등하교 서포트 서비스는 초등학생을 위한 AR 기반 안전 등하교 내비게이션 앱입니다. 이 저장소는 전체 서비스 중 **iOS AR 경로 시각화** 부분을 담당합니다. AI 시스템(https://github.com/woojinhong03/Capstone_SafeMap/)에서 생성된 안전 경로 데이터를 받아, Unity AR Foundation과 ARKit을 활용하여 아이가 따라갈 수 있는 AR 코인 경로를 현실 세계에 구현합니다.

## 🎯 AR 모듈 개발 목적

* AI 시스템이 도출한 **안전 경로 좌표(JSON)**를 정확히 로드 및 해석
* GPS, 나침반, AR 공간 인식을 결합하여 **현실 세계에 경로를 정확히 정렬**
* AR 코인을 **시각적으로 명확하고 안정적으로 배치**하여 직관적인 길 안내 제공
* 게임(코인 수집) 요소를 통해 아이의 **능동적이고 즐거운 참여 유도**
* 경사 변화 등 **다양한 지형에 대응**하는 안정적인 AR 객체 배치 구현

## 🔧 주요 기능 (AR)

1.  **안전 경로 데이터 연동:**
    * Python 백엔드 시스템에서 생성된 `route.json` (좌표 배열) 파일을 로드 (`PathGenerator.cs`)
    * JSON 데이터 파싱 (`Newtonsoft.Json` 활용)

2.  **AR 공간 좌표계 설정 및 보정:**
    * GPS 및 나침반 센서 초기화 및 준비 상태 확인 (`LocationManager.cs`)
    * 사용자 액션(버튼 클릭) 시, 현재 GPS 위치와 **나침반 방향을 기준**으로 AR 월드 좌표계 설정 (`CoinSpawner.cs` - `SpawnCoinsSequence` 코루틴)
    * 현실 세계의 북쪽 방향과 AR 공간의 방향을 일치시켜 경로 왜곡 방지

3.  **AR 코인 경로 시각화:**
    * 로드된 경로 좌표를 보정된 AR 월드 좌표로 변환 (`CoinSpawner.cs` - `GPSToMeters` 및 회전 로직)
    * AR 평면 인식(바닥 감지)을 통해 코인의 **기본 높이 설정** (`CoinSpawner.cs` - `TryGetBaseHeight`)
    * 계산된 3D 위치에 코인 프리팹(`Coin.prefab`) 동적 생성 (`Instantiate`)
    * 개별 코인 높이 미세 조정 (Raycast 활용)

4.  **사용자 인터랙션 및 피드백:**
    * AR 카메라와 코인 간의 3D 거리를 실시간으로 계산 (`GameManager.cs`)
    * 설정된 거리(`collectDistance`) 내 접근 시 자동 코인 수거 (오디오 피드백 포함 - `Coin.cs`)
    * UI 텍스트를 통해 현재 상태, 다음 코인까지의 거리, 수집 현황 등 제공 (`GameManager.cs`)
    * 코인 생성 후 생성 버튼 비활성화 및 텍스트 변경 (`CoinSpawner.cs`)

5.  **자동 높이 재배치 (Auto-Relocation):**
    * 일정 간격(기본 5초)마다 현재 바닥 높이를 다시 감지 (`CoinSpawner.cs` - `AutoRelocateCoroutine`)
    * 수집되지 않은 코인들의 높이를 새로운 바닥 높이에 맞게 자동으로 재조정 (`RelocateExistingCoins`)
    * 재배치까지 남은 시간을 UI 텍스트로 표시 (`relocationTimerText`)

## 🛠️ 기술 스택 (AR)

* **개발 플랫폼:** Unity 2022.3.x (LTS 권장)
* **AR 프레임워크:** AR Foundation 5.x
* **iOS AR 구현:** ARKit XR Plugin 5.x
* **주요 개발 언어:** C#
* **데이터 파싱:** Newtonsoft Json for Unity (`com.unity.nuget.newtonsoft-json`)
* **센서:** iOS Location Service (GPS), Compass

## 📁 Unity 프로젝트 구조 (Assets 폴더 기준)
```
Assets/
 ├─ Scenes/
 │   └─ MainScene.unity       # 메인 AR 씬
 ├─ Scripts/                  # C# 스크립트
 │   ├─ CoinSpawner.cs        # 코인 생성, 방향 보정, 높이 조절
 │   ├─ GameManager.cs        # 게임 상태 관리, 코인 수집, UI 업데이트
 │   ├─ LocationManager.cs    # GPS, 나침반 관리
 │   ├─ PathGenerator.cs      # JSON 경로 데이터 로딩
 │   ├─ Coin.cs               # 코인 개별 오브젝트 스크립트
 │   └─ DebugManager.cs       # (디버깅용) 상태별 배경색 변경
 ├─ Prefabs/                  # 재사용 오브젝트 원본
 │   ├─ Coin.prefab           # AR 코인 프리팹
 │   └─ PlanePrefab.prefab    # AR 바닥 인식 시각화용 프리팹
 ├─ Resources/                # 동적 로딩 애셋
 │   ├─ route.json            # Python 백엔드에서 생성된 경로 좌표 파일 (인천시청역 3거리)
 │   └─ UI file               # UI 이미지 파일
 ├─ Materials/                # 3D 모델 재질
 ├─ Audio/                    # 효과음
 ├─ Fonts/                    # (필요시) TextMeshPro 한글 폰트 에셋
 └─ <기타 Unity 폴더들...>
```

## 🖥️ 씬 계층 구조 (Hierarchy 창)
```
- MainScene
  - Directional Light          # 기본 조명
  - AR Session                 # AR 기능 활성화 컴포넌트
  - XR Origin                  # AR 공간의 기준점 (카메라, 관리자 포함)
    └─ Camera Offset           # 카메라 위치 조정을 위한 중간 객체
       └─ AR Camera            # 실제 AR 화면을 보여주는 카메라 (MainCamera 태그)
         └─ AR Plane Manager   # 바닥 감지 및 시각화 (PlanePrefab 연결)
         └─ AR Raycast Manager # 코인 배치 시 바닥 확인용
  - Managers                   # 모든 관리자 스크립트를 모아둔 빈 오브젝트
    └─ (CoinSpawner.cs)       # 컴포넌트로 부착
    └─ (GameManager.cs)        # 컴포넌트로 부착
    └─ (LocationManager.cs)   # 컴포넌트로 부착
    └─ (PathGenerator.cs)     # 컴포넌트로 부착
    └─ (DebugManager.cs)      # 컴포넌트로 부착
  - Canvas                     # 모든 UI 요소의 부모
    └─ InfoPanel (선택 사항)    # UI 그룹화용 패널
       ├─ Coin Spawn Button    # 코인 생성 버튼
       ├─ StatusText (TMP)     # 상태 메시지 텍스트
       └─ CoinCountText (TMP)  # 코인 개수 텍스트
       └─ RelocationTimerText(TMP)# 재배치 타이머 텍스트
  - EventSystem                # UI 입력 처리를 위해 자동으로 생성됨
```

## 🚀 설치 및 실행 (iOS)

1.  **저장소 클론:**
    ```bash
    git clone <이 저장소의 GitHub URL>
    cd <프로젝트 폴더명>
    ```
2.  **Unity Hub에서 프로젝트 열기:** Unity Hub에 이 프로젝트 폴더를 추가하고, 올바른 Unity 버전(2022.3.x 권장)으로 엽니다.
3.  **필수 패키지 설치 확인:**
    * `Window > Package Manager`에서 아래 패키지들이 설치되어 있는지 확인 (없으면 설치):
        * AR Foundation (5.x 버전)
        * ARKit XR Plugin (5.x 버전)
        * Newtonsoft Json for Unity (`com.unity.nuget.newtonsoft-json` - 이름으로 추가)
4.  **경로 데이터 배치:** Python 백엔드 시스템에서 생성된 `route.json` 파일을 `Assets/Resources/` 폴더 안에 넣습니다.
5.  **iOS 빌드 설정:**
    * `File > Build Settings`에서 플랫폼을 **iOS**로 변경합니다.
    * `Player Settings` 버튼 클릭:
        * `Other Settings > Identification > Bundle Identifier`: 고유한 ID 입력 (예: `com.YourTeam.ARNavigation`)
        * `Other Settings > Configuration > Camera Usage Description`: "AR 기능을 위해 카메라 권한이 필요합니다." 입력
        * `Other Settings > Configuration > Location Usage Description`: "GPS 기반 경로 안내를 위해 위치 권한이 필요합니다." 입력
        * `XR Plug-in Management > iOS`: **ARKit** 체크 활성화
6.  **빌드:** `Build Settings` 창에서 `Build` 버튼을 누르고 Xcode 프로젝트를 생성할 폴더를 지정합니다.
7.  **Xcode에서 실행:**
    * 생성된 `.xcodeproj` 파일을 엽니다.
    * Mac에 iOS 기기(iPhone/iPad)를 연결합니다.
    * Xcode 상단에서 연결된 기기를 선택합니다.
    * 프로젝트 설정의 `Signing & Capabilities` 탭에서 본인의 Apple Developer 계정(Team)을 설정합니다.
    * Xcode의 **▶ (실행)** 버튼을 눌러 기기에 앱을 빌드하고 실행합니다.

## 📊 출력 결과 (사용자 경험)

* 앱 실행 후 GPS 준비 완료 및 바닥 스캔 안내
* 사용자가 '코인 생성' 버튼 클릭
* 현재 사용자의 위치와 바라보는 방향을 기준으로 현실 세계 경로 위에 AR 코인들이 일렬로 생성됨
* 사용자가 코인을 따라 걸어가면 자동으로 수집되며 효과음 발생 및 카운터 증가
* 경사진 지형에서도 코인이 지면 높이에 맞춰 자동으로 높이 조절됨 (5초 간격)

## 🔍 핵심 알고리즘 (AR 좌표계 설정)

1.  **GPS/나침반 준비:** `LocationManager`가 GPS 신호(`IsLocationReady`)와 나침반(`Input.compass.enabled`)을 준비시킵니다.
2.  **바닥 인식:** 사용자가 바닥을 스캔하면 `ARPlaneManager`가 평면을 감지하고, `CoinSpawner`는 이 중 가장 가까운 평면의 높이를 기준으로 사용합니다 (`TryGetBaseHeight`).
3.  **기준점 설정 (버튼 클릭 시):** `CoinSpawner`의 `SpawnCoinsSequence` 코루틴이 시작됩니다.
    * 나침반 값이 안정화될 때까지 대기합니다.
    * **방향 보정값(`northRotation`) 계산:** 현재 AR 카메라가 바라보는 방향(`arCamera.transform.eulerAngles.y`)과 실제 북쪽 방향(`Input.compass.trueHeading`)의 차이를 계산합니다. 이 값이 AR 공간을 현실 방향과 일치시키는 회전 기준이 됩니다.
4.  **코인 위치 계산:**
    * `PathGenerator`가 읽어온 각 경로 좌표(`targetGPS`)에 대해, 현재 사용자 위치(`baseGPS`)로부터의 **미터 단위 상대 위치(`offsetInMeters`)**를 계산합니다 (`GPSToMeters`).
    * 이 상대 위치 벡터를 위에서 계산한 **`northRotation`으로 회전**시켜 방향을 보정합니다 (`rotatedOffset`).
    * 최종적으로, 현재 **AR 카메라의 위치**에 이 **회전된 오프셋**을 더하여 AR 공간 내 코인의 절대 위치(`finalPosition`)를 결정합니다.
5.  **코인 생성 및 관리:** 계산된 `finalPosition`에 코인 프리팹을 `Instantiate`하고, 생성된 코인 목록을 `GameManager`에 넘겨 수집 관리를 시작합니다.

## 📱 시스템 아키텍처 (AR 모듈)
```
1.  지도 가져오기 (route.json -> PathGenerator): 먼저 앱은 보물 지도(안전 경로)가 필요해요. PathGenerator가 AI 팀이 만들어준 지도 데이터(route.json)를 읽습니다.

2.  내 위치 알기 (센서 -> LocationManager): 앱은 스마트폰의 GPS와 나침반 센서를 사용해요. LocationManager는 이 센서들을 계속 확인해서 지금 내가 어디 있는지, 북쪽이 어디인지를 알아내고 다른 친구들에게 GPS가 준비되었다고 알려줘요. 🧭

3.  세상 보기 (AR 카메라 -> AR Foundation): 스마트폰의 카메라와 움직임 센서가 함께 작동해요 (이게 AR Foundation이에요). 이걸로 앱은 실제 세상을 '보고', 바닥 같은 모양을 이해하고, 우리가 폰을 어떻게 움직이는지 따라가요.

4.  지도 위에 코인 놓기 (PathGenerator + LocationManager + AR Foundation -> CoinSpawner):

    * 사용자가 '코인 생성' 버튼을 누르면...

    * CoinSpawner가 지도 데이터(PathGenerator), 내 현재 위치와 방향(LocationManager), 그리고 바닥 정보(AR Foundation)를 다 가져와요.

    * 이걸로 계산해서 실제 세상의 어디에 코인이 보여야 할지 정확히 알아낸 다음, 북쪽 방향에 맞춰서 바닥 위에 가상의 AR 코인들을 만들어줘요. ✨

5.  게임 진행 (AR 코인 + LocationManager + AR Foundation -> GameManager):

    * GameManager는 만들어진 코인들을 전부 기억해요.

    * 실시간 GPS 위치(LocationManager)와 코인들의 위치를 계속 비교해요.

    * 또 AR Foundation을 통해 내가 가상 공간에서 어디를 보고 있는지도 알아요.

    * 내가 코인에 충분히 가까워지면(collectDistance), GameManager가 코인을 사라지게 하고, 소리를 내고 🔊, 점수를 올려줘요.

6.  정보 보여주기 ( GameManager -> UI 요소): GameManager는 화면의 **글자(Text)**에게 지금 뭘 보여줘야 할지 알려줘요 (예: "다음 코인까지: 5.6m", "Coins: 1 / 13").
```
## 🎓 캡스톤 디자인 정보

* **학과:** 인공지능학과
* **지도교수:** 김범성 교수
* **프로젝트명:** AR기반 등하교 서포트 서비스 (AR IOS 버전 구현)
* **수행기간:** 2025년 3월 ~ 2025년 11월
* **참여학생:**
    * 장평화 (202263018) (AR IOS 파트 담당)
    * 김강민 (202263044)
    * 김동윤 (202463006)
    * 김병윤 (202263001)
    * 최혁수 (202263061)
    * 채한빈 (202263020)
    * 홍우진 (202263042)

## 📈 기대효과 및 활용방안 
### **(AR 관점)**
* 데이터 기반 안전 경로를 직관적이고 몰입감 있게 시각화하여 사용자 이해도 증진
* 게임 요소를 통한 즐거운 길 안내 경험 제공으로 아이들의 자발적 참여 유도
* 기존 지도 앱의 한계를 넘어선 현실 증강 기반의 차세대 내비게이션 가능성 제시

### **(전체 관점)**
* **직접적 효과**
    * 아이 스스로 안전한 길을 인식하고 따라가도록 유도
    * 어린이 보행 안전에 대한 자율성과 주체성 향상
    * 기존 수동적 위치 추적 서비스와 차별화된 능동적 안전 경험 제공
* **확장 가능성**
    * 고령자, 발달장애인 등 보행 약자에게도 확장 적용
    * 교육기관, 공공기관 등에서 아동 안전 기술로 활용
    * 사회 전반의 안전 문화 확산과 보행자 중심의 기술 환경 조성에 기여