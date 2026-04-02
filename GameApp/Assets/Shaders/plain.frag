#version 330 core

in vec3 vColor;
in float vAlpha;
in vec2 vTexCoord;

out vec4 FragColor;

uniform sampler2D fontAtlas;

void main() {
    vec4 texColor = texture(fontAtlas, vTexCoord);
    FragColor = vec4(vColor, vAlpha) * texColor;
}