namespace HousePartyTranslator
{
    internal class Setting
    {
        private readonly string _key;
        private object _value;
        internal Setting(string key, object value)
        {
            _key = key;
            _value = value;
        }

        internal void UpdateValue(object value)
        {
            _value = value;
        }

        internal object GetValue()
        {
            return _value;
        }

        public string GetKey()
        {
            return _key;
        }

        public override string ToString()
        {
            return _key + SettingsManager.CFG_STR_DELIM + _value.ToString();
        }
    }

    class BooleanSetting : Setting
    {
        public BooleanSetting(string key, bool value) : base(key, value)
        {

        }

        public void UpdateValue(bool value)
        {
            UpdateValue(value as object);
        }

        public new bool GetValue()
        {
            return (bool)base.GetValue();
        }
    }

    class StringSetting : Setting
    {
        public StringSetting(string key, string value) : base(key, value)
        {

        }

        public void UpdateValue(string value)
        {
            UpdateValue(value as object);
        }

        public new string GetValue()
        {
            return (string)base.GetValue();
        }
    }

    class IntegerSetting : Setting
    {
        public IntegerSetting(string key, int value) : base(key, value)
        {

        }

        public void UpdateValue(int value)
        {
            UpdateValue(value as object);
        }

        public new int GetValue()
        {
            return (int)base.GetValue();
        }
    }

    class FloatSetting : Setting
    {
        public FloatSetting(string key, float value) : base(key, value)
        {

        }

        public void UpdateValue(float value)
        {
            UpdateValue(value as object);
        }

        public new float GetValue()
        {
            return (float)base.GetValue();
        }
    }

    class CustomSetting : Setting
    {
        public CustomSetting(string key, object value) : base(key, value)
        {

        }

        public void UpdateValue<T>(T value)
        {
            UpdateValue(value as object);
        }

        public T GetValue<T>()
        {
            return (T)GetValue();
        }

        public string ToString<T>()
        {
            return GetKey() + SettingsManager.CFG_STR_DELIM + GetValue<T>().ToString() + SettingsManager.CFG_STR_DELIM + typeof(T).ToString();
        }
    }
}
