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

## 빌드가 안 될 때

- **컴파일 에러: `Firebase` 네임스페이스를 못 찾음** → SDK import 가 누락된 상태입니다. 위 절차를 다시 실행하세요.
- **Android 빌드 시 의존성 누락** → Unity 메뉴 `Assets > External Dependency Manager > Android Resolver > Force Resolve` 를 실행합니다.
- **iOS 빌드 시 `libFirebaseCpp*.a` 누락** → `Assets/Plugins/iOS/Firebase/` 가 비어 있는지 확인. 비어 있다면 `FirebaseApp.unitypackage` 가 제대로 import 되지 않은 것입니다.
