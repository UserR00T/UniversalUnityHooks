using System;


public class HookAttribute : Attribute
{
    private string _fullName;
    public HookAttribute(string fullName, bool end = false) {
        _fullName = fullName;
    }
}
public class ChangeAccessModifierAttribute : Attribute
{
    private string _ToAccessModifier;
    private string _Field;
    public ChangeAccessModifierAttribute(string ToAccessModifier, string Field)
    {
        _ToAccessModifier = ToAccessModifier;
        _Field = Field;
    }
}