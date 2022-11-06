using HousePartyTranslator.Helpers;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HousePartyTranslator.Managers
{
    internal static class History
    {
        static private readonly Stack<ICommand> history = new Stack<ICommand>();
        static private readonly Stack<ICommand> future = new Stack<ICommand>();
        static public bool CausedByHistory = false;


        public static void AddAction(ICommand command)
        {
            history.Push(command);
            if (history.Count > 110)
            {
                //after 110 elements, we remove the oldest 10
                Stack<ICommand> temp = new Stack<ICommand>(history.Count);
                for (int i = 0; i < history.Count; i++)
                    temp.Push(history.Pop());

                for (int i = 0; i < 10; i++)
                    _ = temp.Pop();

                for (int i = 0; i < temp.Count; i++)
                    history.Push(temp.Pop());
            }
            future.Clear();
        }

        public static void Undo()
        {
            if (history.Count > 0)
            {
                ICommand command = history.Pop();
                if (command != null)
                {
                    CausedByHistory = true;
                    command.Undo();
                    future.Push(command);
                    CausedByHistory = false;
                }
            }
        }

        public static void Redo()
        {
            if (future.Count > 0)
            {
                ICommand command = future.Pop();
                if (command != null)
                {
                    CausedByHistory = true;
                    command.Do();
                    history.Push(command);
                    CausedByHistory = false;
                }
            }
        }
    }

    internal interface ICommand
    {
        void Do();
        void Undo();
    }

    internal sealed class TextAdded : ICommand
    {
        readonly System.Windows.Forms.TextBox TextBox;
        readonly string AddedText;

        public TextAdded(System.Windows.Forms.TextBox textBox, string addedText)
        {
            TextBox = textBox;
            AddedText = addedText;
        }

        void ICommand.Do()
        {
            TextBox.Text += AddedText;
        }

        void ICommand.Undo()
        {
            TextBox.Text = TextBox.Text.Remove(0, TextBox.Text.Length - AddedText.Length);
        }
    }

    internal sealed class TextRemoved : ICommand
    {
        readonly System.Windows.Forms.TextBox TextBox;
        readonly string RemovedText;

        public TextRemoved(System.Windows.Forms.TextBox textBox, string removedText)
        {
            TextBox = textBox;
            RemovedText = removedText;
        }

        void ICommand.Do()
        {
            TextBox.Text = TextBox.Text.Remove(0, TextBox.Text.Length - RemovedText.Length);
        }

        void ICommand.Undo()
        {
            TextBox.Text += RemovedText;
        }
    }

    internal sealed class TextChanged : ICommand
    {
        readonly System.Windows.Forms.TextBox TextBox;
        readonly string oldText;
        readonly string newText;

        public TextChanged(System.Windows.Forms.TextBox textBox, string oldText, string newText)
        {
            TextBox = textBox;
            this.oldText = oldText;
            this.newText = newText;
        }

        void ICommand.Do()
        {
            TextBox.Text = newText;
            TextBox.SelectionStart = TextBox.Text.Length;
        }

        void ICommand.Undo()
        {
            TextBox.Text = oldText;
            TextBox.SelectionStart = TextBox.Text.Length;
        }
    }

    internal sealed class ApprovedChanged : ICommand
    {
        readonly int index;
        readonly Helpers.ColouredCheckedListBox ListBox;
        public ApprovedChanged(int selectedIndex, Helpers.ColouredCheckedListBox listBox)
        {
            index = selectedIndex;
            ListBox = listBox;
        }

        void ICommand.Do()
        {
            ListBox.SetItemCheckState(index, ListBox.GetItemChecked(index) ? System.Windows.Forms.CheckState.Unchecked : System.Windows.Forms.CheckState.Checked);
        }

        void ICommand.Undo()
        {
            ListBox.SetItemCheckState(index, ListBox.GetItemChecked(index) ? System.Windows.Forms.CheckState.Unchecked : System.Windows.Forms.CheckState.Checked);
        }
    }

    internal sealed class SelectedLineChanged : ICommand
    {
        readonly int oldIndex;
        readonly int newIndex;
        readonly Helpers.ColouredCheckedListBox ListBox;
        public SelectedLineChanged(Helpers.ColouredCheckedListBox listBox, int oldIndex, int newIndex)
        {
            this.oldIndex = oldIndex;
            this.newIndex = newIndex;
            ListBox = listBox;
        }

        void ICommand.Do()
        {
            if (newIndex >= 0 && newIndex < ListBox.Items.Count) ListBox.SelectedIndex = newIndex;
        }

        void ICommand.Undo()
        {
            if (newIndex >= 0 && newIndex < ListBox.Items.Count) ListBox.SelectedIndex = oldIndex;
        }
    }

    internal sealed class TranslationChanged : ICommand
    {
        readonly TranslationManager manager;
        readonly string id;
        readonly string oldText;
        readonly string newText;

        public TranslationChanged( TranslationManager manager,string id, string oldText, string newText)
        {
            this.manager = manager;
            this.id = id;
            this.oldText = oldText;
            this.newText = newText;
        }

        void ICommand.Do()
        {
            manager.TranslationData[id].TranslationString = newText;
        }

        void ICommand.Undo()
        {
            manager.TranslationData[id].TranslationString = oldText;
        }
    }

    internal sealed class SelectedTabChanged : ICommand
    {
        readonly int oldTabIndex, newTabIndex;

        public SelectedTabChanged(int oldTabIndex, int newTabIndex)
        {
            this.oldTabIndex = oldTabIndex;
            this.newTabIndex = newTabIndex;
        }

        void ICommand.Do()
        {
            TabManager.SwitchToTab(newTabIndex);
        }

        void ICommand.Undo()
        {
            TabManager.SwitchToTab(oldTabIndex);
        }
    }

    internal sealed class AllTranslationsChanged : ICommand
    {
        readonly FileData oldTranslations, newTranslations;
        readonly TranslationManager manager;
        readonly string language;

        public AllTranslationsChanged(TranslationManager manager, FileData oldTranslations, FileData newTranslations)
        {
            this.oldTranslations = new FileData(oldTranslations);
            this.newTranslations = new FileData(newTranslations);
            this.manager = manager;
            this.language = TranslationManager.Language;
        }

        void ICommand.Do()
        {
            manager.TranslationData.Clear();
            foreach (var item in newTranslations)
            {
                manager.TranslationData.Add(item.Key, item.Value);
            }
            //update translations also on the database
            DataBase.UpdateTranslations(newTranslations, language);
            manager.ReloadTranslationTextbox();
        }

        void ICommand.Undo()
        {
            manager.TranslationData.Clear();
            foreach (var item in oldTranslations)
            {
                manager.TranslationData.Add(item.Key, item.Value);
            }
            //update translations also on the database
            DataBase.UpdateTranslations(oldTranslations, language);
            manager.ReloadTranslationTextbox();
        }
    }
}
