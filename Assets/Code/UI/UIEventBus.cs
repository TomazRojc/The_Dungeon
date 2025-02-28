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
    public class UIEventBus
    {
        public Action<Direction, int> OnNavigate;
        public Action<int> OnSubmit;
        public Action<int> OnCancel;
        public Action<int> OnEscape;
    }
}