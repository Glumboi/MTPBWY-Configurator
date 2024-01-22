namespace MayThePerfromanceBeWithYou_Configurator.ViewModels;

public class CustomSettingViewModelBase : ViewModelBase
{
    private string _name;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _value;

    public string Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }
}