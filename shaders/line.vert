#version 330 core
// NOTICE: Shaders use the American spelling of color.

//OUTPUTS
out vec4 vertexColor; //to the line fragment shader

//INPUTS
layout (location = 0) in vec2 aPos;
uniform vec4 color; //from GXP

void main()
{
    gl_Position = vec4(aPos.x, aPos.y, 0.0, 1.0);
    vertexColor = color;
}
