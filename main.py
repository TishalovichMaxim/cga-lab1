import pygame
import drawing
from obj_parser import ObjParser
import glm 
import matrices

GREEN_COLOR = (0, 255, 0)
WIDTH, HEIGHT = (1280, 720)
FPS = 60
MODEL_FILE_NAME = "./models/pochita.obj"

def draw(faces, vertices, res_matrix):
    screen.fill("black")

    for face in faces:
        i1 = face[0][0] - 1
        i2 = face[1][0] - 1
        i3 = face[2][0] - 1

        p1 = (res_matrix*vertices[i1])
        p2 = (res_matrix*vertices[i2])
        p3 = (res_matrix*vertices[i3])

        p1 /= p1.w
        p2 /= p2.w
        p3 /= p3.w

        p1 = p1.xy
        p2 = p2.xy
        p3 = p3.xy

        drawing.draw_line(screen, p1, p2, GREEN_COLOR)
        drawing.draw_line(screen, p2, p3, GREEN_COLOR)
        drawing.draw_line(screen, p3, p1, GREEN_COLOR)

pygame.init()
screen = pygame.display.set_mode((WIDTH, HEIGHT))
clock = pygame.time.Clock()
running = True

up = glm.vec3(0, 1, 0)
eye = glm.vec3(0, 0, 4)
target = glm.vec3(0, 0, 0)

view_mat = matrices.get_view_mat(up, eye, target)

proj_mat = matrices.get_proj_mat(
            z_near=0.1,
            z_far=200,
            aspect=WIDTH/HEIGHT,
            fov=50
        )

viewport_mat = matrices.get_viewport_mat(
            width=WIDTH,
            height=HEIGHT,
            x_min=0,
            y_min=0
        )

parse_res = ObjParser().parse(MODEL_FILE_NAME)

model_mat = matrices.get_translate(-11.0, -2, 0)
rotate_mat = matrices.get_rotate_y(1)
res_mat = viewport_mat*proj_mat*view_mat*model_mat

while running:
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False

        pressed_keys = pygame.key.get_pressed()
        if pressed_keys[pygame.K_q]:
            running = False

    res_mat = viewport_mat*proj_mat*view_mat*model_mat

    draw(parse_res["f"], parse_res["v"], res_mat)
    model_mat = rotate_mat * model_mat
    pygame.display.flip()
    clock.tick(FPS)

pygame.quit()
