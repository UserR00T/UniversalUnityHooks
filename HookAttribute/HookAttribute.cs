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
        private string _toAccessModifier;
        private string _field;
        public ChangeAccessModifierAttribute(string toAccessModifier, string field)
        {
            _toAccessModifier = toAccessModifier;
            _field = field;
        }
    }
