using HousePartyTranslator.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace HousePartyTranslator
{
    internal class ContextProvider
    {
        private readonly Panel DrawingPanel;
        private string _StoryFilePath;
        private Graphics DrawingSpace;

        public string StoryFilePath
        {
            get
            {
                return _StoryFilePath;
            }
            set
            {
                if (File.Exists(value))
                {
                    _StoryFilePath = value;
                }
                else
                {
                    OpenFileDialog selectFileDialog = new OpenFileDialog
                    {
                        Title = "Choose the story file you want to explore",
                        Filter = "Text files (*.txt)|*.txt",
                        InitialDirectory = @"C:\Users\%USER%\Documents"
                    };

                    if (selectFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        StoryFilePath = selectFileDialog.FileName;
                    }
                    else
                    {
                        //close form if cancelled
                        DrawingPanel.FindForm().Close();
                    }
                }
            }
        }

        public ContextProvider(string StoryFilePath, Panel DrawingSpace)
        {
            DrawingPanel = DrawingSpace;
            this.DrawingSpace = DrawingPanel.CreateGraphics();
            this.StoryFilePath = StoryFilePath;
        }

        public bool ParseFile()
        {
            if (File.Exists(StoryFilePath))
            {
                string fileString = File.ReadAllText(StoryFilePath);
                List<StoryItem> items = JsonConvert.DeserializeObject<List<StoryItem>>(fileString);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}