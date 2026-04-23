#version 330 core

uniform vec3 uSkyTop;
uniform vec3 uSkyBottom;

in vec3 vRayDir;

out vec4 fragColor;

void main() {
    float t = clamp(normalize(vRayDir).y * 0.5 + 0.5, 0.0, 1.0);
    fragColor = vec4(mix(uSkyBottom, uSkyTop, t), 1.0);
}