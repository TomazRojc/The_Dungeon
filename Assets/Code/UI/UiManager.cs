using System;

namespace Code.UI
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        Center
    }
    public class UiManager
    {
        public Action<Direction, int> OnNavigate;
        public Action<int> OnSubmit;
        public Action<int> OnCancel;
        public Action<int> OnEscape;
    }
}