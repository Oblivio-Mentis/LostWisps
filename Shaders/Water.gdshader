shader_type canvas_item;

uniform float time_speed = 2.0;
uniform float wave_height = 6.0;
uniform float wave_frequency = 0.03;
uniform vec4 water_color_top = vec4(0.2, 0.6, 0.9, 0.6);
uniform vec4 water_color_bottom = vec4(0.1, 0.4, 0.7, 0.8);

float hash(float n) {
    return fract(sin(n) * 1e4);
}

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    float a = hash(i.x + i.y * 1e4);
    float b = hash(i.x + 1.0 + i.y * 1e4);
    float c = hash(i.x + (i.y + 1.0) * 1e4);
    float d = hash(i.x + 1.0 + (i.y + 1.0) * 1e4);
    vec2 u = smoothstep(0.0, 1.0, f);
    return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

void vertex() {
    float wave1 = sin((VERTEX.x + TIME * time_speed) * wave_frequency) * wave_height;
    float wave2 = sin((VERTEX.x + TIME * time_speed * 0.7) * wave_frequency * 1.3) * wave_height * 0.5;
    VERTEX.y += wave1 + wave2;
}

void fragment() {
    float y_norm = UV.y; // от 0 до 1
    vec4 gradient = mix(water_color_top, water_color_bottom, y_norm);

    vec2 distorted_uv = UV + vec2(0.0, noise(UV * 5.0 + TIME * 0.2) * 0.02);
    COLOR = texture(TEXTURE, distorted_uv) * gradient;
}