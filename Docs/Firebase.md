# Firebase 설치 가이드

이 프로젝트는 Firebase Crashlytics / Analytics 를 사용합니다. **Firebase Unity SDK 본체는 git에 올리지 않습니다** (용량이 약 430MB라 레포가 비대해지므로). 각 작업자가 동일 버전의 `.unitypackage`를 직접 import 합니다.

## 사용 중인 버전

**Firebase Unity SDK `13.10.0`**

다른 버전을 임포트하지 마세요. 빌드 결과물에 차이가 생길 수 있습니다.

## 최초 설치 절차 (신규 작업자)

1. [Firebase Unity SDK 다운로드 페이지](https://firebase.google.com/download/unity) 에서 **13.10.0** zip 을 받습니다.
2. zip을 풀고, Unity 프로젝트를 연 상태에서 다음 `.unitypackage` 3개를 차례로 import 합니다.
   - `FirebaseApp.unitypackage`
   - `FirebaseAnalytics.unitypackage`
   - `FirebaseCrashlytics.unitypackage`
3. import 시 EDM4U(Google External Dependency Manager) 가 의존성 해석을 묻습니다. **Force Resolve** 를 실행해 Android 의존성을 받아둡니다 (`Assets/Plugins/Android/FirebaseApp.androidlib/`, `Assets/GeneratedLocalRepo/` 가 자동 생성됩니다).
4. `Assets/google-services.json` 은 git에 들어 있으므로 별도로 받지 않아도 됩니다. 만약 빠져 있다면 Firebase Console 에서 받아 같은 위치에 둡니다.

## git 에서 제외되는 경로 (참고)

`.gitignore` 에 다음 경로들이 들어 있습니다. **import 후 로컬에는 존재해야 정상이며**, 다른 작업자에게 전달되지 않을 뿐입니다.

### Firebase SDK 본체 (직접 import 로 생김)

- `Assets/Firebase/`
- `Assets/Plugins/iOS/Firebase/`
- `Assets/Plugins/tvOS/`
- `Assets/Editor Default Resources/Firebase/`

### EDM4U 자동 생성물 (Unity 가 알아서 다시 만듦)

- `Assets/GeneratedLocalRepo/`
- `Assets/Plugins/Android/FirebaseApp.androidlib/`
- `Assets/StreamingAssets/google-services-desktop.json`

## Android 빌드 호환성 (중요)

Unity 2022.3 + Firebase Unity SDK 13.x 조합은 **R8/D8 dexer override 가 필수**입니다. 이 설정 없이 Android 릴리스 빌드를 실행하면 `D8: ... StackOverflowError` 로 실패합니다.

### 원인

- Unity 2022.3 이 번들하는 AGP 7.4.2 의 D8 dexer (R8 4.0.x 계열) 는 Kotlin 1.9 메타데이터를 파싱할 때 무한 재귀에 빠지는 결함이 있습니다.
- Firebase 13.10.0 의 내장 AAR (`firebase-common 22.0.1`, `firebase-crashlytics 20.0.5`, `firebase-sessions 3.0.5`, `play-services-measurement-api 23.2.0`) 은 Kotlin 1.9.x 로 컴파일되어 있어 이 결함을 그대로 유발합니다.
- AAR 에 새겨진 `@kotlin.Metadata` 는 Gradle `force` 로 바꿀 수 없으므로, R8/D8 자체를 신규 버전으로 교체하는 방식으로 우회합니다.

### 구현 위치

- [Assets/Plugins/Android/baseProjectTemplate.gradle](../Assets/Plugins/Android/baseProjectTemplate.gradle) 상단의 `// FIREBASE_R8_OVERRIDE` 주석 블록에서 `com.android.tools:r8:8.2.47` 을 `buildscript.dependencies.classpath` 로 주입합니다.
- 같은 파일의 `allprojects.configurations.all.resolutionStrategy` 안에 Kotlin 런타임 (`kotlin-stdlib`, `kotlin-parcelize-runtime`, `kotlinx-coroutines-*` 등) 을 1.8.22 / 1.7.3 으로 고정하는 `force` 선언이 있습니다. Firebase 가 1.9 계열 transitive 를 참조해도 D8 가 처리 가능한 버전으로 일치시키는 용도입니다.

### 제거 가능 조건

다음 중 하나가 충족되면 R8 override 블록을 제거해도 됩니다.

- Unity 를 **2023.2 이상** 으로 업그레이드 (번들 AGP 가 갱신되어 D8 결함 해소).
- Firebase Unity SDK 를 **Kotlin 1.8 시절 버전 (11.x 등)** 으로 다운그레이드.

제거 전에는 해당 블록을 일시적으로 비활성화한 상태에서 빌드가 통과하는지 먼저 확인합니다.

### 관련 주의 사항

- `firebase-crashlytics-gradle` plugin 은 **2.9.9** 를 사용합니다. 3.x 라인은 Java 17 을 요구하는데, Unity 2022.3 의 번들 Gradle 은 Java 11 환경입니다.
- `com.google.gms.google-services` Gradle plugin 은 **적용하지 않습니다**. Firebase Unity SDK 가 자체 빌드 스크립트로 `Assets/google-services.json` 을 `FirebaseApp.androidlib/res/values/google-services.xml` 로 변환하는 별도 파이프라인을 운영하므로, Gradle plugin 을 함께 적용하면 두 파이프라인이 충돌하여 `google-services.json is missing` 오류가 발생합니다.
- `firebase-crashlytics` Gradle plugin 은 **`launcherTemplate.gradle` (`:launcher` 모듈)** 에만 적용합니다. `mainTemplate.gradle` (`:unityLibrary` 모듈) 에 적용하면 `needs com.android.application` 오류가 발생합니다.

## 빌드가 안 될 때

- **컴파일 에러: `Firebase` 네임스페이스를 못 찾음** → SDK import 가 누락된 상태입니다. 위 절차를 다시 실행하세요.
- **Android 빌드 시 의존성 누락** → Unity 메뉴 `Assets > External Dependency Manager > Android Resolver > Force Resolve` 를 실행합니다.
- **iOS 빌드 시 `libFirebaseCpp*.a` 누락** → `Assets/Plugins/iOS/Firebase/` 가 비어 있는지 확인. 비어 있다면 `FirebaseApp.unitypackage` 가 제대로 import 되지 않은 것입니다.
