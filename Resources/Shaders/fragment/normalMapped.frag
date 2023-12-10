#version 460 core

uniform sampler2D u_DiffuseMap;
uniform sampler2D u_NormalMap;

in vec3 v_Normal;
in vec2 v_TexCoord;

out vec4 FragColor;

void main()
{
    vec3 normal = normalize(texture(u_NormalMap, v_TexCoord).rgb * 2.0 - 1.0);
    vec3 lightDirection = normalize(vec3(1.0, 1.0, 1.0));
    float diffuseIntensity = max(dot(normal, lightDirection), 0.0);
    FragColor = texture(u_DiffuseMap, v_TexCoord) * diffuseIntensity;
}
