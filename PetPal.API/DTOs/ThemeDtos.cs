public class UpdateThemeDto
{
    public string Theme { get; set; }
    public string ColorAccent { get; set; }
    public string FontSize { get; set; }
    public bool UseSystemPreference { get; set; }
}
public class CreateThemeDto
{
    public string Theme { get; set; }
    public string ColorAccent { get; set; }
    public string FontSize { get; set; }
    public bool UseSystemPreference { get; set; } 
}