using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastleEditor
{
    public class PropertiesControl : StackPanel, IControl
    {
        IPropertiesObj m_selectedObj;
        public MainWindow ControllerRef { get; set; }

        public PropertiesControl()
        {
        }

        public void ShowObjProperties(IPropertiesObj obj)
        {
            this.Children.Clear();

            // Clears out the obj properties control if null is passed in.
            if (obj == null)
                return;

            m_selectedObj = obj;

            CreateNewTextBox("Name:", "Name", obj.Name);
            CreateNewTextBox("X:", "X", obj.X.ToString());
            CreateNewTextBox("Y:", "Y", obj.Y.ToString());
            CreateNewTextBox("Width:", "Width", obj.Width.ToString());
            CreateNewTextBox("Height:", "Height", obj.Height.ToString());
            CreateNewTextBox("ScaleX:", "ScaleX", obj.ScaleX.ToString());
            CreateNewTextBox("ScaleY:", "ScaleY", obj.ScaleY.ToString());
            CreateNewTextBox("Rotation:", "Rotation", obj.Rotation.ToString());
            CreateNewTextBox("Custom Tags", "Tags", (obj as GameObj).Tag);

            if (!(obj is CollHullObj))
            {           
                TextBlock newBlock = new TextBlock();
                newBlock.Margin = new Thickness(0, 20, 0, 0);
                newBlock.Text = "Flip Horizontally:";
                this.Children.Add(newBlock);
                CheckBox newCheckBox = new CheckBox();
                newCheckBox.Name = "FlipCheckBox";
                if (m_selectedObj.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally)
                    newCheckBox.IsChecked = true;
                newCheckBox.Margin = new System.Windows.Thickness(0, 5, 0, 0);
                newCheckBox.Checked += CheckBoxEventHandler;
                newCheckBox.Unchecked += CheckBoxEventHandler;
                this.Children.Add(newCheckBox);

                // Disables width/height modification for objects that do not inherit from CollHullObj
                FindBox("Width").IsEnabled = false;
                FindBox("Height").IsEnabled = false;
            }
            else
            {
                FindBox("ScaleX").IsEnabled = false;
                FindBox("ScaleY").IsEnabled = false;

                TextBlock triggerBlock = new TextBlock();
                triggerBlock.Margin = new Thickness(0, 20, 0, 0);
                triggerBlock.Text = "Is Trigger Object";
                this.Children.Add(triggerBlock);
                CheckBox newCheckBox = new CheckBox();
                newCheckBox.Name = "TriggerCheckBox";
                if ((m_selectedObj as CollHullObj).IsTrigger == true)
                    newCheckBox.IsChecked = true;
                newCheckBox.Margin = new System.Windows.Thickness(0, 5, 0, 0);
                newCheckBox.Checked += CheckBoxEventHandler;
                newCheckBox.Unchecked += CheckBoxEventHandler;
                this.Children.Add(newCheckBox);
                CreateNewCheckBox("Collides Top", "CollidesTop", (m_selectedObj as CollHullObj).CollidesTop);
                CreateNewCheckBox("Collides Bottom", "CollidesBottom", (m_selectedObj as CollHullObj).CollidesBottom);
                CreateNewCheckBox("Collides Left", "CollidesLeft", (m_selectedObj as CollHullObj).CollidesLeft);
                CreateNewCheckBox("Collides Right", "CollidesRight", (m_selectedObj as CollHullObj).CollidesRight);
            }

            if (obj is IAnimateableObj)
            {
                TextBlock playAnimationText = new TextBlock();
                playAnimationText.Text = "Play Animation:";
                this.Children.Add(playAnimationText);
                CheckBox playAnimCheckBox = new CheckBox();
                playAnimCheckBox.Name = "PlayAnimation";
                playAnimCheckBox.Margin = new System.Windows.Thickness(0, 5, 0, 0);
                if ((obj as IAnimateableObj).IsAnimating == true)
                    playAnimCheckBox.IsChecked = true;
                else
                    playAnimCheckBox.IsChecked = false;
                playAnimCheckBox.Checked += CheckBoxEventHandler;
                playAnimCheckBox.Unchecked += CheckBoxEventHandler;
                this.Children.Add(playAnimCheckBox);
            }

            if (obj is IPhysicsObj)
            {
                TextBlock collideText = new TextBlock();
                collideText.Text = "Is Collidable:";
                this.Children.Add(collideText);
                CheckBox collideCheckBox = new CheckBox();
                collideCheckBox.Name = "IsCollidable";
                collideCheckBox.Margin = new System.Windows.Thickness(0, 5, 0, 0);
                if ((obj as IPhysicsObj).IsCollidable == true)
                    collideCheckBox.IsChecked = true;
                collideCheckBox.Checked += CheckBoxEventHandler;
                collideCheckBox.Unchecked += CheckBoxEventHandler;
                this.Children.Add(collideCheckBox);
                TextBlock weightText = new TextBlock();
                weightText.Text = "Is Weighted:";
                this.Children.Add(weightText);
                CheckBox weightCheckBox = new CheckBox();
                weightCheckBox.Name = "IsWeighted";
                weightCheckBox.Margin = new System.Windows.Thickness(0, 5, 0, 0);
                if ((obj as IPhysicsObj).IsWeighted == true)
                    weightCheckBox.IsChecked = true;
                weightCheckBox.Checked += CheckBoxEventHandler;
                weightCheckBox.Unchecked += CheckBoxEventHandler;
                this.Children.Add(weightCheckBox);
            }

            if (obj is IRoomPropertiesObj)
            {
                TextBlock selectRoomAll = new TextBlock();
                selectRoomAll.Margin = new Thickness(0, 20, 0, 0);
                selectRoomAll.Text = "Select All Objects in Room";
                this.Children.Add(selectRoomAll);
                RadioButton rad1 = new RadioButton();
                rad1.GroupName = "selectionGroup";
                rad1.Name = "SelectAll";
                
                this.Children.Add(rad1);
                TextBlock selectLayer = new TextBlock();
                selectLayer.Text = "Select All Objects in Layer";
                this.Children.Add(selectLayer);
                RadioButton rad2 = new RadioButton();
                rad2.GroupName = "selectionGroup";
                rad2.Name = "SelectLayer";
                
                this.Children.Add(rad2);
                TextBlock selectNone = new TextBlock();
                selectNone.Text = "Select None";
                this.Children.Add(selectNone);
                RadioButton rad3 = new RadioButton();
                rad3.GroupName = "selectionGroup";
                rad3.Name = "SelectNone";
                
                this.Children.Add(rad3);
                if ((obj as IRoomPropertiesObj).SelectionMode == 0)
                    rad1.IsChecked = true;
                else if ((obj as IRoomPropertiesObj).SelectionMode == 1)
                    rad2.IsChecked = true;
                else
                    rad3.IsChecked = true;

                rad1.Checked += RadioButtonEventHandler;
                rad2.Checked += RadioButtonEventHandler;
                rad3.Checked += RadioButtonEventHandler;
            }

            if (obj is EnemyMapObject)
            {
                TextBlock enemyDifficultyBasic = new TextBlock();
                enemyDifficultyBasic.Margin = new Thickness(0, 20, 0, 0);
                enemyDifficultyBasic.Text = "Enemy Difficulty - Basic";
                this.Children.Add(enemyDifficultyBasic);
                RadioButton rad1 = new RadioButton();
                rad1.GroupName = "enemyDifficulty";
                rad1.Name = "EnemyDifficultyBasic";

                this.Children.Add(rad1);
                TextBlock enemyDifficultyAdvanced = new TextBlock();
                enemyDifficultyAdvanced.Text = "Enemy Difficulty - Advanced";
                this.Children.Add(enemyDifficultyAdvanced);
                RadioButton rad2 = new RadioButton();
                rad2.GroupName = "enemyDifficulty";
                rad2.Name = "EnemyDifficultyAdvanced";

                this.Children.Add(rad2);
                TextBlock enemyDifficultyExpert = new TextBlock();
                enemyDifficultyExpert.Text = "Enemy Difficulty - Expert";
                this.Children.Add(enemyDifficultyExpert);
                RadioButton rad3 = new RadioButton();
                rad3.GroupName = "enemyDifficulty";
                rad3.Name = "EnemyDifficultyExpert";

                this.Children.Add(rad3);
                TextBlock enemyDifficultyMiniboss = new TextBlock();
                enemyDifficultyMiniboss.Text = "Enemy Difficulty - Miniboss";
                this.Children.Add(enemyDifficultyMiniboss);
                RadioButton rad4 = new RadioButton();
                rad4.GroupName = "enemyDifficulty";
                rad4.Name = "EnemyDifficultyMiniboss";
                this.Children.Add(rad4);

                CreateNewTextBox("Initial Logic Delay (s)", "InitialDelay", (obj as EnemyMapObject).InitialLogicDelay.ToString());

                if ((obj as EnemyMapObject).Difficulty == EnemyDifficulty.BASIC)
                    rad1.IsChecked = true;
                else if ((obj as EnemyMapObject).Difficulty == EnemyDifficulty.ADVANCED)
                    rad2.IsChecked = true;
                else if ((obj as EnemyMapObject).Difficulty == EnemyDifficulty.EXPERT)
                    rad3.IsChecked = true;
                else
                    rad4.IsChecked = true;

                rad1.Checked += RadioButtonEventHandler;
                rad2.Checked += RadioButtonEventHandler;
                rad3.Checked += RadioButtonEventHandler;
                rad4.Checked += RadioButtonEventHandler;
            }
        }

        private TextBox CreateNewTextBox(string boxName, string boxTag, string boxText)
        {
            TextBlock newBlock = new TextBlock();
            newBlock.Text = boxName;
            this.Children.Add(newBlock);
            
            TextBox newBox = new TextBox();
            newBox.Name = boxTag;
            newBox.Text = boxText;
            this.Children.Add(newBox);
            newBox.TextChanged += TextBoxEventHandler;
            newBox.PreviewMouseDoubleClick += HighlightAllText;
            return newBox;
        }

        private CheckBox CreateNewCheckBox(string boxName, string boxTag, bool isChecked)
        {
            TextBlock newBlock = new TextBlock();
            newBlock.Margin = new Thickness(0, 5, 0, 0);
            newBlock.Text = boxName;
            this.Children.Add(newBlock);

            CheckBox newCheckBox = new CheckBox();
            newCheckBox.Name = boxTag;
            newCheckBox.IsChecked = isChecked;
            newCheckBox.Margin = new System.Windows.Thickness(0, 5, 0, 0);
            newCheckBox.Checked += CheckBoxEventHandler;
            newCheckBox.Unchecked += CheckBoxEventHandler;
            this.Children.Add(newCheckBox);
            return newCheckBox;
        }

        private void HighlightAllText(object sender, RoutedEventArgs e)
        {
            TextBox box = sender as TextBox;
            box.SelectAll();
        }

        // The method that runs whenever a text box is changed in the properties control.
        private void TextBoxEventHandler(object sender, TextChangedEventArgs args)
        {
            TextBox textBox = (sender as TextBox);
            float value = 0;

            switch (textBox.Name)
            {
                case ("Name"):
                    m_selectedObj.Name = textBox.Text;
                    ControllerRef.RefreshObjTreeData();
                    break;
                case ("X"):
                    if (float.TryParse(textBox.Text, out value))
                        m_selectedObj.X = value;
                    else
                    {
                        m_selectedObj.X = TestForSpecialKey(textBox.Text, true);
                        if (m_selectedObj.X != 0)
                            textBox.Text = m_selectedObj.X.ToString();
                    }
                    break;
                case ("Y"):
                    if (float.TryParse(textBox.Text, out value))
                        m_selectedObj.Y = value;
                    else
                    {
                        m_selectedObj.Y = TestForSpecialKey(textBox.Text, false);
                        if (m_selectedObj.Y != 0)
                            textBox.Text = m_selectedObj.Y.ToString();
                    }
                    break;
                case("Rotation"):
                    if (float.TryParse(textBox.Text, out value))
                        m_selectedObj.Rotation = value;
                    break;
                case ("ScaleX"):
                    if (float.TryParse(textBox.Text, out value))
                        m_selectedObj.ScaleX = value;
                    break;
                case ("ScaleY"):
                    if (float.TryParse(textBox.Text, out value))
                        m_selectedObj.ScaleY = value;
                    break;
                case ("Width"):
                    if (m_selectedObj is CollHullObj)
                    {
                        if (float.TryParse(textBox.Text, out value))
                            (m_selectedObj as CollHullObj).Width = (int)value;
                        else
                        {
                            (m_selectedObj as CollHullObj).Width = (int)TestForSpecialKey(textBox.Text, true);
                            if ((m_selectedObj as CollHullObj).Width != 0)
                                textBox.Text = (m_selectedObj as CollHullObj).Width.ToString();
                        }
                    }
                    break;
                case ("Height"):
                    if (m_selectedObj is CollHullObj)
                    {
                        if (float.TryParse(textBox.Text, out value))
                            (m_selectedObj as CollHullObj).Height = (int)value;
                        else
                        {
                            (m_selectedObj as CollHullObj).Height = (int)TestForSpecialKey(textBox.Text, false);
                            if ((m_selectedObj as CollHullObj).Height != 0)
                                textBox.Text = (m_selectedObj as CollHullObj).Height.ToString();
                        }
                    }
                    break;
                case("InitialDelay"):
                    if (float.TryParse(textBox.Text, out value))
                    {
                        if (m_selectedObj is EnemyMapObject)
                            (m_selectedObj as EnemyMapObject).InitialLogicDelay = value;
                    }
                    break;
                case("Tags"):
                    if (m_selectedObj is GameObj)
                        (m_selectedObj as GameObj).Tag = textBox.Text;
                    break;
            }
        }

        // A method that ties special buttons to text boxes to change their length.
        private float TestForSpecialKey(string text, bool useWidth = true)
        {
            if (text.Length > 1)
            {
                string letter = text.Substring(text.Length - 1);
                string originalText = text.Substring(0, text.Length - 1);
                float tryParseValue = 0;

                switch (letter)
                {
                    case ("r"):
                        if (float.TryParse(originalText, out tryParseValue) == true)
                        {
                            if (useWidth == true)
                                return tryParseValue * Consts.ScreenWidth;
                            else
                                return tryParseValue * Consts.ScreenHeight;
                        }
                        break;
                    case ("g"):
                        if (float.TryParse(originalText, out tryParseValue) == true)
                        {
                            if (useWidth == true)
                                return tryParseValue * Consts.GridWidth;
                            else
                                return tryParseValue * Consts.GridHeight;
                        }
                        break;
                }
            }
            return 0;
        }

        private void CheckBoxEventHandler(object sender, RoutedEventArgs args)
        {
            CheckBox box = sender as CheckBox;
            switch (box.Name)
            {
                case ("FlipCheckBox"):
                    if (box.IsChecked == true)
                        m_selectedObj.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                    else
                        m_selectedObj.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
                    break;
                case("IsWeighted"):
                    if (box.IsChecked == true)
                        (m_selectedObj as IPhysicsObj).IsWeighted = true;
                    else
                        (m_selectedObj as IPhysicsObj).IsWeighted = false;
                    break;
                case ("IsCollidable"):
                    if (box.IsChecked == true)
                        (m_selectedObj as IPhysicsObj).IsCollidable = true;
                    else
                        (m_selectedObj as IPhysicsObj).IsCollidable = false;
                    break;
                case("PlayAnimation"):
                    if (box.IsChecked == true)
                        (m_selectedObj as IAnimateableObj).PlayAnimation();
                    else
                        (m_selectedObj as IAnimateableObj).StopAnimation();
                    break;
                case("TriggerCheckBox"):
                    if (box.IsChecked == true)
                        (m_selectedObj as CollHullObj).IsTrigger = true;
                    else
                        (m_selectedObj as CollHullObj).IsTrigger = false;
                    break;
                case("CollidesTop"):
                    if (box.IsChecked == true)
                        (m_selectedObj as CollHullObj).CollidesTop = true;
                    else
                        (m_selectedObj as CollHullObj).CollidesTop = false;
                    break;
                case ("CollidesBottom"):
                    if (box.IsChecked == true)
                        (m_selectedObj as CollHullObj).CollidesBottom = true;
                    else
                        (m_selectedObj as CollHullObj).CollidesBottom = false;
                    break;
                case ("CollidesLeft"):
                    if (box.IsChecked == true)
                        (m_selectedObj as CollHullObj).CollidesLeft = true;
                    else
                        (m_selectedObj as CollHullObj).CollidesLeft = false;
                    break;
                case ("CollidesRight"):
                    if (box.IsChecked == true)
                        (m_selectedObj as CollHullObj).CollidesRight = true;
                    else
                        (m_selectedObj as CollHullObj).CollidesRight = false;
                    break;
            }
        }

        private void RadioButtonEventHandler(object sender, RoutedEventArgs args)
        {
            RadioButton button = sender as RadioButton;
            EnemyMapObject enemy = m_selectedObj as EnemyMapObject;

            switch (button.Name)
            {
                case("SelectAll"):
                    (m_selectedObj as IRoomPropertiesObj).SelectionMode = 0;
                    break;
                case ("SelectLayer"):
                    (m_selectedObj as IRoomPropertiesObj).SelectionMode = 1;
                    break;
                case ("SelectNone"):
                    (m_selectedObj as IRoomPropertiesObj).SelectionMode = 2;
                    break;
                case("EnemyDifficultyBasic"):
                    if (enemy != null)
                    {
                        enemy.TextureColor = Color.White;
                        enemy.Scale = enemy.BasicScale;
                        enemy.Difficulty = EnemyDifficulty.BASIC;
                    }
                    break;
                case ("EnemyDifficultyAdvanced"):
                    if (enemy != null)
                    {
                        enemy.TextureColor = Color.Yellow;
                        enemy.Scale = enemy.AdvancedScale;
                        enemy.Difficulty = EnemyDifficulty.ADVANCED;
                    }
                    break;
                case ("EnemyDifficultyExpert"):
                    if (enemy != null)
                    {
                        enemy.TextureColor = Color.Orange;
                        enemy.Scale = enemy.ExpertScale;
                        enemy.Difficulty = EnemyDifficulty.EXPERT;
                    }
                    break;
                case ("EnemyDifficultyMiniboss"):
                    if (enemy != null)
                    {
                        enemy.TextureColor = Color.Red;
                        enemy.Scale = enemy.MinibossScale;
                        enemy.Difficulty = EnemyDifficulty.MINIBOSS;
                    }
                    break;
            }
        }

        public void RefreshPropertiesData()
        {
            if (m_selectedObj != null)
            {
                TextBox box = FindBox("X");
                if (box != null) box.Text = m_selectedObj.X.ToString();

                box = FindBox("Y");
                if (box != null) box.Text = m_selectedObj.Y.ToString();

                box = FindBox("Rotation");
                if (box != null) box.Text = m_selectedObj.Rotation.ToString();

                box = FindBox("Width");
                if (box != null) box.Text = m_selectedObj.Width.ToString();

                box = FindBox("Height");
                if (box != null) box.Text = m_selectedObj.Height.ToString();

                box = FindBox("ScaleX");
                if (box != null) box.Text = m_selectedObj.ScaleX.ToString();

                box = FindBox("ScaleY");
                if (box != null) box.Text = m_selectedObj.ScaleY.ToString();
            }
        }

        private TextBox FindBox(string tag)
        {
            TextBox boxToReturn = null;
            foreach (object obj in this.Children)
            {
                if (obj is TextBox)
                    if ((obj as TextBox).Name == tag)
                        boxToReturn = obj as TextBox;
            }
            return boxToReturn;
        }
    }
}
