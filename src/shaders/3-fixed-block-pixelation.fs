#version 130

// uniforms
uniform sampler2D diffuseTexture;
uniform vec2 mousePosition;
uniform vec2 screenSize;

// varyings
in vec2 fUV;

// output
out vec4 fragColor;


float getScale()
{
	return 0.5 * (mousePosition.x + 1) / screenSize.x;
}


vec2 pixelateUV(vec2 uv, float scale)
{
	float blockScale = 0.02;

	float s = 1 - clamp(scale * 2, 0, 1);

	s *= s;
	s *= s;

	return mix(floor(uv / blockScale) * blockScale, uv, s);
}

void main()
{
	float scale = getScale();

	vec2 uv = pixelateUV(fUV - 0.5, scale);

	fragColor = texture(diffuseTexture, uv + 0.5);
}