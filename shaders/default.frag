#version 330 core

in vec2 texCoord;
in vec4 color;
in float visibility;
out vec4 FragColor;
uniform sampler2D texture0;
uniform vec3 skyColor;

void main()
{
    vec4 texColor = texture(texture0, texCoord);
    if (texColor.a == 0) discard;
    FragColor = texColor * color;
    FragColor = mix(vec4(skyColor, 1.0), FragColor, visibility);
}