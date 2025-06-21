shader_type canvas_item;

uniform float blur_amount = 2.77;
uniform float mix_amount = 0.41;
uniform vec4 color_over : source_color;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

void fragment() {
   vec4 blurred = textureLod(SCREEN_TEXTURE, SCREEN_UV, blur_amount);
   blurred.a = 255.0;
   vec4 fin = mix(blurred, color_over, mix_amount);
   COLOR = fin;
}
