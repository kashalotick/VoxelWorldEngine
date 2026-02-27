#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;
layout (location = 3) in uint aBlockId;

out vec3 vertexNormal;
out vec2 texCoord;
out vec3 fragPos;
flat out uint blockId;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat3 normalMatrix;

void main()
{
    gl_Position = projection * view * model * vec4(aPosition, 1.0);

    vertexNormal = normalMatrix * aNormal;
    texCoord = aTexCoord;
    blockId = aBlockId;

    fragPos = vec3(model * vec4(aPosition, 1.0));
}