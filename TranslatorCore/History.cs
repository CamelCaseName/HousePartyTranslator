﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Translator.UICompatibilityLayer;
using Translator.Core.Helpers;

namespace Translator.Core
{
    public static class History
    {
        private static readonly Stack<ICommand> history = new();
        private static readonly Stack<ICommand> future = new();
        private static bool CausedByHistory = false;

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
            return history.Count > 0 ? history.Peek() : NullCommand.Instance;
        }

        /// <summary>
        /// removes all history actions tied to the file, but not the tab
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="StoryName"></param>
#if TRACE
        public static void ClearForFile<T>(string FileName, string StoryName, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerName = "", [CallerLineNumber] int lineNumber = 0) where T : class, ILineItem, new()
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
                if (item.StoryName != StoryName || item.FileName != FileName || item.GetType() == typeof(SelectedTabChanged<T>))
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
                if (item.StoryName != StoryName || item.FileName != FileName || item.GetType() == typeof(SelectedTabChanged<T>))
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

    public interface ICommand
    {
        string FileName { get; set; }
        string StoryName { get; set; }
        void Do();
        void Undo();
    }

    public sealed class NullCommand : ICommand
    {
        public static NullCommand Instance { get; } = new();
        public string FileName { get => "none"; set { } }
        public string StoryName { get => "none"; set { } }

        public NullCommand() { }

        public void Do() { }
        public void Undo() { }
    }

    public sealed class TextAdded : ICommand
    {
        readonly ITextBox TextBox;
        readonly string AddedText;

        public TextAdded(ITextBox textBox, string addedText, string fileName, string storyName)
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

    public sealed class TextRemoved : ICommand
    {
        readonly ITextBox TextBox;
        readonly string RemovedText;

        public TextRemoved(ITextBox textBox, string removedText, string fileName, string storyName)
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

    public sealed class TextChanged : ICommand
    {
        readonly ITextBox TextBox;
        readonly string oldText;
        readonly string newText;

        public TextChanged(ITextBox textBox, string oldText, string newText, string fileName, string storyName)
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

    public sealed class ApprovedChanged<T> : ICommand where T : class, ILineItem, new()
    {
        readonly int index;
        readonly LineList<T> ListBox;
        public ApprovedChanged(int selectedIndex, LineList<T> listBox, string fileName, string storyName)
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
            ListBox.SetApprovalState(index, !ListBox.GetApprovalState(index));
        }

        public void Undo()
        {
            ListBox.SetApprovalState(index, !ListBox.GetApprovalState(index));
        }
    }

    public sealed class SelectedLineChanged<T> : ICommand where T : class, ILineItem, new()
    {
        readonly int oldIndex;
        readonly int newIndex;
        readonly LineList<T> ListBox;
        public SelectedLineChanged(LineList<T> listBox, int oldIndex, int newIndex, string fileName, string storyName)
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

    public sealed class TranslationChanged<T> : ICommand where T : class, ILineItem, new()
    {
        readonly TranslationManager<T> manager;
        readonly string id;
        readonly string oldText;
        readonly string newText;

        public TranslationChanged(TranslationManager<T> manager, string id, string oldText, string newText)
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

    public sealed class SelectedTabChanged<T> : ICommand where T : class, ILineItem, new()
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
            TabManager<T>.SwitchToTab(newTabIndex);
        }

        public void Undo()
        {
            TabManager<T>.SwitchToTab(oldTabIndex);
        }
    }

    public sealed class AllTranslationsChanged<T> : ICommand where T : class, ILineItem, new()
    {
        readonly FileData oldTranslations, newTranslations;
        readonly TranslationManager<T> manager;
        readonly string language;

        public AllTranslationsChanged(TranslationManager<T> manager, FileData oldTranslations, FileData newTranslations)
        {
            this.oldTranslations = new FileData(oldTranslations);
            this.newTranslations = new FileData(newTranslations);
            this.manager = manager;
            this.language = TranslationManager<T>.Language;
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
            _ = DataBase<T>.UpdateTranslations(newTranslations, language);
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
            _ = DataBase<T>.UpdateTranslations(oldTranslations, language);
            manager.ReloadTranslationTextbox();
        }
    }
}
