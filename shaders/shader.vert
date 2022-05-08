#version 330 core
// NOTICE: Shaders use the American spelling of color.

//OUTPUTS
out vec2 TexCoord; //to fragment shader
out vec3 texPos;

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTexCoord;

uniform mat4 transform; //from GXP

void main()
{
	gl_Position = transform * vec4(aPos, 1.0);
	TexCoord = aTexCoord;
	texPos = aPos;
}
