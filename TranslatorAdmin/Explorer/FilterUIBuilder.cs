using System.Reflection;
using Translator.Desktop.Properties;

namespace Translator.Desktop.Explorer
{
    internal static class FilterUIBuilder
    {
        internal static bool ReadOnly => !Settings.Default.EnableStoryExplorerEdit;

        internal static void CheckBoxSetValue(object? sender, PropertyInfo property, object data)
        {
            if (!ReadOnly && sender != null)
                property.SetValue(data, Convert.ToBoolean(((CheckBox)sender).Checked));
        }

        internal static void DropDownNullableSetValue(object? sender, PropertyInfo property, object data)
        {
            if (!ReadOnly && sender != null)
                property.SetValue(data, Enum.Parse(property.PropertyType.GenericTypeArguments[0], ((ComboBox)sender).SelectedItem?.ToString() ?? string.Empty));
        }

        internal static void DropDownSetValue(object? sender, PropertyInfo property, object data)
        {
            if (!ReadOnly && sender != null)
                property.SetValue(data, Enum.Parse(property.PropertyType, ((ComboBox)sender).SelectedItem?.ToString()!));
        }

        internal static void NumericFloatSetValue(object? sender, PropertyInfo property, object data)
        {
            if (!ReadOnly && sender != null)
                property.SetValue(data, (float)Convert.ToDouble(((NumericUpDown)sender).Value));
        }

        internal static void NumericIntSetValue(object? sender, PropertyInfo property, object data)
        {
            if (!ReadOnly && sender != null)
                property.SetValue(data, Convert.ToInt32(((NumericUpDown)sender).Value));
        }

        internal static void TextBoxSetValue(object? sender, PropertyInfo property, object data)
        {
            if (!ReadOnly && sender != null)
                property.SetValue(data, ((TextBoxBase)sender).Text);
        }

        internal static void SetEditableStates(GroupBox box)
        {
            foreach (var control in box.Controls[0].Controls)
            {
                if (control.GetType() == typeof(ComboBox) || control.GetType() == typeof(CheckBox)) ((Control)control).Enabled = !ReadOnly;
                else if (control.GetType() == typeof(TextBox)) ((TextBox)control).ReadOnly = ReadOnly;
                else if (control.GetType() == typeof(NumericUpDown)) ((NumericUpDown)control).ReadOnly = ReadOnly;
            }
        }

        internal static void FillDisplayComponents(GroupBox box, object data)
        {
            foreach (var property in data.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                Type valueType = property.PropertyType;
                object? value = property.GetValue(data);

                if (valueType == typeof(string))
                {
                    var text = box.Controls[0].Controls.Find(property.Name + "TextBox", true);
                    if (text.Length == 1) text[0].Text = (string?)value ?? string.Empty;
                }
                else if (valueType == typeof(int) || valueType == typeof(float))
                {
                    var text = box.Controls[0].Controls.Find(property.Name + "Numeric", true);
                    if (text.Length == 1 && text[0].GetType().IsAssignableFrom(typeof(NumericUpDown))) ((NumericUpDown)text[0]).Value = Convert.ToDecimal(value);
                }
                else if (valueType == typeof(bool))
                {
                    var text = box.Controls[0].Controls.Find(property.Name + "CheckBox", true);
                    if (text.Length == 1 && text[0].GetType().IsAssignableFrom(typeof(CheckBox))) ((CheckBox)text[0]).Checked = Convert.ToBoolean(value);
                }
                else if (valueType.GenericTypeArguments.Length > 0)
                {
                    if (valueType.GenericTypeArguments[0].IsEnum)
                    {
                        var text = box.Controls[0].Controls.Find(property.Name + "ComboBox", true);
                        if (text.Length == 1 && text[0].GetType().IsAssignableFrom(typeof(ComboBox))) ((ComboBox)text[0]).SelectedItem = value?.ToString();
                    }
                }
                else if (valueType.IsEnum)
                {
                    var text = box.Controls[0].Controls.Find(property.Name + "ComboBox", true);
                    if (text.Length == 1 && text[0].GetType().IsAssignableFrom(typeof(ComboBox))) ((ComboBox)text[0]).SelectedItem = value?.ToString();
                }
            }
        }

    }
}