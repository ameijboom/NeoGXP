#version 330 core
// NOTICE: Shaders use the American spelling of color.

//OUTPUTS
out vec4 FragColor;

//INPUTS
uniform sampler2D ourTexture;
in vec2 TexCoord; //from the vertex shader
uniform vec4 tint; //from GXP

void main()
{
    FragColor = texture(ourTexture, TexCoord) * tint;
}
