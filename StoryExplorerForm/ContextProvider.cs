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
                        Filter = "Story Files (*.story)|*.story",
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
            if (Properties.Settings.Default.story_path != "")
            {
                this.StoryFilePath = Properties.Settings.Default.story_path;
            }
            else
            {
                this.StoryFilePath = StoryFilePath;
            }
        }

        public bool ParseFile()
        {
            if (File.Exists(StoryFilePath))
            {
                string fileString = File.ReadAllText(StoryFilePath);
                //save path
                Properties.Settings.Default.story_path = StoryFilePath;

                MainStory story = JsonConvert.DeserializeObject<MainStory>(fileString);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}