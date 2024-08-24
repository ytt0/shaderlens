namespace ProjectName;

class Common
{
    public vec3 getColor(float hue)
    {
        return vec3(sin(hue * 6.28), sin((hue + 0.33) * 6.28), sin((hue + 0.66) * 6.28)) * 0.3 + 0.7;
    }
}
