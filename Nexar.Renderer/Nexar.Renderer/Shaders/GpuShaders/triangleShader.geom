#version 330 core

layout (triangles) in;

layout (triangle_strip, max_vertices = 10) out;

in VS_OUT {
    vec3 color;
} gs_in[];

out vec3 fColor;

void main()
{
	fColor = vec3(gs_in[0].color);
    int i;
    for(i = 0; i < gl_in.length(); i++)
    {
        gl_Position = gl_in[i].gl_Position;
        EmitVertex();
    }
	EndPrimitive();
}