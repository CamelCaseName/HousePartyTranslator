using System.Collections.Generic;

namespace HousePartyTranslator.Managers
{
    static class History
    {
        static private readonly Stack<ICommand> history = new Stack<ICommand>();
        static private readonly Stack<ICommand> future = new Stack<ICommand>();


        static public void AddAction(ICommand command)
        {
            history.Push(command);
            future.Clear();
        }

        static public void Undo()
        {
            ICommand command = history.Pop();
            command.Undo();
            future.Push(command);
        }

        static public void Redo()
        {
            ICommand command = future.Pop();
            command.Do();
            history.Push(command);
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


}
