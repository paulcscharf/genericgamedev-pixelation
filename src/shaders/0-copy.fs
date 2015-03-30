#version 130

// uniforms
uniform sampler2D diffuseTexture;
uniform vec2 mousePosition;
uniform vec2 screenSize;

// varyings
in vec2 fUV;

// output
out vec4 fragColor;

void main()
{
	fragColor = texture(diffuseTexture, fUV);
}