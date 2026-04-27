#version 330 core

in vec4 vColor;
in vec2 vTexCoord;
in vec3 vPos;

out vec4 FragColor;

uniform sampler2D uTexture;

void main() {
    vec4 texColor = texture(uTexture, vTexCoord);

    FragColor = vColor * texColor;

    //    vec4 uvDebug = vec4(vTexCoord, 0.0, 1.0);
    //    FragColor = mix(uvDebug, vColor * texColor, texColor.a);

}