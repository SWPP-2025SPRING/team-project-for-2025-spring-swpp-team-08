# Team Gradu8

## Asset 파일 관리 규칙

### 특정 Scene에서만 사용되는 Asset

- 해당 Scene 이름에 해당하는 폴더 안에 넣는다.
  - ex: `Assets/Scripts/Stage1/SomeScript.cs`, `Assets/Models/Stage4/SomeModel.fbx` 등
- 스크립트의 경우 이름이 겹칠 수 있으므로 namespace를 사용한다.
  - ex:
    ```csharp
    namespace Stage1
    {
        public class SomeCommonName
        {
            /* ... */
        }
    }
    ```

### 여러 Scene에서 공통으로 사용되는 Object 관련 Asset

- Commons 폴더 안에 넣는다.
  - ex: `Assets/Prefabs/Commons/Player.prefab` 등

### Manager 관련 Asset

- Managers 폴더 안에 넣는다.
  - ex: `Assets/Scripts/Managers/GameManager.cs` 등