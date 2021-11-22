namespace HousePartyTranslator
{
    /// <summary>
    /// A internal base class of a setting with a key value pair and a default ToString override,
    /// as well as the appropriate getter/setter methods.
    /// </summary>
    internal class Setting
    {
        private readonly string _key;
        private object _value;

        /// <summary>
        /// The main constructor for a Setting object with object value.
        /// </summary>
        /// <param name="key"> The key of the setting as a string.</param>
        /// <param name="value">The value of the setting, should have a serializing ToString method implemented.</param>
        internal Setting(string key, object value)
        {
            _key = key;
            _value = value;
        }

        /// <summary>
        /// Updates the value of the Setting object to the new value
        /// </summary>
        /// <param name="value"> The new value.</param>
        internal void UpdateValue(object value)
        {
            _value = value;
        }

        /// <summary>
        /// Returns the value stored in the instance of the Setting object
        /// </summary>
        /// <returns>The stored value</returns>
        internal object GetValue()
        {
            return _value;
        }

        /// <summary>
        /// Gets the key of the Setting as a string.
        /// </summary>
        /// <returns>The key of this Setting</returns>
        public string GetKey()
        {
            return _key;
        }

        /// <summary>
        /// A default ToString override which adds a = between key and value.
        /// </summary>
        /// <returns>The key plus a = followed by the string representation of the value.</returns>
        public override string ToString()
        {
            return _key + SettingsManager.CFG_STR_DELIM + _value.ToString();
        }
    }

    /// <summary>
    /// An extension of the Settings class to be used for boolean values.
    /// </summary>
    class BooleanSetting : Setting
    {
        /// <summary>
        /// The constructor of the extended class.
        /// </summary>
        /// <param name="key">The key of this setting as a string</param>
        /// <param name="value">The value for this setting, as a bool</param>
        public BooleanSetting(string key, bool value) : base(key, value)
        {

        }

        /// <summary>
        /// Updates the value of the Setting object to the new value
        /// </summary>
        /// <param name="value"> The new value.</param>
        public void UpdateValue(bool value)
        {
            UpdateValue(value as object);
        }

        /// <summary>
        /// Returns the value stored in the instance of the Setting object
        /// </summary>
        /// <returns>The stored value</returns>
        public new bool GetValue()
        {
            return (bool)base.GetValue();
        }
    }

    /// <summary>
    /// An extension of the Settings class to be used for string values.
    /// </summary>
    class StringSetting : Setting
    {
        /// <summary>
        /// The constructor of the extended class.
        /// </summary>
        /// <param name="key">The key of this setting as a string</param>
        /// <param name="value">The value for this setting, as a string</param>
        public StringSetting(string key, string value) : base(key, value)
        {

        }

        /// <summary>
        /// Updates the value of the Setting object to the new value
        /// </summary>
        /// <param name="value"> The new value.</param>
        public void UpdateValue(string value)
        {
            UpdateValue(value as object);
        }

        /// <summary>
        /// Returns the value stored in the instance of the Setting object
        /// </summary>
        /// <returns>The stored value</returns>
        public new string GetValue()
        {
            return (string)base.GetValue();
        }
    }

    /// <summary>
    /// An extension of the Settings class to be used for float values.
    /// </summary>
    class FloatSetting : Setting
    {
        /// <summary>
        /// The constructor of the extended class.
        /// </summary>
        /// <param name="key">The key of this setting as a string</param>
        /// <param name="value">The value for this setting, as a float</param>
        public FloatSetting(string key, float value) : base(key, value)
        {

        }

        /// <summary>
        /// Updates the value of the Setting object to the new value
        /// </summary>
        /// <param name="value"> The new value.</param>
        public void UpdateValue(float value)
        {
            UpdateValue(value as object);
        }

        /// <summary>
        /// Returns the value stored in the instance of the Setting object
        /// </summary>
        /// <returns>The stored value</returns>
        public new float GetValue()
        {
            return (float)base.GetValue();
        }

        public override string ToString()
        {
            return GetKey() + SettingsManager.CFG_STR_DELIM + ((float)base.GetValue()).ToString("0.00",System.Globalization.NumberFormatInfo.InvariantInfo);
        }
    }

    /// <summary>
    /// An extension of the Settings class to be used for values of type T.
    /// </summary>
    class CustomSetting : Setting
    {
        /// <summary>
        /// The constructor of the extended class.
        /// </summary>
        /// <param name="key">The key of this setting as a string</param>
        /// <param name="value">The value for this setting, as a an object of custom type</param>
        public CustomSetting(string key, object value) : base(key, value)
        {

        }

        /// <summary>
        /// Updates the value of the Setting object to the new value of type T
        /// </summary>
        /// <param name="value"> The new value in type T.</param>
        public void UpdateValue<T>(T value)
        {
            UpdateValue(value as object);
        }

        /// <summary>
        /// Returns the value stored in the instance of the Setting object in type T
        /// </summary>
        /// <returns>The stored value in type T</returns>
        public T GetValue<T>()
        {
            return (T)GetValue();
        }

        /// <summary>
        /// A ToString override which adds a = between key and value, as well as the string representation of the type used.
        /// </summary>
        /// <returns>The key followed by the string representation of the value and the string representation of type T, seperated by =.</returns>
        public string ToString<T>()
        {
            return GetKey() + SettingsManager.CFG_STR_DELIM + GetValue<T>().ToString() + SettingsManager.CFG_STR_DELIM + typeof(T).ToString();
        }
    }
}
