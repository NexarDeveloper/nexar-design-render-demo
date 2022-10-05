#version 330 core

out vec4 FragColor;

in vec4 vertexColor; // the input variable from the vertex shader (same name and same type)  

void main()
{
    ivec2 coord = ivec2(gl_FragCoord.xy - 0.5);

    if (mod(coord.x + coord.y, 12) < 6)
       discard;

    FragColor = vertexColor;
}