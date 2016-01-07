using FirstWave.Unity.Gui.Enums;
using FirstWave.Unity.Gui.Panels;
using FirstWave.Unity.Gui.Primitives;
using System;

namespace FirstWave.Unity.Gui.Controls
{
    /// <summary>
    /// See comments on Menu class for the proper location of this class
    /// </summary>
    public class MenuItem : ContentControl
    {
        private Image pointer;
        private TextBlock labelText;
        private BorderTextures _textures;

        public string Label { get; set; }
        public Action SelectedAction { get; set; }

        /// <summary>
        /// Distance between the pointer and the label text
        /// </summary>
        public float PointerPadding
        {
            get { return pointer.Margin.Right; }
            set { pointer.Margin = new Thickness(0, 0, value, 0); }
        }

        public bool IsSelected { get; internal set; }

        public BorderTextures Textures
        {
            get { return _textures != null ? _textures : GUIManager.Instance.borderTextures; }
            set { _textures = value; }
        }

        public MenuItem(string label, Action selectedAction, BorderTextures textures = null)
        {
            _textures = textures;

            Label = label;
            SelectedAction = selectedAction;

            var child = new StackPanel { Orientation = Orientation.Horizontal };

            pointer = new Image(Textures.Pointer) { VerticalAlignment = Enums.VerticalAlignment.Center };
            labelText = new TextBlock(label);

            PointerPadding = 10;

            child.AddChild(pointer);
            child.AddChild(labelText);

            Child = child;
        }

        public override void Draw()
        {
            pointer.Visibility = IsSelected ? Visibility.Visible : Visibility.Hidden;

            base.Draw();
        }
    }
}
