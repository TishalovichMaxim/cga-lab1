def draw_pixel(surface, pos, color):
    surface.fill(color, (pos, (1, 1)))

def draw_line(surface, from_point, to_point, color):
    x1, y1 = from_point
    x2, y2 = to_point

    dx = (x2 - x1)
    dy = (y2 - y1)

    if (abs(dx) >= abs(dy)):
        step = abs(dx)
    else:
        step = abs(dy)

    if step < 0.001:
        return

    dx = dx / step
    dy = dy / step

    x = x1
    y = y1
    i = 0
    while (i <= step):
        draw_pixel(surface, (x, y), color)
        x = x + dx
        y = y + dy
        i = i + 1

def bres_draw_line(surface, from_point, to_point, color):
    x1, y1 = from_point
    x2, y2 = to_point
    
