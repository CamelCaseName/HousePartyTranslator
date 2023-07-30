using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Translator.Core.Helpers;
using Translator.Desktop.Properties;

namespace Translator.Desktop.Explorer
{
    internal static class ExtendedInfoUIBuilder
    {
        internal static bool ReadOnly => !Settings.Default.enableStoryExplorerEdit;

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

        internal static GroupBox GetDisplayComponentsForType(object data, Type dataType, Size maximumSize)
        {
            var box = new GroupBox()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                AutoSize = true,
                ForeColor = Utils.brightText,
                Name = dataType.Name + "DisplayBox",
                TabIndex = 10,
                TabStop = false,
                MaximumSize = maximumSize,
                Text = dataType.Name,
                Visible = false
            };
            box.SuspendLayout();
            var grid = new TableLayoutPanel()
            {
                Name = dataType.Name + "ValueTable",
                GrowStyle = TableLayoutPanelGrowStyle.AddRows,
                AutoSize = true,
                TabStop = false,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                ColumnCount = 2,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Dock = DockStyle.Fill,
                AutoScroll = true,
            };
            box.Controls.Add(grid);

            foreach (var property in dataType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                try
                {
                    Type valueType = property.PropertyType;
                    object? value = property.GetValue(data);
                    //string
                    if (valueType == typeof(string))
                    {
                        var label = new Label() { AutoSize = true, Text = property.Name, Name = property.Name + "Label", Dock = DockStyle.Fill, ForeColor = Utils.brightText };
                        grid.Controls.Add(label);
                        var text = new TextBox() { Text = (string?)value ?? string.Empty, Name = property.Name + "TextBox", Multiline = true, WordWrap = true, Dock = DockStyle.Fill, ReadOnly = ReadOnly, };
                        text.TextChanged += (object? sender, EventArgs e) => TextBoxSetValue(sender, property, data);
                        grid.Controls.Add(text);
                    }
                    //numbers
                    else if (valueType == typeof(int) || valueType == typeof(float))
                    {
                        var label = new Label() { AutoSize = true, Text = property.Name, Name = property.Name + "Label", Dock = DockStyle.Fill, ForeColor = Utils.brightText };
                        grid.Controls.Add(label);
                        var numeric = new NumericUpDown() { Minimum = int.MinValue, Maximum = int.MaxValue, Value = Convert.ToDecimal(value), Name = property.Name + "Numeric", ReadOnly = ReadOnly, InterceptArrowKeys = true };
                        if (valueType == typeof(int)) numeric.ValueChanged += (object? sender, EventArgs e) => NumericIntSetValue(sender, property, data);
                        else numeric.ValueChanged += (object? sender, EventArgs e) => NumericFloatSetValue(sender, property, data);
                        grid.Controls.Add(numeric);
                    }
                    //bool
                    else if (valueType == typeof(bool))
                    {
                        var label = new Label() { AutoSize = true, Text = property.Name, Name = property.Name + "Label", Dock = DockStyle.Fill, ForeColor = Utils.brightText };
                        grid.Controls.Add(label);
                        var checkBox = new CheckBox { Checked = Convert.ToBoolean(value), Name = property.Name + "Checkbox", Enabled = !ReadOnly };
                        checkBox.CheckedChanged += (object? sender, EventArgs e) => CheckBoxSetValue(sender, property, data);
                        grid.Controls.Add(checkBox);
                    }
                    //nullable enum
                    else if (valueType.GenericTypeArguments.Length > 0)
                    {
                        if (valueType.GenericTypeArguments[0].IsEnum)
                        {
                            var label = new Label() { AutoSize = true, Text = property.Name, Name = property.Name + "Label", Dock = DockStyle.Fill, ForeColor = Utils.brightText };
                            grid.Controls.Add(label);
                            var dropDown = new ComboBox() { Name = property.Name + "ComboBox", DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Anchor = AnchorStyles.Left };
                            foreach (var enumItem in Enum.GetNames(valueType.GenericTypeArguments[0]))
                            {
                                dropDown.Items.Add(enumItem);
                            }
                            dropDown.SelectedItem = value?.ToString();
                            dropDown.SelectedValueChanged += (object? sender, EventArgs e) => DropDownNullableSetValue(sender, property, data);
                            dropDown.Enabled = !ReadOnly;
                            grid.Controls.Add(dropDown);
                        }
                    }
                    //enum
                    else if (valueType.IsEnum)
                    {
                        var label = new Label() { AutoSize = true, Text = property.Name, Name = property.Name + "Label", Dock = DockStyle.Fill, ForeColor = Utils.brightText };
                        grid.Controls.Add(label);
                        var dropDown = new ComboBox() { Name = property.Name + "ComboBox", DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill, Anchor = AnchorStyles.Left };
                        foreach (var enumItem in Enum.GetNames(valueType))
                        {
                            dropDown.Items.Add(enumItem);
                        }
                        dropDown.SelectedItem = value?.ToString();
                        dropDown.SelectedValueChanged += (object? sender, EventArgs e) => DropDownSetValue(sender, property, data);
                        dropDown.Enabled = !ReadOnly;
                        grid.Controls.Add(dropDown);
                    }
                }
                catch (Exception e)
                {
                    LogManager.Log(e.Message);
                }
            }

            box.ResumeLayout();
            return box;
        }

    }
}