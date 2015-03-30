#version 130

// attributes
in vec2 v_position;
in vec2 v_texCoord;

// varyings
out vec2 fUV;

void main()
{
	gl_Position = vec4(v_position, 0, 1);
	fUV = v_texCoord;
}