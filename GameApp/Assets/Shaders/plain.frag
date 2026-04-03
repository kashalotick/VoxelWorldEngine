#version 330 core

in vec4 vColor;
in vec2 vTexCoord;

out vec4 FragColor;

uniform sampler2D fontAtlas;

void main() {
    vec4 texColor = texture(fontAtlas, vTexCoord);
    FragColor = vColor * texColor;
}