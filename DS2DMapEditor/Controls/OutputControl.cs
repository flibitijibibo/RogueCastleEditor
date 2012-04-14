using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace RogueCastleEditor
{
    public class OutputControl : StackPanel, IControl
    {
        public MainWindow ControllerRef { get; set; }

        private static ListView m_listView;

        public OutputControl()
        {
            m_listView = new ListView();

            m_listView.Height = 167;
            m_listView.FontSize = 14;
            m_listView.FontFamily = new FontFamily("Arial");
            //m_listView.Background = Brushes.Black;
            this.Children.Add(m_listView);
            m_listView.ItemContainerGenerator.StatusChanged += new EventHandler(ContainerStatusChanged);
        }

        private void ContainerStatusChanged(object sender, EventArgs e)
        {
            if (m_listView.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                Brush textBrush;
                for (int i = 0; i < m_listView.Items.Count; i++)
                {
                    string text = m_listView.Items[i] as string;
                    if (text.Substring(0, 5).ToUpper() == "ERROR")
                        textBrush = Brushes.Red;
                    else if (text.Substring(0, 7).ToUpper() == "WARNING")
                        textBrush = Brushes.Blue;
                    else
                        textBrush = Brushes.Black;
                    (m_listView.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem).Foreground = textBrush;
                }
            }
        }

        //0 is black, 1 is yellow, 2 is red.
        public static void Trace(string text)
        {
            m_listView.Items.Add(text);
        }

        public static void Clear()
        {
            m_listView.Items.Clear();
        }
    }
}
