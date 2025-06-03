import numpy as np
from scipy.ndimage import gaussian_filter
import os

# ✅ 범프 노이즈 생성 함수
# shape: 노이즈 배열의 형태 (예: (100, 100))
# sigma: 가우시안 필터의 시그마 값 (클수록 부드러워짐)
# bump_strength: 범프의 강도
# falloff: 폴오프 마스크 배열 (범프가 적용될 영역과 강도를 조절)
def generate_noise_bump(shape, sigma, bump_strength, falloff):
    np.random.seed(42) # 일관된 노이즈를 위해 시드 고정
    noise = np.random.rand(*shape)
    smooth_noise = gaussian_filter(noise, sigma=sigma)
    smooth_noise = (smooth_noise - 0.5) * 2 # -1에서 1 사이로 정규화
    return smooth_noise * bump_strength * falloff

# ✅ .obj 파일 저장 함수
# vertices: 정점 리스트 [(x, y, z), ...]
# faces: 면 리스트 [(v1_idx, v2_idx, v3_idx, ...), ...] (0-인덱싱)
# path: 저장할 파일 경로
def save_obj(vertices, faces, path):
    if os.path.exists(path):
        raise FileExistsError(f"파일이 이미 존재합니다: {path}")
    os.makedirs(os.path.dirname(path), exist_ok=True)

    # 표준 OBJ: 정점은 1-인덱싱됨
    lines = [f"v {vx:.6f} {vy:.6f} {vz:.6f}" for vx, vy, vz in vertices]
    lines += [f"f {' '.join(str(i+1) for i in face)}" for face in faces] # 0-인덱싱을 1-인덱싱으로 변환
    with open(path, "w") as f:
        f.write("\n".join(lines))
    print(f"✅ 저장됨: {path}")


# ✅ 외곽 루프 기반 두께 생성 함수 (나선형 외 다른 타일용)
# vertices: 원본 메쉬의 정점 리스트
# faces: 원본 메쉬의 면 리스트
# edge_parts: 외곽선 정보를 담은 딕셔너리 {'bottom': [...], 'right': [...], 'top': [...], 'left': [...]}
# thickness: 돌출시킬 두께
# edge_resolution: 옆면 및 밑면 생성 시 외곽선 샘플링 해상도 계수
# reverse_winding: 원본 윗면의 와인딩이 시계방향(CW)인지 여부 (True면 CW, False면 CCW)
def extrude_thickness_from_top(vertices, faces, edge_parts, thickness, edge_resolution, reverse_winding=False):
    if thickness <= 0:
        return vertices, faces

    edge_resolution_scaled = 4 * edge_resolution # 샘플링 스텝 계산용 해상도 스케일링
    base_idx = len(vertices) # 새로 추가될 밑면 정점들의 시작 인덱스
    
    # 밑면 정점 생성: 위쪽 정점에서 y축으로 thickness만큼 아래로 이동
    bottom_vertices_list = [(vx, vy_orig - thickness, vz) for (vx, vy_orig, vz) in vertices]
    vertices += bottom_vertices_list

    # 외곽 전체 루프 구성
    full_loop = edge_parts['bottom'] + edge_parts['right'] + edge_parts['top'] + edge_parts['left']
    loop_len = len(full_loop)

    if loop_len == 0 or edge_resolution_scaled == 0:
        if not full_loop:
            print(f"경고: 압출을 위한 전체 루프가 비어 있습니다. 옆면/바닥면이 생성되지 않았습니다.")
        return vertices, faces
    
    step = max(1, loop_len // edge_resolution_scaled) # 샘플링 간격
    sampled_indices = [full_loop[i] for i in range(0, loop_len, step)] # 외곽선 샘플링

    # 샘플링된 루프 닫기 (시작점과 끝점이 다르면 시작점 추가)
    if sampled_indices and sampled_indices[0] != sampled_indices[-1]:
        sampled_indices.append(sampled_indices[0])

    if not sampled_indices or len(sampled_indices) < 2: # 유효한 루프인지 확인
        print(f"경고: 압출을 위한 샘플링된 루프가 너무 짧습니다. 옆면/바닥면이 생성되지 않았습니다.")
        return vertices, faces

    # 옆면 생성: 샘플링된 외곽선 정점들을 이용하여 위쪽과 아래쪽을 잇는 면 생성
    for i in range(len(sampled_indices) - 1):
        a = sampled_indices[i]       # 현재 위쪽 정점
        b = sampled_indices[i + 1]   # 다음 위쪽 정점
        a_bot = a + base_idx         # 현재 아래쪽 정점
        b_bot = b + base_idx         # 다음 아래쪽 정점
        if reverse_winding: # 원본 윗면이 CW인 경우
            faces.append((b, a, a_bot, b_bot)) # 옆면은 외부에서 볼 때 CCW가 되도록 조정
        else: # 원본 윗면이 CCW인 경우
            faces.append((a, b, b_bot, a_bot)) # 옆면은 외부에서 볼 때 CCW

    # 밑면 생성: 샘플링된 외곽선의 아래쪽 정점들을 이용하여 밑면 폴리곤 생성
    bottom_loop_verts_indices = [idx + base_idx for idx in sampled_indices[:-1]] # 중복 제거된 밑면 루프 정점

    if len(bottom_loop_verts_indices) >= 3: # 폴리곤은 최소 3개의 정점 필요
        if not reverse_winding: # 원본 윗면이 CCW였던 경우, 밑면은 그대로 CCW (아래에서 봤을 때)
            faces.append(tuple(bottom_loop_verts_indices))
        else: # 원본 윗면이 CW였던 경우, 밑면은 CW이므로 뒤집어서 CCW로 (아래에서 봤을 때)
            faces.append(tuple(reversed(bottom_loop_verts_indices)))
    else:
        print(f"경고: 밑면을 만들기에 정점이 충분하지 않습니다 ({len(bottom_loop_verts_indices)}개 발견).")

    return vertices, faces

# ✅ 경계 루프 인덱스 생성 함수 (정사각형 타일용)
# resolution: 한 변의 정점 개수
def get_square_edge_loop(resolution):
    bottom = [i for i in range(resolution)]
    right = [i * resolution + (resolution - 1) for i in range(1, resolution)] # 시작점 중복 피하기 위해 1부터
    top = [resolution * (resolution - 1) + i for i in range(resolution - 2, -1, -1)] # 끝점, 시작점 중복 피하기
    left = [(i * resolution) for i in range(resolution - 2, 0, -1)] # 끝점, 시작점 중복 피하기
    return {"bottom": bottom, "right": right, "top": top, "left": left}


# ✅ Cant 도로 생성 함수
# name: 생성될 모델의 이름
# output_dir: 모델이 저장될 디렉토리 경로
# curve_amount: 도로의 한쪽이 기울어지는 양 (캔트 양)
# tile_size: 타일의 X, Z 크기
# resolution: 타일을 구성하는 X, Z 방향 그리드의 정점 수
# edge_resolution: 두께 생성 시 옆면/밑면 샘플링 해상도 계수
# bump_strength: 표면 범프의 강도
# sigma: 범프 노이즈의 가우시안 필터 시그마 값
# thickness: 도로의 두께
def generate_cant(name, output_dir, curve_amount, tile_size=4.0, resolution=100, edge_resolution=25, bump_strength=0.3, sigma=3.0, thickness=1.0):
    if resolution < 10: 
        resolution = 10

    x_coords = np.linspace(-tile_size/2, tile_size/2, resolution)
    z_coords = np.linspace(-tile_size/2, tile_size/2, resolution) # 도로는 XZ 평면에 놓임
    xv, zv = np.meshgrid(x_coords, z_coords)
    
    y_center_offset = thickness / 2 # 도로 두께의 중간 Y 좌표

    # 범프 폴오프 마스크 생성
    edge_margin = tile_size * 0.04
    falloff_x = np.clip((tile_size/2 - np.abs(xv)) / edge_margin, 0, 1)
    falloff_z = np.clip((tile_size/2 - np.abs(zv)) / edge_margin, 0, 1)
    falloff = (falloff_x * falloff_z)

    # Cant 효과 적용 (Z 좌표에 따라 X 좌표 오프셋)
    x_offset = np.sin(zv / tile_size * np.pi) * curve_amount
    xv_curved = xv + x_offset

    bump = generate_noise_bump((resolution, resolution), sigma, bump_strength, falloff)
    
    vertices = []
    for i in range(resolution): # Z축 방향 (행)
        for j in range(resolution): # X축 방향 (열)
            vertices.append((xv_curved[i,j], y_center_offset + bump[i,j], zv[i,j]))
            
    # 윗면 생성: (v00, v10, v11, v01) 순서로 정의하여 위에서 볼 때 CCW (+Y 법선)
    # v00=(r,c), v01=(r,c+1), v10=(r+1,c), v11=(r+1,c+1)
    # 면: (r,c), (r+1,c), (r+1,c+1), (r,c+1) -> CCW
    faces = []
    for r in range(resolution - 1): # 행 반복
        for c in range(resolution - 1): # 열 반복
            v00 = r * resolution + c
            v01 = r * resolution + c + 1
            v10 = (r + 1) * resolution + c
            v11 = (r + 1) * resolution + c + 1
            faces.append((v00, v10, v11, v01)) # 사용자가 수정한 CCW 와인딩

    edge_parts = get_square_edge_loop(resolution)
    # 윗면이 CCW이므로 reverse_winding=False 전달
    vertices, faces = extrude_thickness_from_top(vertices, faces, edge_parts, thickness, edge_resolution, reverse_winding=False)
    save_obj(vertices, faces, os.path.join(output_dir, f"{name}.obj"))

# ✅ S-Curve 도로 생성 함수
# name: 생성될 모델의 이름
# output_dir: 모델이 저장될 디렉토리 경로
# flip: S-Curve의 방향 반전 여부
# tile_length: 타일의 Z축 길이
# tile_width: 타일의 X축 폭
# resolution: 타일을 구성하는 X, Z 방향 그리드의 정점 수
# edge_resolution: 두께 생성 시 옆면/밑면 샘플링 해상도 계수
# amplitude: S-Curve의 진폭
# bump_strength: 표면 범프의 강도
# sigma: 범프 노이즈의 가우시안 필터 시그마 값
# thickness: 도로의 두께
def generate_s_curve(name, output_dir, flip=False, tile_length=8.0, tile_width=4.0,
                     resolution=100, edge_resolution=25,
                     amplitude=2.0, bump_strength=0.3, sigma=3.0, thickness=1.0):
    x_coords = np.linspace(-tile_width/2, tile_width/2, resolution)
    z_coords = np.linspace(-tile_length/2, tile_length/2, resolution)
    xv, zv = np.meshgrid(x_coords, z_coords)
    
    y_center_offset = thickness / 2

    direction = -1 if flip else 1
    edge_margin_x = tile_width * 0.04
    edge_margin_z = tile_length * 0.04
    falloff_x = np.clip((tile_width/2 - np.abs(xv)) / edge_margin_x, 0, 1)
    falloff_z = np.clip((tile_length/2 - np.abs(zv)) / edge_margin_z, 0, 1)
    falloff = (falloff_x * falloff_z)

    # S-Curve 형태 적용 (Z 좌표에 따라 X 좌표 오프셋)
    x_offset = direction * amplitude * np.sin(np.pi * zv / tile_length)
    xv_s = xv + x_offset
    bump = generate_noise_bump((resolution, resolution), sigma, bump_strength, falloff)

    vertices = []
    for i in range(resolution):
        for j in range(resolution):
            vertices.append((xv_s[i,j], y_center_offset + bump[i,j], zv[i,j]))

    # 윗면 생성 (CCW, +Y 법선)
    faces = []
    for r in range(resolution - 1):
        for c in range(resolution - 1):
            v00 = r * resolution + c
            v01 = r * resolution + c + 1
            v10 = (r + 1) * resolution + c
            v11 = (r + 1) * resolution + c + 1
            faces.append((v00, v10, v11, v01)) # 사용자가 수정한 CCW 와인딩

    edge = get_square_edge_loop(resolution)
    # 윗면이 CCW이므로 reverse_winding=False 전달
    vertices, faces = extrude_thickness_from_top(vertices, faces, edge, thickness, edge_resolution, reverse_winding=False)
    save_obj(vertices, faces, os.path.join(output_dir, f"{name}.obj"))

# ✅ Straight 도로 생성 함수
# name: 생성될 모델의 이름
# output_dir: 모델이 저장될 디렉토리 경로
# tile_length: 타일의 Z축 길이
# tile_width: 타일의 X축 폭
# resolution: 타일을 구성하는 X, Z 방향 그리드의 정점 수
# edge_resolution: 두께 생성 시 옆면/밑면 샘플링 해상도 계수
# bump_strength: 표면 범프의 강도
# sigma: 범프 노이즈의 가우시안 필터 시그마 값
# thickness: 도로의 두께
def generate_straight(name, output_dir, tile_length=4.0, tile_width=4.0,
                      resolution=100, edge_resolution=25,
                      bump_strength=0.3, sigma=3.0, thickness=1.0):
    x_len = tile_width
    z_len = tile_length

    x_coords = np.linspace(-x_len/2, x_len/2, resolution)
    z_coords = np.linspace(-z_len/2, z_len/2, resolution)
    xv, zv = np.meshgrid(x_coords, z_coords)
    
    y_center_offset = thickness / 2

    edge_margin_x = x_len * 0.04
    edge_margin_z = z_len * 0.04
    falloff_x = np.clip((x_len/2 - np.abs(xv)) / edge_margin_x, 0, 1)
    falloff_z = np.clip((z_len/2 - np.abs(zv)) / edge_margin_z, 0, 1)
    falloff = (falloff_x * falloff_z)

    bump = generate_noise_bump((resolution, resolution), sigma, bump_strength, falloff)
    
    vertices = []
    for i in range(resolution):
        for j in range(resolution):
            vertices.append((xv[i,j], y_center_offset + bump[i,j], zv[i,j]))

    # 윗면 생성 (CCW, +Y 법선)
    faces = []
    for r in range(resolution - 1):
        for c in range(resolution - 1):
            v00 = r * resolution + c
            v01 = r * resolution + c + 1
            v10 = (r + 1) * resolution + c
            v11 = (r + 1) * resolution + c + 1
            faces.append((v00, v10, v11, v01)) # 사용자가 수정한 CCW 와인딩

    edge = get_square_edge_loop(resolution)
    # 윗면이 CCW이므로 reverse_winding=False 전달
    vertices, faces = extrude_thickness_from_top(vertices, faces, edge, thickness, edge_resolution, reverse_winding=False)
    save_obj(vertices, faces, os.path.join(output_dir, f"{name}.obj"))

# ✅ Curve 도로 생성 함수
# name: 생성될 모델의 이름
# output_dir: 모델이 저장될 디렉토리 경로
# radius_outer: 곡선 도로의 바깥쪽 반지름
# angle_rad: 곡선 도로의 총 각도 (라디안)
# width: 도로의 폭
# res_r: 반지름 방향 해상도 (정점 수)
# res_theta: 각도 방향 해상도 (정점 수)
# edge_resolution: 두께 생성 시 옆면/밑면 샘플링 해상도 계수
# bump_strength: 표면 범프의 강도
# sigma: 범프 노이즈의 가우시안 필터 시그마 값
# thickness: 도로의 두께
def generate_curve(name, output_dir, radius_outer, angle_rad=np.radians(90),
                   width=4.0, res_r=100, res_theta=100, edge_resolution=25,
                   bump_strength=0.3, sigma=(2.0, 2.0), thickness=1.0):
    radius_inner = radius_outer - width
    edge_margin = width * 0.04
    safe_theta_margin_num = max(1e-9, angle_rad * 0.01) 
    safe_theta_margin_den_end = max(1e-9, angle_rad - (angle_rad*0.01))


    r_coords = np.linspace(radius_inner, radius_outer, res_r)     # 반지름 방향 좌표
    theta_coords = np.linspace(0, angle_rad, res_theta)          # 각도 방향 좌표
    rv, thetav = np.meshgrid(r_coords, theta_coords, indexing='ij') # ij 인덱싱: rv가 행(r), thetav가 열(theta)

    x = rv * np.cos(thetav)
    z = rv * np.sin(thetav)
    
    falloff_r = np.clip((rv - radius_inner)/edge_margin, 0, 1) * np.clip((radius_outer - rv)/edge_margin, 0, 1)
    falloff_theta = np.ones_like(thetav)
    if angle_rad > 1e-6: # 유효한 각도일 때만 폴오프 적용
        falloff_theta = np.clip(thetav/safe_theta_margin_num, 0, 1) * np.clip((angle_rad - thetav)/safe_theta_margin_den_end, 0, 1)
        
    bump_noise = generate_noise_bump((res_r, res_theta), sigma, bump_strength, falloff_r * falloff_theta)
    y_center_offset = thickness / 2 
    
    vertices_list = []
    for i in range(res_r):     # r 인덱스 (행)
        for j in range(res_theta): # theta 인덱스 (열)
            vertices_list.append((x[i,j], y_center_offset + bump_noise[i,j], z[i,j]))

    # 윗면 생성 (CCW, +Y 법선)
    # (r_idx, theta_idx) = (i, j)
    # v00 = (i,j), v01 = (i,j+1), v10 = (i+1,j), v11 = (i+1,j+1)
    # 면: (v00, v01, v11, v10) -> (i,j), (i,j+1), (i+1,j+1), (i+1,j)
    top_faces = []
    for i in range(res_r - 1):
        for j in range(res_theta - 1):
            v00 = i * res_theta + j        
            v01 = i * res_theta + j + 1    
            v10 = (i + 1) * res_theta + j  
            v11 = (i + 1) * res_theta + j + 1
            top_faces.append((v00, v01, v11, v10)) # 사용자가 수정한 CCW 와인딩

    # Curve에 맞는 edge_parts 정의
    part1 = [i * res_theta + 0 for i in range(res_r)] # theta = 0 (시작 각도), r_inner 부터 r_outer 까지
    part2 = []
    if res_theta > 1 : part2 = [(res_r - 1) * res_theta + j for j in range(1, res_theta)] # r = r_outer, theta 증가 (첫 정점 제외)
    part3 = []
    if res_r > 1 and res_theta > 1 : part3 = [i * res_theta + (res_theta - 1) for i in range(res_r - 2, -1, -1)] # theta = max_angle, r_outer 부터 r_inner 까지 (양 끝 정점 제외)
    part4 = []
    if res_r > 1 and res_theta > 2 : part4 = [0 * res_theta + j for j in range(res_theta - 2, 0, -1)] # r = r_inner, theta 감소 (양 끝 정점 제외)
    
    effective_edge_parts_curve = { "bottom": part1, "right": part2, "top": part3, "left": part4 }

    # 윗면이 CCW이므로 reverse_winding=False 전달
    final_vertices, final_faces = extrude_thickness_from_top(
        vertices_list, top_faces, effective_edge_parts_curve, thickness, edge_resolution, 
        reverse_winding=False
    )
    save_obj(final_vertices, final_faces, os.path.join(output_dir, f"{name}.obj"))


# ✅ Spiral 도로 생성 함수 (자체 두께 및 옆면/밑면 처리)
# name: 생성될 모델의 이름
# output_dir: 모델이 저장될 디렉토리 경로
# reverse: 나선 방향 반전 여부 (True면 반대 방향으로 감김)
# radius_outer: 나선의 바깥쪽 반지름
# width: 나선 도로의 폭
# height: 나선의 총 높이
# angle_rad: 나선이 감기는 총 각도 (라디안, 예: 360도 = 2*np.pi)
# res_r: 반지름 방향 해상도 (정점 수)
# res_theta: 각도 방향 해상도 (정점 수)
# edge_resolution: (현재 직접 생성 로직에서는 미사용)
# bump_strength: 표면 범프의 강도
# sigma: 범프 노이즈의 가우시안 필터 시그마 값
# thickness: 도로의 두께
def generate_spiral(name, output_dir, reverse=False,
                    radius_outer=8.0, width=4.0, height=8.0,
                    angle_rad=np.radians(360), res_r=50, res_theta=100,
                    edge_resolution=50, 
                    bump_strength=0.3, sigma=(3.0, 1.0), thickness=1.0):
    radius_inner = radius_outer - width
    edge_margin = width * 0.04 
    
    theta_coords_ref = np.linspace(0, angle_rad, res_theta) # 높이 정규화 및 범프 폴오프용 (0 ~ angle_rad)
    
    current_theta_coords = np.linspace(0, angle_rad, res_theta) # 실제 정점 생성용 theta 배열
    if reverse: 
        current_theta_coords = current_theta_coords[::-1] # 경로 반전

    r_coords = np.linspace(radius_inner, radius_outer, res_r)
    
    rv, thetav = np.meshgrid(r_coords, current_theta_coords, indexing='ij')

    x = rv * np.cos(thetav)
    z = rv * np.sin(thetav)
    
    # 높이 계산: current_theta_coords 범위(0~angle_rad 또는 angle_rad~0)를 0~1로 정규화 후 높이 적용
    normalized_theta_for_height = (thetav - current_theta_coords[0]) / (current_theta_coords[-1] - current_theta_coords[0] + 1e-9) # 분모 0 방지
    if reverse: # 경로가 반전되면 높이도 반대로 (위에서 아래로)
        base_y = (1.0 - normalized_theta_for_height) * height 
    else: # 기본 경로는 아래에서 위로
        base_y = normalized_theta_for_height * height 

    # 범프 폴오프는 항상 0 ~ angle_rad 기준 theta_coords_ref 사용
    _, falloff_thetav_ref = np.meshgrid(r_coords, theta_coords_ref, indexing='ij')
    theta_falloff_margin_num = max(1e-9, angle_rad * 0.01)
    theta_falloff_margin_den_end = max(1e-9, angle_rad - theta_falloff_margin_num)
    
    falloff_r = np.clip((rv - radius_inner)/edge_margin, 0, 1) * np.clip((radius_outer - rv)/edge_margin, 0, 1)
    falloff_theta = np.ones_like(falloff_thetav_ref)
    if angle_rad > 1e-6: # 유효한 각도일 때만 폴오프 적용
         falloff_theta = np.clip(falloff_thetav_ref/theta_falloff_margin_num, 0, 1) * \
                         np.clip((angle_rad - falloff_thetav_ref)/theta_falloff_margin_den_end, 0, 1)
    
    bump = generate_noise_bump((res_r, res_theta), sigma, bump_strength, falloff_r * falloff_theta)

    y_center_offset = thickness / 2 
    y_top_surface_values = base_y + bump 
    
    vertices = []
    for i in range(res_r): 
        for j in range(res_theta): 
            vertices.append((x[i,j], y_top_surface_values[i,j] + y_center_offset, z[i,j]))

    # 윗면 생성 (CCW, +Y 법선)
    # (i,j), (i,j+1), (i+1,j+1), (i+1,j)
    faces = []
    for i in range(res_r - 1): 
        for j in range(res_theta - 1): 
            v00 = i * res_theta + j
            v01 = i * res_theta + j + 1
            v10 = (i + 1) * res_theta + j
            v11 = (i + 1) * res_theta + j + 1
            if reverse:
                faces.append((v00, v10, v11, v01))
            else:
                faces.append((v00, v01, v11, v10)) # 사용자가 수정한 CCW 와인딩

    if thickness > 0:
        base_idx = len(vertices) 
        
        bottom_vertices_list = []
        for i in range(res_r):
            for j in range(res_theta):
                vx_top, vy_top_with_offset, vz_top = vertices[i*res_theta + j]
                bottom_vertices_list.append((vx_top, vy_top_with_offset - thickness, vz_top))
        vertices.extend(bottom_vertices_list)

        # 밑면 생성 (위에서 볼 때 CW = 아래에서 볼 때 CCW, -Y 법선)
        # 윗면 (v00, v01, v11, v10)의 밑면 대응은 (v00b, v10b, v11b, v01b)
        for i in range(res_r - 1):
            for j in range(res_theta - 1):
                v00_bot = base_idx + (i * res_theta + j)
                v01_bot = base_idx + (i * res_theta + j + 1)
                v10_bot = base_idx + ((i + 1) * res_theta + j)
                v11_bot = base_idx + ((i + 1) * res_theta + j + 1)
                if reverse: 
                    faces.append((v00_bot, v01_bot, v11_bot, v10_bot))
                else:
                    faces.append((v00_bot, v10_bot, v11_bot, v01_bot)) # 사용자가 수정한 밑면 CCW 와인딩

        # 옆면 생성
        idx_outer_r = res_r - 1 # 바깥쪽 반지름 인덱스
        for j in range(res_theta - 1): # theta 방향으로 면 생성
            v_top_curr = idx_outer_r * res_theta + j
            v_top_next = idx_outer_r * res_theta + j + 1
            v_bot_curr = base_idx + v_top_curr
            v_bot_next = base_idx + v_top_next
            if not reverse: # 기본 방향일 때 바깥쪽 면
                faces.append((v_top_curr, v_top_next, v_bot_next, v_bot_curr)) 
            else: # 경로 반전 시 옆면 와인딩도 반전시켜 법선 방향 유지
                faces.append((v_top_curr, v_bot_curr, v_bot_next, v_top_next))

        idx_inner_r = 0 # 안쪽 반지름 인덱스
        for j in range(res_theta - 1):
            v_top_curr = idx_inner_r * res_theta + j
            v_top_next = idx_inner_r * res_theta + j + 1
            v_bot_curr = base_idx + v_top_curr
            v_bot_next = base_idx + v_top_next
            if not reverse: # 기본 방향일 때 안쪽 면 (메쉬 안쪽을 향함)
                faces.append((v_top_curr, v_bot_curr, v_bot_next, v_top_next)) 
            else:
                faces.append((v_top_curr, v_top_next, v_bot_next, v_bot_curr))

        # 시작 캡 면 (theta=0 또는 reverse시 theta=angle_rad에 해당하는 면)
        start_theta_col_idx = 0 
        for i in range(res_r - 1): 
            v_top_curr_r = i * res_theta + start_theta_col_idx
            v_top_next_r = (i + 1) * res_theta + start_theta_col_idx
            v_bot_curr_r = base_idx + v_top_curr_r
            v_bot_next_r = base_idx + v_top_next_r
            if not reverse: # 법선이 나선 시작 바깥쪽을 향하도록
                faces.append((v_top_curr_r, v_top_next_r, v_bot_next_r, v_bot_curr_r))
            else:
                faces.append((v_top_curr_r, v_bot_curr_r, v_bot_next_r, v_top_next_r))
                
        # 끝 캡 면 (theta=angle_rad 또는 reverse시 theta=0에 해당하는 면)
        end_theta_col_idx = res_theta - 1 
        for i in range(res_r - 1):
            v_top_curr_r = i * res_theta + end_theta_col_idx
            v_top_next_r = (i + 1) * res_theta + end_theta_col_idx
            v_bot_curr_r = base_idx + v_top_curr_r
            v_bot_next_r = base_idx + v_top_next_r
            if not reverse: # 법선이 나선 끝 바깥쪽을 향하도록
                faces.append((v_top_curr_r, v_bot_curr_r, v_bot_next_r, v_top_next_r))
            else:
                faces.append((v_top_curr_r, v_top_next_r, v_bot_next_r, v_bot_curr_r))

    save_obj(vertices, faces, os.path.join(output_dir, f"{name}.obj"))


# ✅ Ramp Curve 도로 생성 함수
# name: 생성될 모델의 이름
# output_dir: 모델이 저장될 디렉토리 경로
# curve_dir: 곡선 방향 ("left" 또는 "right")
# radius_outer: 곡선 도로의 바깥쪽 반지름
# width: 도로의 폭
# height: 램프의 총 높이 변화량
# angle_rad: 곡선 도로의 총 각도 (라디안)
# res_r: 반지름 방향 해상도 (정점 수)
# res_theta: 각도 방향 해상도 (정점 수)
# edge_resolution: 두께 생성 시 옆면/밑면 샘플링 해상도 계수
# bump_strength: 표면 범프의 강도
# sigma: 범프 노이즈의 가우시안 필터 시그마 값
# thickness: 도로의 두께
def generate_ramp_curve(name, output_dir, curve_dir="right",
                        radius_outer=8.0, width=4.0, height=1.8,
                        angle_rad=np.pi/2, res_r=100, res_theta=100,
                        edge_resolution=25,
                        bump_strength=0.3, sigma=(2.5, 2.5), thickness=1.0):
    radius_inner = radius_outer - width
    edge_margin = width * 0.04
    safe_theta_margin_num = max(1e-9, angle_rad * 0.01)
    safe_theta_margin_den_end = max(1e-9, angle_rad - safe_theta_margin_num)

    r_coords = np.linspace(radius_inner, radius_outer, res_r)
    theta_coords = np.linspace(0, angle_rad, res_theta)
    rv, thetav = np.meshgrid(r_coords, theta_coords, indexing='ij')

    x = rv * np.cos(thetav)
    z_values = rv * np.sin(thetav) 
    if curve_dir == "left": # 왼쪽 커브 시 X 좌표 반전
        x = -x 

    # 높이(Elevation) 계산: sin 곡선을 이용해 부드러운 경사 생성
    t = np.zeros_like(thetav)
    if angle_rad > 1e-6 : t = thetav / angle_rad # 0~1 정규화
    elevation = (np.sin(np.pi * (t - 0.5)) * 0.5 + 0.5) * height
    
    falloff_r = np.clip((rv - radius_inner)/edge_margin, 0, 1) * np.clip((radius_outer - rv)/edge_margin, 0, 1)
    falloff_theta = np.ones_like(thetav)
    if angle_rad > 1e-6:
        falloff_theta = np.clip(thetav/safe_theta_margin_num, 0, 1) * np.clip((angle_rad - thetav)/safe_theta_margin_den_end, 0, 1)
    bump = generate_noise_bump((res_r, res_theta), sigma, bump_strength, falloff_r * falloff_theta)

    y_center_offset = thickness / 2
    y_values_final = y_center_offset + elevation + bump
    
    vertices_list = []
    for i in range(res_r):
        for j in range(res_theta):
            vertices_list.append((x[i,j], y_values_final[i,j], z_values[i,j]))

    top_faces = []
    for i in range(res_r - 1):
        for j in range(res_theta - 1):
            v00 = i * res_theta + j
            v01 = i * res_theta + j + 1
            v10 = (i + 1) * res_theta + j
            v11 = (i + 1) * res_theta + j + 1
            if curve_dir == "left": # 왼쪽 커브 시 윗면 와인딩 (사용자 수정 반영)
                top_faces.append((v00, v10, v11, v01)) # CCW (X 반전 고려)
            else: # 오른쪽 커브 시 윗면 와인딩
                top_faces.append((v00, v01, v11, v10)) # CCW

    # Ramp Curve용 edge_parts 정의
    part1_rc = [i * res_theta + 0 for i in range(res_r)]
    part2_rc = []
    if res_theta > 1 : part2_rc = [(res_r - 1) * res_theta + j for j in range(1, res_theta)]
    part3_rc = []
    if res_r > 1 and res_theta > 1 : part3_rc = [i * res_theta + (res_theta - 1) for i in range(res_r - 2, -1, -1)]
    part4_rc = []
    if res_r > 1 and res_theta > 2 : part4_rc = [0 * res_theta + j for j in range(res_theta - 2, 0, -1)]
    
    effective_edge_parts_ramp_curve = { "bottom": part1_rc, "right": part2_rc, "top": part3_rc, "left": part4_rc }
    
    final_vertices, final_faces = extrude_thickness_from_top(
        vertices_list, top_faces, effective_edge_parts_ramp_curve, thickness, edge_resolution,
        reverse_winding= (curve_dir == "left")
    )
    save_obj(final_vertices, final_faces, os.path.join(output_dir, f"{name}.obj"))


def main():
    output_dir = f"{os.path.dirname(os.path.abspath(__file__))}/export"

    generate_cant("CantMediumLeft", output_dir, curve_amount=-0.5, resolution=10, bump_strength=0)
    generate_cant("CantMediumRight", output_dir, curve_amount=0.5, resolution=10, bump_strength=0)
    generate_cant("CantSmallLeft", output_dir, curve_amount=-0.25, resolution=10, bump_strength=0)
    generate_cant("CantSmallRight", output_dir, curve_amount=0.25, resolution=10, bump_strength=0)
    generate_straight("Straight", output_dir, resolution=10, bump_strength=0)
    generate_s_curve("SCurveLeft", output_dir, flip=False, resolution=10, bump_strength=0)
    generate_s_curve("SCurveRight", output_dir, flip=True, resolution=10, bump_strength=0)
    generate_curve("CurveLargeLeft", output_dir, radius_outer=8.0, res_r=10, res_theta=10, bump_strength=0)
    generate_curve("CurveLeft", output_dir, radius_outer=6.0, res_r=10, res_theta=10, bump_strength=0)
    generate_curve("CurveMediumLeft", output_dir, radius_outer=4.0, res_r=10, res_theta=10, bump_strength=0)
    generate_spiral("Spiral", output_dir, reverse=False, res_r=50, res_theta=100, bump_strength=0)
    generate_ramp_curve("RampCurveLeft", output_dir, curve_dir="left", res_r=10, res_theta=10, bump_strength=0, radius_outer=6, width=3, height=4)
    generate_ramp_curve("RampCurveRight", output_dir, curve_dir="right", res_r=10, res_theta=10, bump_strength=0, radius_outer=6, width=3, height=4)

    generate_cant("BumpedCantMediumLeft", output_dir, curve_amount=-0.5)
    generate_cant("BumpedCantMediumRight", output_dir, curve_amount=0.5)
    generate_cant("BumpedCantSmallLeft", output_dir, curve_amount=-0.25)
    generate_cant("BumpedCantSmallRight", output_dir, curve_amount=0.25)
    generate_straight("BumpedStraight", output_dir)
    generate_s_curve("BumpedSCurveLeft", output_dir, flip=False)
    generate_s_curve("BumpedSCurveRight", output_dir, flip=True)
    generate_curve("BumpedCurveLargeLeft", output_dir, radius_outer=8.0)
    generate_curve("BumpedCurveLeft", output_dir, radius_outer=6.0)
    generate_curve("BumpedCurveMediumLeft", output_dir, radius_outer=4.0)
    generate_spiral("BumpedSpiral", output_dir, reverse=False)
    generate_ramp_curve("BumpedRampCurveLeft", output_dir, curve_dir="left", radius_outer=6, width=3, height=4)
    generate_ramp_curve("BumpedRampCurveRight", output_dir, curve_dir="right", radius_outer=6, width=3, height=4)

if __name__ == "__main__":
    main()
