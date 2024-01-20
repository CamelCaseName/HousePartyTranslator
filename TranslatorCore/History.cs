using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Translator.Core.Data;
using Translator.Core.Helpers;
using Translator.Core.UICompatibilityLayer;

namespace Translator.Core
{
    public static class History
    {
        private static readonly Stack<ICommand> history = new();
        private static readonly Stack<ICommand> future = new();
        private static bool CausedByHistory = false;
        public static event EventHandler? HistoryChanged;

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
                LogManager.LogDebug("History action added by " + LogManager.ExtractCSFileNameFromPath(callerFile) + '<' + callerName + ">:" + lineNumber.ToString());
                LogManager.LogDebug("L__" + command.ToString() + $" - {command.StoryName}\\{command.FileName}");
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

                HistoryChanged?.Invoke(null, null!);
            }
        }

        public static ICommand Peek()
        {
            return history.Count > 0 ? history.Peek() : NullCommand.Instance;
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
            var temp = new Stack<ICommand>(history.Count);
            //check all history items
            for (int i = history.Count; i > 0; --i)
            {
                ICommand item = history.Pop();
                if (item.StoryName != StoryName || item.FileName != FileName || item.GetType() == typeof(SelectedTabChanged))
                    temp.Push(item);
            }
            for (int i = temp.Count; i > 0; --i)
            {
                history.Push(temp.Pop());
            }
            temp.Clear();
            //check all future items
            for (int i = future.Count; i > 0; --i)
            {
                ICommand item = future.Pop();
                if (item.StoryName != StoryName || item.FileName != FileName || item.GetType() == typeof(SelectedTabChanged))
                    temp.Push(item);
            }
            for (int i = temp.Count; i > 0; --i)
            {
                future.Push(temp.Pop());
            }
            temp.Clear();
        }

        public static void Undo()
        {
            if (history.Count > 0)
            {
                ICommand command = history.Pop();
                if (command is not null)
                {
                    CausedByHistory = true;
                    command.Undo();
                    future.Push(command);
                    LogManager.LogDebug($"undid {command} - {command.StoryName}\\{command.FileName} | history length now: ({history.Count})");
                    CausedByHistory = false;
                    HistoryChanged?.Invoke(null, null!);
                }
            }
        }

        public static void Undo(int count)
        {
            if (history.Count < count) return;
            for (int i = 0; i < count; i++)
            {
                Undo();
            }
        }

        public static void Redo()
        {
            if (future.Count > 0)
            {
                ICommand command = future.Pop();
                if (command is not null)
                {
                    CausedByHistory = true;
                    command.Do();
                    history.Push(command);
                    LogManager.LogDebug($"redid {command} - {command.StoryName}\\{command.FileName} | history length now: ({history.Count})");
                    CausedByHistory = false;
                    HistoryChanged?.Invoke(null, null!);
                }
            }
        }

        public static void Redo(int count)
        {
            if (future.Count < count) return;
            for (int i = 0; i < count; i++)
            {
                Redo();
            }
        }

        public static void Clear()
        {
            history.Clear();
            future.Clear();
            CausedByHistory = false;
            HistoryChanged?.Invoke(null, null!);
        }

        //returns the latest 5 entries in the history
        public static List<ICommand> GetLastFiveActions()
        {
            List<ICommand> actions = new();
            var enumerator = history.GetEnumerator();
            while (enumerator.MoveNext() && actions.Count < 5)
            {
                actions.Add(enumerator.Current);
            }
            return actions;
        }

        //returns the newest 5 entries in the future
        public static List<ICommand> GetNextFiveActions()
        {
            List<ICommand> actions = new();
            var enumerator = future.GetEnumerator();
            while (enumerator.MoveNext() && actions.Count < 5)
            {
                actions.Add(enumerator.Current);
            }
            return actions;
        }
    }

    public interface ICommand
    {
        string FileName { get; }
        string StoryName { get; }
        string Description { get; }
        string DetailedDescription { get; }
        void Do();
        void Undo();
    }

    public record NullCommand : ICommand
    {
        public static NullCommand Instance { get; } = new();
        public string FileName { get => "none"; }
        public string StoryName { get => "none"; }
        public string Description { get => "none"; }
        public string DetailedDescription { get => "none"; }

        public NullCommand() { }

        public void Do() { }
        public void Undo() { }
    }

    public record TextAdded : ICommand
    {
        private readonly ITextBox TextBox;
        private readonly string AddedText;

        public TextAdded(ITextBox textBox, string addedText, string fileName, string storyName)
        {
            TextBox = textBox;
            AddedText = addedText;
            FileName = fileName;
            StoryName = storyName;
            DetailedDescription = "Added " + addedText + " in " + storyName + "/" + fileName;
        }

        public string FileName { get; }
        public string StoryName { get; }
        public string Description { get => "Added Text"; }
        public string DetailedDescription { get; }

        public void Do()
        {
            TextBox.Text += AddedText;
        }

        public void Undo()
        {
            TextBox.Text = TextBox.Text.Remove(0, TextBox.Text.Length - AddedText.Length);
        }
    }

    public record TextRemoved : ICommand
    {
        private readonly ITextBox TextBox;
        private readonly string RemovedText;

        public TextRemoved(ITextBox textBox, string removedText, string fileName, string storyName)
        {
            TextBox = textBox;
            RemovedText = removedText;
            FileName = fileName;
            StoryName = storyName;
            DetailedDescription = "removed " + removedText + " from " + storyName + "/" + fileName;
        }

        public string FileName { get; }
        public string StoryName { get; }
        public string Description { get => "Removed Text"; }
        public string DetailedDescription { get; }

        public void Do()
        {
            TextBox.Text = TextBox.Text.Remove(0, TextBox.Text.Length - RemovedText.Length);
        }

        public void Undo()
        {
            TextBox.Text += RemovedText;
        }
    }

    public record TextChanged : ICommand
    {
        private readonly ITextBox TextBox;
        private readonly string oldText;
        private readonly string newText;

        public TextChanged(ITextBox textBox, string oldText, string newText, string fileName, string storyName)
        {
            TextBox = textBox;
            this.oldText = oldText;
            this.newText = newText;
            FileName = fileName;
            StoryName = storyName;
            DetailedDescription = "changed " + oldText + " to " + newText + " in " + storyName + "/" + fileName;
        }

        public string FileName { get; }
        public string StoryName { get; }
        public string Description { get => "Changed Text"; }
        public string DetailedDescription { get; }

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

    public record ApprovedChanged : ICommand

    {
        private readonly int index;
        private readonly ILineList ListBox;
        private readonly TranslationManager manager;
        private readonly bool isApproved;
        public ApprovedChanged(int selectedIndex, ILineList listBox, TranslationManager manager, string fileName, string storyName)
        {
            index = selectedIndex;
            ListBox = listBox;
            this.manager = manager;
            FileName = fileName;
            StoryName = storyName;
            isApproved = ListBox.GetApprovalState(selectedIndex);
            DetailedDescription = (isApproved ? "Approved " : "Unapproved ") + listBox[selectedIndex].ToString() + " in " + storyName + "/" + fileName;
        }

        public string FileName { get; }
        public string StoryName { get; }
        public string Description { get => "Approval state changed"; }
        public string DetailedDescription { get; }

        public void Do()
        {
            ListBox.SetApprovalState(index, isApproved);
            manager.UpdateSimilarityMarking(ListBox[index].ID);
        }

        public void Undo()
        {
            ListBox.SetApprovalState(index, !isApproved);
            manager.UpdateSimilarityMarking(ListBox[index].ID);
        }
    }

    public record SelectedLineChanged : ICommand
    {
        private readonly int oldIndex;
        private readonly int newIndex;
        private readonly ILineList ListBox;
        public SelectedLineChanged(ILineList listBox, int oldIndex, int newIndex, string fileName, string storyName)
        {
            this.oldIndex = oldIndex;
            this.newIndex = newIndex;
            ListBox = listBox;
            FileName = fileName;
            StoryName = storyName;
            DetailedDescription = "Selected line " + listBox[newIndex].ToString() + " in " + storyName + "/" + fileName;
        }

        public string FileName { get; }
        public string StoryName { get; }
        public string Description { get => "Selected line changed"; }
        public string DetailedDescription { get; }

        public void Do()
        {
            if (newIndex >= 0 && newIndex < ListBox.Count) ListBox.SelectedIndex = newIndex;
        }

        public void Undo()
        {
            if (newIndex >= 0 && newIndex < ListBox.Count) ListBox.SelectedIndex = oldIndex;
        }
    }

    public record TranslationChanged : ICommand
    {
        private readonly TranslationManager manager;
        private readonly EekStringID id;
        private readonly string oldText;
        private readonly string newText;

        public TranslationChanged(TranslationManager manager, EekStringID id, string oldText, string newText)
        {
            this.manager = manager;
            this.id = id;
            this.oldText = oldText;
            this.newText = newText;
            FileName = manager.FileName;
            StoryName = manager.StoryName;
            DetailedDescription = "Translation changed in line " + id + " in " + manager.StoryName + "/" + manager.FileName + " from " + oldText + " to " + newText;
        }

        public string FileName { get; }
        public string StoryName { get; }
        public string Description { get => "Translations changed"; }
        public string DetailedDescription { get; }

        public void Do()
        {
            manager.TranslationData[id].TranslationString = newText;
            manager.UpdateSimilarityMarking(id);
        }

        public void Undo()
        {
            manager.TranslationData[id].TranslationString = oldText;
            manager.UpdateSimilarityMarking(id);
        }
    }

    public record SelectedTabChanged : ICommand
    {
        private readonly int oldTabIndex, newTabIndex;

        public SelectedTabChanged(int oldTabIndex, int newTabIndex, string storyName, string fileName)
        {
            this.oldTabIndex = oldTabIndex;
            this.newTabIndex = newTabIndex;
            FileName = fileName;
            StoryName = storyName;
            DetailedDescription = "Selected tab " + newTabIndex;
        }

        public string FileName { get; } = "none";
        public string StoryName { get; } = "none";
        public string Description { get => "Selected Tab Changed"; }
        public string DetailedDescription { get; }

        public void Do()
        {
            TabManager.SwitchToTab(newTabIndex);
        }

        public void Undo()
        {
            TabManager.SwitchToTab(oldTabIndex);
        }
    }

    public record AllTranslationsChanged : ICommand
    {
        private readonly FileData oldTranslations, newTranslations;
        private readonly TranslationManager manager;

        public AllTranslationsChanged(TranslationManager manager, FileData oldTranslations, FileData newTranslations)
        {
            this.oldTranslations = new FileData(oldTranslations, manager.StoryName, manager.FileName);
            this.newTranslations = new FileData(newTranslations, manager.StoryName, manager.FileName);
            this.manager = manager;
            FileName = manager.FileName;
            StoryName = manager.StoryName;
            DetailedDescription = "All translations changed in " + manager.StoryName + "/" + manager.FileName;
        }

        public string FileName { get; }
        public string StoryName { get; }
        public string Description { get => "All translations changed"; }
        public string DetailedDescription { get; }

        public void Do()
        {
            manager.TranslationData = newTranslations;
            foreach (KeyValuePair<EekStringID, LineData> item in newTranslations)
            {
                manager.UpdateSimilarityMarking(item.Key);
            }
            manager.ReloadTranslationTextbox();
        }

        public void Undo()
        {
            manager.TranslationData = oldTranslations;
            foreach (KeyValuePair<EekStringID, LineData> item in oldTranslations)
            {
                manager.UpdateSimilarityMarking(item.Key);
            }
            manager.ReloadTranslationTextbox();
        }
    }
}
