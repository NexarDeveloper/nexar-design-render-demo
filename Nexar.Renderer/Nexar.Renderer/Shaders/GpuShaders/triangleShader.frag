#version 330 core

out vec4 FragColor;
  
in vec3 vColor; // the input variable from the vertex shader (same name and same type)  

void main()
{
    FragColor = vec4(vColor, 1.0f);
} 