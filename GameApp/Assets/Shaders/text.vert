#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aColor;
layout (location = 2) in vec2 aTexCoord;
//layout (location = 3) in uint aBlockId;

//out vec3 vertexNormal;
out vec3 vColor;
out vec2 vTexCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
//uniform mat3 normalMatrix;

void main()
{
    gl_Position = projection * view * model * vec4(aPosition, 1.0);

//    vertexNormal = normalMatrix * aNormal;
    vTexCoord = aTexCoord;
    vColor = aColor;
}