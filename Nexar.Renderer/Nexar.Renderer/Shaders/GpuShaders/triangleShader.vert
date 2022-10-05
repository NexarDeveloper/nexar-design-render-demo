#version 330 core

layout(location = 0) in vec3 aPosition;

layout(location = 1) in vec3 aColor;

in float color;

out VS_OUT {
    vec3 color;
} vs_out;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
    vs_out.color = vec3(aColor);     // set the output variable color
}