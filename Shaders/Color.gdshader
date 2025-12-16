shader_type canvas_item;

uniform vec4 fill_color : source_color = vec4(1.0, 0.0, 0.0, 1.0);

void fragment() {
    COLOR = fill_color;
}