# Ground Tile Generator

**`ground_generator.py`**는 우리가 개발 중인 게임에서 사용할 바닥 타일(.obj 모델)을 자동으로 생성해주는 Python 스크립트입니다. 이 도구를 통해 다양한 형태의 도로(직선, 곡선, S자, 경사로 등)를 빠르게 만들 수 있습니다. main()을 수정해 원하는 타일을 만들어보세요.

---

## ✅ 시작 전 준비

1. Python 3.13.3 이상을 설치하세요.
2. 필요한 라이브러리를 설치하려면, 다음 명령어를 실행하세요:

```bash
pip install -r requirements.txt
```

---

## ✅ 실행 방법

```bash
python ground_generator.py
```

스크립트를 실행하면 `Assets/Tools/Ground/export` 폴더에 여러 개의 `.obj` 3D 타일 파일이 생성됩니다. 이 파일들은 Unity와 같은 게임 엔진에서 사용 가능합니다.

---

## ✅ 생성 가능한 도로 종류와 조정 가능한 파라미터

각 함수는 도로 형태를 생성하며, 파라미터를 조절해 다양하게 응용할 수 있습니다.

### 1. `generate_straight()`: 직선형 도로
- **tile_length / tile_width**: 도로의 크기
- **resolution**: 디테일 정도 (값이 클수록 면이 많고 부드러움)
- **bump_strength**: 표면의 울퉁불퉁함 정도 (0이면 완전히 평평)
- **sigma**: 울퉁불퉁함의 부드러움 정도
- **thickness**: 도로 두께

---

### 2. `generate_cant()`: 3차 함수 모양으로 굽은 도로
- **curve_amount**: 굽은 정도
- 나머지 파라미터는 `generate_straight()`와 동일

---

### 3. `generate_s_curve()`: S자 도로
- **flip**: True로 설정하면 반대 방향 S자
- **amplitude**: 굽은 정도
- **tile_length / tile_width**: 도로 크기

---

### 4. `generate_curve()`: 곡선 도로 (90도 또는 사용자 지정 각도)
- **radius_outer**: 바깥쪽 곡선 반지름
- **angle_rad**: 곡선 각도 (라디안, 예: np.radians(90))
- **width**: 도로 폭
- **res_r / res_theta**: 정점 수 (디테일 정도)

---

### 5. `generate_spiral()`: 나선형 도로
- **height**: 위로 올라가는 높이
- **angle_rad**: 전체 감기는 각도 (기본은 360도)
- **res_r / res_theta**: 정점 수

---

### 6. `generate_ramp_curve()`: 경사 곡선 도로
- **curve_dir**: `"left"` 또는 `"right"`로 방향 지정
- **height**: 경사 높이
- 나머지 파라미터는 `generate_curve()`와 동일

---

## ✅ 결과 파일

스크립트를 실행하면 `.obj` 형식의 파일이 `Assets/Tools/Ground/export` 폴더에 저장됩니다. 예:

```
Assets/Tools/Ground/export
├── Straight.obj
├── CantMediumLeft.obj
├── SCurveRight.obj
├── Spiral.obj
└── ...
```

---

## ✅ 팁

- `resolution` 값을 너무 높이면 모델 용량이 커질 수 있어요.
- `bump_strength`를 0으로 설정하면 평평한 모델을 만들 수 있어요.
- `.obj` 파일은 Blender, Unity 등에서 바로 열 수 있습니다.

---

## ✅ 오류가 났을 때?

- **파일이 이미 있다는데요?**
  → `Assets/Tools/Ground/export` 폴더의 기존 파일을 삭제하거나 이름을 바꿔서 실행하세요.

- **모델이 비어있어요.**
  → `resolution`이 너무 작거나, `thickness`가 0일 수 있습니다.

- **Unity에서 투명하게 보여요.**
  → 표면 방향(winding order)이 잘못되었을 수 있어요. 대부분은 CCW 기준으로 처리되어 있습니다.

---

## ✅ 기타

이 스크립트는 게임 개발에 필요한 커스텀 지형 생성을 쉽게 하기 위해 만들어졌습니다. 필요에 따라 `.py` 파일을 수정해 더 다양한 타일을 만들 수 있습니다.