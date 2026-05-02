#version 330 core

in vec3 vertexNormal;
in vec2 texCoord;
in vec3 fragPos;
flat in uint blockId;

out vec4 FragColor;

uniform sampler2DArray uTextureArray;

uniform vec3 lightColor;
uniform vec3 lightDirection;
uniform vec4 ambientColor;
uniform vec3 viewPos;
uniform float shininess;

uniform vec3 fogColor = vec3(0.5, 0.6, 0.7);
uniform float fogDensity = 0.003;

vec3 applyFog(vec3 lightingResult) {
    float dist = length(viewPos - fragPos);
    float fogFactor = exp(-pow(dist * fogDensity, 2.0));
    fogFactor = clamp(fogFactor, 0.0, 1.0);

    return mix(fogColor, lightingResult, fogFactor);
}

void main()
{
    vec3 N = normalize(vertexNormal);
    vec3 L = normalize(-lightDirection);
    vec3 V = normalize(viewPos - fragPos);
    vec3 R = reflect(-L, N);

    float diff = max(dot(N, L), 0.0);

    vec4 texColor = texture(uTextureArray, vec3(texCoord, blockId));

    if (texColor.a < 0.1)
    discard;

    float spec = pow(max(dot(V, R), 0.0), shininess);

    vec3 ambient = texColor.rgb * vec3(ambientColor.xyz) * ambientColor.w;
    vec3 diffuse = texColor.rgb * lightColor * diff;
    vec3 specular = lightColor * spec * 0;

    vec3 lightingResult = ambient + diffuse + specular;

    vec3 withFog = applyFog(lightingResult);

    FragColor = vec4(withFog, texColor.a);
}