shader_type canvas_item;
uniform sampler2D screen_texture : hint_screen_texture, filter_linear_mipmap;
uniform float blurring_scale = 25.0;
uniform float blurring_percent = 0;
uniform float zoom_offset_percentage = .2f;
//uniform float pincushion_scale = 1;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec2 center_vector = SCREEN_UV-vec2(.5,.5);
	float dis_to_mid = length(center_vector);
	float blur_level = pow(dis_to_mid,2) * blurring_scale * blurring_percent;

	//float pincushion_magnitude = sin(abs(abs(center_vector.x) - abs(center_vector.y)) * PI) * dis_to_mid * pincushion_scale;
	//vec2 pincushion_uv = SCREEN_UV - center_vector * pincushion_magnitude;

	vec2 zoomed_uv = SCREEN_UV - center_vector * zoom_offset_percentage * blurring_percent;



	COLOR.rgb = textureLod(screen_texture, zoomed_uv, blur_level).rgb;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
