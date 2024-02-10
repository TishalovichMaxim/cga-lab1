import math
import glm
from math import sin, cos, tan
import numpy as np

def get_translate(x, y, z):
    return glm.mat4(
        np.array([
            [1, 0, 0, x],
            [0, 1, 0, y],
            [0, 0, 1, z],
            [0, 0, 0, 1],
        ])
    )

def get_rotate_x(degrees_angle):
    angle = math.radians(degrees_angle)
    return glm.mat4(
        [
            [1, 0,          0,           0],
            [0, cos(angle), -sin(angle), 0],
            [0, sin(angle), cos(angle),  0],
            [0, 0,          0,           1]
        ]
    )

def get_rotate_y(degrees_angle):
    angle = math.radians(degrees_angle)
    return glm.mat4(
        [
            [cos(angle), 0, -sin(angle), 0],
            [0,          1, 0,           0],
            [sin(angle), 0, cos(angle),  0],
            [0,          0, 0,           1]
        ]
    )

def get_rotate_z(degrees_angle):
    angle = math.radians(degrees_angle)
    return glm.mat4(
        [
            [cos(angle), -sin(angle), 0, 0],
            [sin(angle), cos(angle),  0, 0],
            [0,          0,           1, 0],
            [0,          0,           0, 1]
        ]
    )

def get_view_mat(up, eye, target):

    ZAxis = glm.normalize(eye - target)
    XAxis = glm.normalize(glm.cross(up, ZAxis))
    YAxis = up

    return glm.mat4(
                np.array([
                    [XAxis.x, XAxis.y, XAxis.z, -(glm.dot(XAxis, eye))],
                    [YAxis.x, YAxis.y, YAxis.z, -(glm.dot(YAxis, eye))],
                    [ZAxis.x, ZAxis.y, ZAxis.z, -(glm.dot(ZAxis, eye))],
                    [0, 0, 0, 1]
                ])
            )

def get_proj_mat(z_near, z_far, aspect, fov):
    fov = (fov*180)/3.14

    return glm.mat4(
                np.array([
                    [1/(aspect*tan(fov/2)),  0,                0,                        0],
                    [0,                    1/tan(fov/2),       0,                          0],
                    [0,                    0,                  (z_far)/(z_near - z_far),   (z_near*z_far)/(z_near - z_far)],
                    [0,                    0,                   -1,                         0]
                ])
            )

def get_viewport_mat(width, height, x_min, y_min):
 
    return glm.mat4(
                np.array([
                    [width/2, 0, 0, x_min + width/2],
                    [0, -height/2,  0, y_min + height/2],
                    [0, 0, 1, 0],
                    [0, 0, 0, 1]
                ])
            )

