using System;

namespace Code {
    public enum Direction {
        Up,
        Down,
        Left,
        Right
    }
    public class UiManager {
        
        public Action<Direction, int> OnNavigate;
        public Action<int> OnSubmit;
        public Action<int> OnBack;

    }
}