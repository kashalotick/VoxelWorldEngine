#version 330 core

uniform mat4 uInvViewProj;

out vec3 vRayDir;

void main() {
    vec2 pos = vec2(
    (gl_VertexID & 1) * 4.0 - 1.0,
    (gl_VertexID & 2) * 2.0 - 1.0
    );
    
    vec4 worldPos = uInvViewProj * vec4(pos, 1.0, 1.0);
    vRayDir = worldPos.xyz / worldPos.w;
    gl_Position = vec4(pos, 0.9999, 1.0);
}