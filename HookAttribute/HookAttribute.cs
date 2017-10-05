using System;


public class HookAttribute : Attribute
{
    private string _fullName;
    public HookAttribute(string fullName, bool end = false) {
        _fullName = fullName;
    }
}