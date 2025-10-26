shader_type canvas_item;

/* Палитра (мягкая) */
uniform vec4 col_core : source_color = vec4(1.05, 0.95, 0.80, 1.0);
uniform vec4 col_mid  : source_color = vec4(1.00, 0.60, 0.20, 1.0);
uniform vec4 col_low  : source_color = vec4(0.35, 0.12, 0.05, 1.0);

/* Интенсивности (сдержанные) */
uniform float emissive_gain : hint_range(0.0, 1.5) = 0.35;
uniform float fire_mix      : hint_range(0.0, 1.0) = 0.45;

/* Движение/мерцание */
uniform float noise_scale   : hint_range(0.5, 8.0) = 3.0;
uniform float flow_speed    : hint_range(0.0, 6.0) = 0.9;
uniform float flicker_amp   : hint_range(0.0, 0.5) = 0.12;

/* Мягкость и дезеринг */
uniform float edge_softness   : hint_range(0.0, 2.0) = 0.9;
uniform float dither_strength : hint_range(0.0, 0.02) = 0.006;

/* Шум */
float hash(vec2 p){ return fract(sin(dot(p, vec2(127.1,311.7))) * 43758.5453); }

float value_noise(vec2 p){
    vec2 i = floor(p), f = fract(p);
    float a = hash(i);
    float b = hash(i + vec2(1.0,0.0));
    float c = hash(i + vec2(0.0,1.0));
    float d = hash(i + vec2(1.0,1.0));
    vec2 u = f*f*(3.0-2.0*f);
    return mix(a,b,u.x) + (c-a)*u.y*(1.0-u.x) + (d-b)*u.x*u.y;
}

float fbm(vec2 p){
    float v = 0.0;
    float amp = 0.5;
    mat2 ROT = mat2(vec2(0.8, 0.6), vec2(-0.6, 0.8));
    for (int i = 0; i < 4; i++){
        v += amp * value_noise(p);
        p = ROT * p * 2.0 + vec2(31.0, 17.0);
        amp *= 0.5;
    }
    return v;
}

/* Дезеринг без размеров экрана: плавающее “зерно” по SCREEN_UV */
float blue_noise_uv(vec2 suv){
    // Псевдослучайное зерно из нормализованных координат экрана
    vec2 p = floor(suv * 1024.0); // фиксированная сетка 1024 — достаточно для стабильности
    return fract(sin(dot(p, vec2(12.9898,78.233))) * 43758.5453);
}

vec3 fire_palette(float t){
    vec3 c1 = mix(col_low.rgb, col_mid.rgb, smoothstep(0.0, 0.6, t));
    vec3 c2 = mix(col_mid.rgb, col_core.rgb, smoothstep(0.4, 1.0, t));
    return mix(c1, c2, t);
}

void fragment(){
    vec4 base = texture(TEXTURE, UV);
    float a = base.a;

    float y = clamp(UV.y, 0.0, 1.0);

    float t = TIME * flow_speed;
    vec2 p = vec2(UV.x, UV.y * 1.5 - t);
    float n = fbm(p * noise_scale);

    float flicker = 1.0 + (fbm(vec2(t*3.1, t*2.3)) - 0.5) * 2.0 * flicker_amp;

    float heat = clamp(y * 0.85 + n * 0.25, 0.0, 1.0) * flicker;

    float edge = smoothstep(0.0, edge_softness, a);

    vec3 fire = fire_palette(heat);

    vec3 color = mix(base.rgb, fire, fire_mix * edge);

    color += fire * emissive_gain * edge;

    float dn = blue_noise_uv(SCREEN_UV) - 0.5;
    color += dither_strength * dn;

    COLOR = vec4(color, a);
}
