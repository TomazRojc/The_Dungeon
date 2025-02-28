using System;

namespace Code.UI
{
    public enum Direction
    {
        Center,
        Up,
        Down,
        Left,
        Right
    }
    public class UIEventBus
    {
        public Action<Direction, int> OnNavigate;
        public Action<int> OnSubmit;
        public Action<int> OnCancel;
        public Action<int> OnEscape;
    }
}