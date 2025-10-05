shader_type canvas_item;

uniform float time_factor : hint_range(0.1, 10.0) = 3.0;
uniform vec4 light_color : source_color = vec4(1.0, 0.8, 0.2, 1.0);
uniform float pulse_speed : hint_range(0.1, 10.0) = 2.0;

void fragment() {
    // Нормализуем положение пикселя по длине линии (координата от 0 до 1)
    float t = UV.x;

    // Анимированный пульс
    float pulse = sin(TIME * pulse_speed + t * time_factor) * 0.5 + 0.5;

    // Гладкие края (затухание на концах)
    float fade = smoothstep(0.0, 0.2, t) * smoothstep(1.0, 0.8, t);

    // Объединяем эффекты
    float brightness = pulse * fade;

    // Цвет и яркость
    COLOR = light_color * brightness;
}