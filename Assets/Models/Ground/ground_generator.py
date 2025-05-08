# Modular Road Tile Generator (Optimized Thickness + Bump)
import numpy as np
from scipy.ndimage import gaussian_filter
import os

# ✅ 범프 노이즈 생성 함수
def generate_noise_bump(shape, sigma, bump_strength, falloff):
    np.random.seed(42)
    noise = np.random.rand(*shape)
    smooth_noise = gaussian_filter(noise, sigma=sigma)
    smooth_noise = (smooth_noise - 0.5) * 2
    return smooth_noise * bump_strength * falloff

# ✅ .obj 파일 저장 함수 (중복 방지)
def save_obj(vertices, faces, path, do_optimize=False):
    if os.path.exists(path):
        raise FileExistsError(f"❌ File already exists: {path}")
    os.makedirs(os.path.dirname(path), exist_ok=True)

    if do_optimize:
        import trimesh
        # 1. 저장 (소수점 3자리)
        obj_lines = [f"v {vx:.3f} {vy:.3f} {vz:.3f}" for vx, vy, vz in vertices]
        obj_lines += [f"f {' '.join(str(i+1) for i in face)}" for face in faces]

        temp_path = path.replace(".obj", "_temp.obj")
        with open(temp_path, "w") as f:
            f.write("\n".join(obj_lines))

        # 2. 중복 정점 제거 후 다시 저장
        try:
            mesh = trimesh.load(temp_path, force='mesh')
            mesh.merge_vertices()
            mesh.export(path)
            os.remove(temp_path)
            print(f"✅ Optimized and saved: {path}")
        except Exception as e:
            print(f"⚠️ Failed to optimize {path}: {e}")
            os.rename(temp_path, path)  # fallback 저장
    else:
        lines = [f"v {vx:.6f} {vy:.6f} {vz:.6f}" for vx, vy, vz in vertices]
        lines += [f"f {' '.join(str(i+1) for i in face)}" for face in faces]
        with open(path, "w") as f:
            f.write("\n".join(lines))
        print(f"✅ Saved: {path}")


# ✅ 외곽 루프 기반 두께 생성 함수 (옆면 + 바닥 최소 polygon)
def extrude_thickness_from_top(vertices, faces, edge_loop_indices, thickness):
    if thickness <= 0:
        return vertices, faces

    base_idx = len(vertices)
    bottom_vertices = [(vx, vy - thickness, vz) for (vx, vy, vz) in vertices]
    vertices += bottom_vertices

    for i in range(len(edge_loop_indices)):
        a = edge_loop_indices[i]
        b = edge_loop_indices[(i + 1) % len(edge_loop_indices)]
        a_bot = a + base_idx
        b_bot = b + base_idx
        faces.append((a, b, b_bot, a_bot))

    bottom_loop = [i + base_idx for i in reversed(edge_loop_indices)]
    if len(bottom_loop) >= 4:
        faces.append(tuple(bottom_loop))

    return vertices, faces

# ✅ 경계 루프 인덱스 생성 함수 (정사각형 타일)
def get_square_edge_loop(resolution):
    edge = []
    for i in range(resolution): edge.append(i)  # bottom
    for i in range(1, resolution): edge.append(i*resolution + resolution - 1)  # right
    for i in range(resolution-2, -1, -1): edge.append(resolution*(resolution-1) + i)  # top
    for i in range(resolution-2, 0, -1): edge.append(i*resolution)  # left
    return edge

# ✅ spiral용 edge loop 생성 함수 (θ 방향 기준)
def get_spiral_edge_loop(res_r, res_theta):
    # res_r: 반지름 방향 해상도
    # res_theta: 각도 방향 해상도

    edge_loop = []
    # 바깥쪽 (가장 바깥 반지름)의 각도 방향 vertex index
    for j in range(res_theta):
        i = res_r - 1
        edge_loop.append(i * res_theta + j)
    for j in range(res_theta - 1, -1, -1):
        i = 0
        edge_loop.append(i * res_theta + j)
    return edge_loop

# ✅ Cant 도로 생성 함수
def generate_cant(name, output_dir, curve_amount, tile_size=4.0, resolution=400, bump_strength=0.15, sigma=3.0, thickness=1.0):
    x = np.linspace(-tile_size/2, tile_size/2, resolution)
    y = np.linspace(-tile_size/2, tile_size/2, resolution)
    xv, yv = np.meshgrid(x, y)
    xv, yv = xv.flatten(), yv.flatten()
    top_y = thickness / 2

    edge_margin = tile_size * 0.01
    falloff_x = np.clip((tile_size/2 - np.abs(xv)) / edge_margin, 0, 1)
    falloff_y = np.clip((tile_size/2 - np.abs(yv)) / edge_margin, 0, 1)
    falloff = (falloff_x * falloff_y).reshape((resolution, resolution))

    x_offset = np.sin(yv / tile_size * np.pi) * curve_amount
    xv_curved = xv + x_offset

    bump = generate_noise_bump((resolution, resolution), sigma, bump_strength, falloff).flatten()
    vertices = [(xv_curved[i], top_y + bump[i], yv[i]) for i in range(len(xv))]
    faces = [(i, i+1, i+1+resolution, i+resolution) for i in range((resolution-1)*resolution) if (i+1)%resolution != 0]

    edge = get_square_edge_loop(resolution)
    vertices, faces = extrude_thickness_from_top(vertices, faces, edge, thickness)
    save_obj(vertices, faces, os.path.join(output_dir, f"{name}.obj"))

# ✅ S-Curve 도로 생성 함수
def generate_s_curve(name, output_dir, flip=False, tile_length=8.0, tile_width=4.0, resolution=400, amplitude=2.0, bump_strength=0.12, sigma=3.0, thickness=1.0):
    x = np.linspace(-tile_width/2, tile_width/2, resolution)
    y = np.linspace(-tile_length/2, tile_length/2, resolution)
    xv, yv = np.meshgrid(x, y)
    xv, yv = xv.flatten(), yv.flatten()
    top_y = thickness / 2

    direction = -1 if flip else 1
    edge_margin = tile_length * 0.01
    falloff_x = np.clip((tile_width/2 - np.abs(xv)) / edge_margin, 0, 1)
    falloff_y = np.clip((tile_length/2 - np.abs(yv)) / edge_margin, 0, 1)
    falloff = (falloff_x * falloff_y).reshape((resolution, resolution))

    x_offset = direction * amplitude * np.sin(np.pi * yv / tile_length)
    xv_s = xv + x_offset
    bump = generate_noise_bump((resolution, resolution), sigma, bump_strength, falloff).flatten()

    vertices = [(xv_s[i], top_y + bump[i], yv[i]) for i in range(len(xv))]
    faces = [(i, i+1, i+1+resolution, i+resolution) for i in range((resolution-1)*resolution) if (i+1)%resolution != 0]
    edge = get_square_edge_loop(resolution)
    vertices, faces = extrude_thickness_from_top(vertices, faces, edge, thickness)
    save_obj(vertices, faces, os.path.join(output_dir, f"{name}.obj"))

# ✅ Straight 도로 생성 함수
def generate_straight(name, output_dir, tile_length=4.0, resolution=400, bump_strength=0.12, sigma=3.0, thickness=1.0):
    x = np.linspace(-tile_length/2, tile_length/2, resolution)
    y = np.linspace(-tile_length/2, tile_length/2, resolution)
    xv, yv = np.meshgrid(x, y)
    xv, yv = xv.flatten(), yv.flatten()
    top_y = thickness / 2

    edge_margin = tile_length * 0.01
    falloff_x = np.clip((tile_length/2 - np.abs(xv)) / edge_margin, 0, 1)
    falloff_y = np.clip((tile_length/2 - np.abs(yv)) / edge_margin, 0, 1)
    falloff = (falloff_x * falloff_y).reshape((resolution, resolution))

    bump = generate_noise_bump((resolution, resolution), sigma, bump_strength, falloff).flatten()
    vertices = [(xv[i], top_y + bump[i], yv[i]) for i in range(len(xv))]
    faces = [(i, i+1, i+1+resolution, i+resolution) for i in range((resolution-1)*resolution) if (i+1)%resolution != 0]
    edge = get_square_edge_loop(resolution)
    vertices, faces = extrude_thickness_from_top(vertices, faces, edge, thickness)
    save_obj(vertices, faces, os.path.join(output_dir, f"{name}.obj"))

def generate_curve(name, output_dir, radius_outer, angle_rad=np.radians(90),
                   width=4.0, res_r=400, res_theta=400,
                   bump_strength=0.10, sigma=(2.0, 2.0), thickness=1.0):
    radius_inner = radius_outer - width
    edge_margin = width * 0.01
    theta_margin = angle_rad * 0.01

    r = np.linspace(radius_inner, radius_outer, res_r)
    theta = np.linspace(0, angle_rad, res_theta)
    rv, thetav = np.meshgrid(r, theta, indexing='ij')

    x = rv * np.cos(thetav)
    z = rv * np.sin(thetav)
    top_y = thickness / 2

    falloff_r = np.clip((rv - radius_inner)/edge_margin, 0, 1) * np.clip((radius_outer - rv)/edge_margin, 0, 1)
    falloff_theta = np.clip(thetav/theta_margin, 0, 1) * np.clip((angle_rad - thetav)/theta_margin, 0, 1)
    falloff = falloff_r * falloff_theta
    y = generate_noise_bump((res_r, res_theta), sigma, bump_strength, falloff)

    vertices = [(x[i,j], top_y + y[i,j], z[i,j]) for i in range(res_r) for j in range(res_theta)]
    faces = [(i*res_theta + j, i*res_theta + j + 1, (i+1)*res_theta + j + 1, (i+1)*res_theta + j)
             for i in range(res_r - 1) for j in range(res_theta - 1)]

    edge = get_square_edge_loop(res_theta)
    vertices, faces = extrude_thickness_from_top(vertices, faces, edge, thickness)
    save_obj(vertices, faces, os.path.join(output_dir, f"{name}.obj"))

def generate_spiral(name, output_dir, reverse=False,
                    radius_outer=8.0, width=4.0, height=8.0,
                    angle_rad=np.radians(360),
                    res_r=400, res_theta=800,
                    bump_strength=0.12, sigma=(3.0, 1.0), thickness=1.0):
    radius_inner = radius_outer - width
    edge_margin = width * 0.01
    theta_margin = angle_rad * 0.01

    theta = np.linspace(0, angle_rad, res_theta)
    if reverse:
        theta = theta[::-1]
    r = np.linspace(radius_inner, radius_outer, res_r)
    rv, thetav = np.meshgrid(r, theta, indexing='ij')

    x = rv * np.cos(thetav)
    z = rv * np.sin(thetav)
    base_y = (thetav / angle_rad) * height

    falloff_r = np.clip((rv - radius_inner)/edge_margin, 0, 1) * np.clip((radius_outer - rv)/edge_margin, 0, 1)
    falloff_theta = np.clip(thetav/theta_margin, 0, 1) * np.clip((angle_rad - thetav)/theta_margin, 0, 1)
    bump = generate_noise_bump((res_r, res_theta), sigma, bump_strength, falloff_r * falloff_theta)

    top_y = thickness / 2
    y = base_y + bump
    vertices = [(x[i,j], top_y + y[i,j], z[i,j]) for i in range(res_r) for j in range(res_theta)]
    faces = [(i*res_theta + j, i*res_theta + j + 1, (i+1)*res_theta + j + 1, (i+1)*res_theta + j)
             for i in range(res_r - 1) for j in range(res_theta - 1)]

    edge = get_spiral_edge_loop(res_r, res_theta)
    vertices, faces = extrude_thickness_from_top(vertices, faces, edge, thickness)
    save_obj(vertices, faces, os.path.join(output_dir, f"{name}.obj"))

def generate_ramp_curve(name, output_dir, curve_dir="right",
                        radius_outer=8.0, width=4.0, height=1.8,
                        angle_rad=np.pi/2,
                        res_r=400, res_theta=400,
                        bump_strength=0.12, sigma=(2.5, 2.5), thickness=1.0):
    radius_inner = radius_outer - width
    edge_margin = width * 0.01
    theta_margin = angle_rad * 0.01

    r = np.linspace(radius_inner, radius_outer, res_r)
    theta = np.linspace(0, angle_rad, res_theta)
    rv, thetav = np.meshgrid(r, theta, indexing='ij')

    x = rv * np.cos(thetav)
    z = rv * np.sin(thetav)
    if curve_dir == "left":
        x = -x

    t = thetav / angle_rad
    elevation = np.sin(np.pi * (t - 0.5)) * 0.5 + 0.5
    elevation *= height

    falloff_r = np.clip((rv - radius_inner)/edge_margin, 0, 1) * np.clip((radius_outer - rv)/edge_margin, 0, 1)
    falloff_theta = np.clip(thetav/theta_margin, 0, 1) * np.clip((angle_rad - thetav)/theta_margin, 0, 1)
    total_falloff = falloff_r * falloff_theta
    bump = generate_noise_bump((res_r, res_theta), sigma, bump_strength, total_falloff)

    y = elevation + bump
    top_y = thickness / 2
    vertices = [(x[i,j], top_y + y[i,j], z[i,j]) for i in range(res_r) for j in range(res_theta)]
    faces = [(i*res_theta + j, i*res_theta + j + 1, (i+1)*res_theta + j + 1, (i+1)*res_theta + j)
             for i in range(res_r - 1) for j in range(res_theta - 1)]

    edge = get_square_edge_loop(res_theta)
    vertices, faces = extrude_thickness_from_top(vertices, faces, edge, thickness)
    save_obj(vertices, faces, os.path.join(output_dir, f"{name}.obj"))

def main():
    output_dir = "./export"

    generate_cant("CantMediumLeft", output_dir, curve_amount=-0.5, bump_strength=0)
    generate_cant("CantMediumRight", output_dir, curve_amount=0.5, bump_strength=0)
    generate_cant("CantSmallLeft", output_dir, curve_amount=-0.25, bump_strength=0)
    generate_cant("CantSmallRight", output_dir, curve_amount=0.25, bump_strength=0)
    generate_straight("Straight", output_dir, bump_strength=0)
    generate_s_curve("SCurveLeft", output_dir, flip=False, bump_strength=0)
    generate_s_curve("SCurveRight", output_dir, flip=True, bump_strength=0)
    generate_curve("CurveLargeLeft", output_dir, radius_outer=8.0, bump_strength=0)
    generate_curve("CurveLeft", output_dir, radius_outer=6.0, bump_strength=0)
    generate_curve("CurveMediumLeft", output_dir, radius_outer=4.0, bump_strength=0)
    generate_spiral("SpiralLeft", output_dir, reverse=False, bump_strength=0)
    generate_spiral("SpiralRight", output_dir, reverse=True, bump_strength=0)
    generate_ramp_curve("RampCurveLeft", output_dir, curve_dir="left", bump_strength=0)
    generate_ramp_curve("RampCurveRight", output_dir, curve_dir="right", bump_strength=0)

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
    generate_spiral("BumpedSpiralLeft", output_dir, reverse=False)
    generate_spiral("BumpedSpiralRight", output_dir, reverse=True)
    generate_ramp_curve("BumpedRampCurveLeft", output_dir, curve_dir="left")
    generate_ramp_curve("BumpedRampCurveRight", output_dir, curve_dir="right")

if __name__ == "__main__":
    main()
