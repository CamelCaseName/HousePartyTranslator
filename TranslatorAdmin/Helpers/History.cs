using Translator.Helpers;
using System.Runtime.CompilerServices;

namespace Translator.Managers
{
    internal static class History
    {
        private static readonly Stack<ICommand> history = new Stack<ICommand>();
        private static readonly Stack<ICommand> future = new Stack<ICommand>();
        public static bool CausedByHistory = false;

#if TRACE
        public static void AddAction(ICommand command, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
#else
            public static void AddAction(ICommand command)
        {
#endif
            if (!CausedByHistory)
            {
#if TRACE
                Console.WriteLine("History action added by " + callerFile + '<' + callerName + ">:" + lineNumber.ToString());
                Console.WriteLine("L__" + command.ToString() + $" - {command.StoryName}\\{command.FileName}");
#endif
                history.Push(command);
                if (history.Count > 110)
                {
                    //after 110 elements, we remove the oldest 10
                    var temp = new Stack<ICommand>(history.Count);
                    for (int i = 0; i < history.Count; i++)
                        temp.Push(history.Pop());

                    for (int i = 0; i < 10; i++)
                        _ = temp.Pop();

                    for (int i = 0; i < temp.Count; i++)
                        history.Push(temp.Pop());
                }
                future.Clear();
            }
        }

        public static ICommand Peek()
        {
            return history.Count > 0 ? history.Peek() : new ICommandInstance();
        }

        /// <summary>
        /// removes all history actions tied to the file, but not the tab
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="StoryName"></param>
#if TRACE
        public static void ClearForFile(string FileName, string StoryName, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0)
        {
            Console.WriteLine("History cleared by " + callerFile + '<' + callerName + ">:" + lineNumber.ToString());
            Console.WriteLine($"L__ cleared for {StoryName}\\{FileName}");
#else
            public static void ClearForFile(string FileName, string StoryName)
            {

#endif
            var temp = new Stack<ICommand>(history);
            //check all history items
            for (int i = 0; i < history.Count; i++)
            {
                ICommand item = history.Pop();
                if (item.StoryName != StoryName || item.FileName != FileName || item.GetType() == typeof(SelectedTabChanged))
                    temp.Push(item);
            }
            for (int i = 0; i < temp.Count; i++)
            {
                history.Push(temp.Pop());
            }
            temp.Clear();
            //check all future items
            for (int i = 0; i < future.Count; i++)
            {
                ICommand item = future.Pop();
                if (item.StoryName != StoryName || item.FileName != FileName || item.GetType() == typeof(SelectedTabChanged))
                    temp.Push(item);
            }
            for (int i = 0; i < temp.Count; i++)
            {
                future.Push(temp.Pop());
            }
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

        public static void Clear()
        {
            history.Clear();
            future.Clear();
            CausedByHistory = false;
        }
    }

    internal interface ICommand
    {
        string FileName { get; set; }
        string StoryName { get; set; }
        void Do();
        void Undo();
    }

    internal sealed class ICommandInstance : ICommand
    {
        public string FileName { get => "none"; set { } }
        public string StoryName { get => "none"; set { } }

        public ICommandInstance() { }

        public void Do() { }
        public void Undo() { }
    }

    internal sealed class TextAdded : ICommand
    {
        readonly TextBox TextBox;
        readonly string AddedText;

        public TextAdded(TextBox textBox, string addedText, string fileName, string storyName)
        {
            TextBox = textBox;
            AddedText = addedText;
            FileName = fileName;
            StoryName = storyName;
        }

        public string FileName { get; set; }
        public string StoryName { get; set; }

        public void Do()
        {
            TextBox.Text += AddedText;
        }

        public void Undo()
        {
            TextBox.Text = TextBox.Text.Remove(0, TextBox.Text.Length - AddedText.Length);
        }
    }

    internal sealed class TextRemoved : ICommand
    {
        readonly TextBox TextBox;
        readonly string RemovedText;

        public TextRemoved(TextBox textBox, string removedText, string fileName, string storyName)
        {
            TextBox = textBox;
            RemovedText = removedText;
            FileName = fileName;
            StoryName = storyName;
        }

        public string FileName { get; set; }
        public string StoryName { get; set; }

        public void Do()
        {
            TextBox.Text = TextBox.Text.Remove(0, TextBox.Text.Length - RemovedText.Length);
        }

        public void Undo()
        {
            TextBox.Text += RemovedText;
        }
    }

    internal sealed class TextChanged : ICommand
    {
        readonly TextBox TextBox;
        readonly string oldText;
        readonly string newText;

        public TextChanged(TextBox textBox, string oldText, string newText, string fileName, string storyName)
        {
            TextBox = textBox;
            this.oldText = oldText;
            this.newText = newText;
            FileName = fileName;
            StoryName = storyName;
        }

        public string FileName { get; set; }
        public string StoryName { get; set; }

        public void Do()
        {
            TextBox.Text = newText;
            TextBox.SelectionStart = TextBox.Text.Length;
        }

        public void Undo()
        {
            TextBox.Text = oldText;
            TextBox.SelectionStart = TextBox.Text.Length;
        }
    }

    internal sealed class ApprovedChanged : ICommand
    {
        readonly int index;
        readonly ColouredCheckedListBox ListBox;
        public ApprovedChanged(int selectedIndex, ColouredCheckedListBox listBox, string fileName, string storyName)
        {
            index = selectedIndex;
            ListBox = listBox;
            FileName = fileName;
            StoryName = storyName;
        }

        public string FileName { get; set; }
        public string StoryName { get; set; }

        public void Do()
        {
            ListBox.SetItemCheckState(index, ListBox.GetItemChecked(index) ? System.Windows.Forms.CheckState.Unchecked : System.Windows.Forms.CheckState.Checked);
        }

        public void Undo()
        {
            ListBox.SetItemCheckState(index, ListBox.GetItemChecked(index) ? System.Windows.Forms.CheckState.Unchecked : System.Windows.Forms.CheckState.Checked);
        }
    }

    internal sealed class SelectedLineChanged : ICommand
    {
        readonly int oldIndex;
        readonly int newIndex;
        readonly ColouredCheckedListBox ListBox;
        public SelectedLineChanged(ColouredCheckedListBox listBox, int oldIndex, int newIndex, string fileName, string storyName)
        {
            this.oldIndex = oldIndex;
            this.newIndex = newIndex;
            ListBox = listBox;
            FileName = fileName;
            StoryName = storyName;
        }

        public string FileName { get; set; }
        public string StoryName { get; set; }

        public void Do()
        {
            if (newIndex >= 0 && newIndex < ListBox.Items.Count) ListBox.SelectedIndex = newIndex;
        }

        public void Undo()
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

        public TranslationChanged(TranslationManager manager, string id, string oldText, string newText)
        {
            this.manager = manager;
            this.id = id;
            this.oldText = oldText;
            this.newText = newText;
            FileName = manager.FileName;
            StoryName = manager.StoryName;
        }

        public string FileName { get; set; }
        public string StoryName { get; set; }

        public void Do()
        {
            manager.TranslationData[id].TranslationString = newText;
        }

        public void Undo()
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

        public string FileName { get; set; } = "none";
        public string StoryName { get; set; } = "none";

        public void Do()
        {
            TabManager.SwitchToTab(newTabIndex);
        }

        public void Undo()
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
            FileName = manager.FileName;
            StoryName = manager.StoryName;
        }

        public string FileName { get; set; }
        public string StoryName { get; set; }

        public void Do()
        {
            manager.TranslationData.Clear();
            foreach (KeyValuePair<string, LineData> item in newTranslations)
            {
                manager.TranslationData[item.Key] = item.Value;
            }
            //update translations also on the database
            _ = DataBase.UpdateTranslations(newTranslations, language);
            manager.ReloadTranslationTextbox();
        }

        public void Undo()
        {
            manager.TranslationData.Clear();
            foreach (KeyValuePair<string, LineData> item in oldTranslations)
            {
                manager.TranslationData[item.Key] = item.Value;
            }
            //update translations also on the database
            _ = DataBase.UpdateTranslations(oldTranslations, language);
            manager.ReloadTranslationTextbox();
        }
    }
}
