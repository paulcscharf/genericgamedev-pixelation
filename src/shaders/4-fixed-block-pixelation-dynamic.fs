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
	float v0 = 1 - fUV.x * fUV.y;
	float v1 = fUV.x;
	float v = mix(v0, v1, mousePosition.x / screenSize.x);
	return 0.1 * (v * v * v * screenSize.x) / screenSize.x;
}

vec2 pixelateUV(vec2 uv, float scale)
{
	float blockScale = 0.02;

	float s = 1 - clamp(scale * 10, 0, 1);

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