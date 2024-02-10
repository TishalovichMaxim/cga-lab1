import glm

def v_handler(parts, parse_result):
    vertices = parse_result["v"]        

    w = 1
    if len(parts) == 5:
        w = parts[4]

    new_vertex = glm.vec4(float(parts[1]), float(parts[2]), float(parts[3]), w)
    new_vertex /= w

    vertices.append(new_vertex)

def vt_handler(parts, parse_result):
    textures = parse_result["vt"]
    v = w = 0

    u = float(parts[1])

    if len(parts) >= 3:
        v = float(parts[2])
        if len(parts) == 4:
            w = float(parts[3])

    textures.append((u, v, w))

def vn_handler(parts, parse_result):
    normals = parse_result["vn"]

    normals.append((float(parts[1]), float(parts[2]), float(parts[3])))

def f_handler(parts, parse_result):
    faces = parse_result["f"]

    face = []
    for i in range(1, 4):
        curr_parts = parts[i].split("/")
        v = int(curr_parts[0])
        vt = vn = None
        if len(curr_parts) == 2:
            vt = int(curr_parts[1])
        elif len(curr_parts) == 3:
            if vt:
                vt = int(curr_parts[1])
            vn = int(curr_parts[2])

        face.append(
            (v, vt, vn)
        )

    faces.append(tuple(face))
    
class ObjParser:
    def __init__(self) -> None:
        self.handlers = {}
        self.parse_result = {
                    "v": [],
                    "vt": [],
                    "vn": [],
                    "f": []
                }

        self.handlers["v"] = v_handler
        self.handlers["vt"] = vt_handler
        self.handlers["vn"] = vn_handler
        self.handlers["f"] = f_handler

    def parse(self, file_name):
        with open(file_name) as f:
            lines = f.readlines()

        for line in lines:
            parts = line.split()

            if len(parts) > 0 and parts[0] in self.handlers:
                self.handlers[parts[0]](parts, self.parse_result)

        return self.parse_result

