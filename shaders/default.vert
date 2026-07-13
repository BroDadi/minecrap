#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec4 aColor;

out vec2 texCoord;
out vec4 color;
out float visibility;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
const float density = 0.015;
const float gradient = 5;

void main()
{
    vec4 posRelToCam = vec4(aPosition, 1.0) * model * view;
    gl_Position = posRelToCam * projection;
    texCoord = aTexCoord;
    color = aColor;

    float dist = length(posRelToCam.xyz);
    visibility = exp(-pow((dist * density), gradient));
}