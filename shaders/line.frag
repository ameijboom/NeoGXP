#version 330 core
// NOTICE: Shaders use the American spelling of color.

//OUTPUTS
out vec4 FragColor;

//INPUTS
in vec4 vertexColor; //from the line vertex shader

void main()
{
    FragColor = vertexColor;
}
