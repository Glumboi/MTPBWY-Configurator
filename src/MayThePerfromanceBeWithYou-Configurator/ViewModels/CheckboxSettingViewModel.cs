namespace MayThePerfromanceBeWithYou_Configurator.ViewModels;

public class CheckboxSettingViewModel : CustomSettingViewModelBase
{
    private bool _checkboxValue;

    public bool CheckboxValue
    {
        get => bool.TryParse(Value, out _);
        set => Value = value.ToString();
    }
    
    
}