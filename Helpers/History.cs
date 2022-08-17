using System.Collections.Generic;

namespace HousePartyTranslator.Managers
{
    static class History
    {
        static private readonly Stack<ICommand> history = new Stack<ICommand>();
        static private readonly Stack<ICommand> future = new Stack<ICommand>();
        static public bool CausedByHistory = false;


        static public void AddAction(ICommand command)
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

        static public void Undo()
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

        static public void Redo()
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

    interface ICommand
    {
        void Do();
        void Undo();
    }

    class TextAdded : ICommand
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

    class TextRemoved : ICommand
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

    class TextChanged : ICommand
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

    class ApprovedChanged : ICommand
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

    class SelectedLineChanged : ICommand
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
}
